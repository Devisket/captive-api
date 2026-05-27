using Captive.Applications.Batch.Services;
using Captive.Data.Enums;
using Captive.Data.UnitOfWork.Write;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Captive.Commands.BackgroundServices
{
    public class BatchProcessConsumerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IRabbitConnectionManager _rabbitConnManager;
        private readonly ILogger<BatchProcessConsumerService> _logger;

        private IConnection? _connection;
        private IModel? _channel;

        public BatchProcessConsumerService(
            IServiceScopeFactory scopeFactory,
            IRabbitConnectionManager rabbitConnManager,
            ILoggerFactory loggerFactory)
        {
            _scopeFactory = scopeFactory;
            _rabbitConnManager = rabbitConnManager;
            _logger = loggerFactory.CreateLogger<BatchProcessConsumerService>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connection = _rabbitConnManager.GetRabbitMQConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "CaptiveBatchProcess", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = JsonConvert.DeserializeObject<BatchProcessMessage>(Encoding.UTF8.GetString(body));

                    if (message != null)
                        await ProcessBatchJobAsync(message, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing batch job message");
                }

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume("CaptiveBatchProcess", false, consumer);

            return Task.CompletedTask;
        }

        private async Task ProcessBatchJobAsync(BatchProcessMessage message, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var orchestrator = scope.ServiceProvider.GetRequiredService<IBatchProcessingOrchestratorService>();

            try
            {
                await orchestrator.ProcessAsync(message.JobId, message.BatchId, message.ForceProcess, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in batch processing orchestrator for job {JobId}", message.JobId);

                try
                {
                    var writeUow = scope.ServiceProvider.GetRequiredService<IWriteUnitOfWork>();
                    var job = await writeUow.BatchJobs.GetAll()
                        .FirstOrDefaultAsync(x => x.Id == message.JobId, cancellationToken);

                    if (job != null)
                    {
                        job.Status = BatchJobStatus.Failed;
                        job.ErrorMessage = ex.Message;
                        job.UpdatedAt = DateTime.UtcNow;
                        await writeUow.Complete(cancellationToken);
                    }
                }
                catch (Exception innerEx)
                {
                    _logger.LogError(innerEx, "Failed to update job status to Failed for job {JobId}", message.JobId);
                }
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
