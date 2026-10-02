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
        private IChannel? _channel;

        public BatchProcessConsumerService(
            IServiceScopeFactory scopeFactory,
            IRabbitConnectionManager rabbitConnManager,
            ILoggerFactory loggerFactory)
        {
            _scopeFactory = scopeFactory;
            _rabbitConnManager = rabbitConnManager;
            _logger = loggerFactory.CreateLogger<BatchProcessConsumerService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connection = await _rabbitConnManager.GetRabbitMQConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
            await _channel.QueueDeclareAsync(queue: "CaptiveBatchProcess", durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);

            var channel = _channel;
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
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

                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            await channel.BasicConsumeAsync("CaptiveBatchProcess", false, consumer, stoppingToken);
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
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
