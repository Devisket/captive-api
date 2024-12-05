using Captive.Fileprocessor.Services.FileProcessOrchestrator.cs;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Captive.Fileprocessor
{
    public class FileProcessorConsumerService : BackgroundService
    {
        private ILogger<FileProcessorConsumerService> _logger;
        private readonly IRabbitConnectionManager _rabbitConnManager;
        private readonly IFileProcessOrchestratorService _fileOrchestrator;

        private IConnection _connection;
        private IModel _channel;

        public FileProcessorConsumerService(IRabbitConnectionManager rabbitConnManager, IFileProcessOrchestratorService fileOrchestrator, ILoggerFactory loggerFactory)
        {
            _rabbitConnManager = rabbitConnManager;
            _fileOrchestrator = fileOrchestrator;
            _logger = loggerFactory.CreateLogger<FileProcessorConsumerService>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            _connection = _rabbitConnManager.GetRabbitMQConnection();

            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "CaptiveFileUpload", durable: false, exclusive: false, autoDelete: false, arguments:null);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) => 
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

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume("CaptiveFileUpload", false, consumer);

            return Task.CompletedTask;
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
