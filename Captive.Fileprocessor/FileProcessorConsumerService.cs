using Captive.Fileprocessor.Services.FileProcessOrchestrator.cs;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Captive.Fileprocessor
{
    public class FileProcessorConsumerService : BackgroundService
    {
        private readonly IRabbitConnectionManager _rabbitConnManager;
        private readonly IFileProcessOrchestratorService _fileOrchestrator;
        public FileProcessorConsumerService(IRabbitConnectionManager rabbitConnManager, IFileProcessOrchestratorService fileOrchestrator)
        {
            _rabbitConnManager = rabbitConnManager;
            _fileOrchestrator = fileOrchestrator;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connection = _rabbitConnManager.GetRabbitMQConnection();

            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "CaptiveFileUpload", durable: false, exclusive: false, autoDelete: false, arguments:null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) => 
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var fileUpload = JsonConvert.DeserializeObject<FileUploadMessage>(message);            

                await _fileOrchestrator.ProcessFile(fileUpload);
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume("CaptiveFileUpload", true,consumer);
        }
    }
}
