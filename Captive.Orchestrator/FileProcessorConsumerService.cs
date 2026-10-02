using Captive.Orchestrator.Services.FileProcessOrchestrator.cs;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Captive.Orchestrator
{
    public class FileProcessorConsumerService : BackgroundService
    {
        private ILogger<FileProcessorConsumerService> _logger;
        private readonly IRabbitConnectionManager _rabbitConnManager;
        private readonly IFileProcessOrchestratorService _fileOrchestrator;

        private IConnection _connection;
        private IChannel _channel;

        public FileProcessorConsumerService(IRabbitConnectionManager rabbitConnManager, IFileProcessOrchestratorService fileOrchestrator, ILoggerFactory loggerFactory)
        {
            _rabbitConnManager = rabbitConnManager;
            _fileOrchestrator = fileOrchestrator;
            _logger = loggerFactory.CreateLogger<FileProcessorConsumerService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            _connection = await _rabbitConnManager.GetRabbitMQConnectionAsync(stoppingToken);

            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(queue: "CaptiveFileUpload", durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) => 
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var fileUpload = JsonConvert.DeserializeObject<FileUploadMessage>(message);

                    await _fileOrchestrator.ProcessFile(fileUpload);
                   
                }
                catch (Exception ex) { 
                    _logger.LogError(ex.Message);
                }

                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            // v7 consumers expose async events; ConsumerCancelled no longer exists (use UnregisteredAsync).
            consumer.ShutdownAsync += OnConsumerShutdown;
            consumer.RegisteredAsync += OnConsumerRegistered;
            consumer.UnregisteredAsync += OnConsumerUnregistered;

            await _channel.BasicConsumeAsync("CaptiveFileUpload", false, consumer, stoppingToken);
        }

        private Task OnConsumerUnregistered(object sender, ConsumerEventArgs e) => Task.CompletedTask;
        private Task OnConsumerRegistered(object sender, ConsumerEventArgs e) => Task.CompletedTask;
        private Task OnConsumerShutdown(object sender, ShutdownEventArgs e) => Task.CompletedTask;

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
