using Captive.Orchestrator.Services.DbfService;
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
    public class DbfRequestConsumerService : BackgroundService
    {
        private ILogger<DbfRequestConsumerService> _logger;
        private readonly IRabbitConnectionManager _rabbitConnManager;
        private readonly IDbfService _dbfService;
        private IConnection _connection;
        private IModel _channel;

        public DbfRequestConsumerService(IRabbitConnectionManager rabbitConnManager, IFileProcessOrchestratorService fileOrchestrator, ILoggerFactory loggerFactory, IDbfService dbfService)
        {
            _logger = loggerFactory.CreateLogger<DbfRequestConsumerService>();
            _rabbitConnManager = rabbitConnManager;
            _dbfService = dbfService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connection = _rabbitConnManager.GetRabbitMQConnection();

            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "DbfGenerate", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var dbfGenerateMessage = JsonConvert.DeserializeObject<DbfGenerateMessage>(message);
                    await _dbfService.GenerateDbfFile(dbfGenerateMessage.BatchId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume("DbfGenerate", false, consumer);

            return Task.CompletedTask;
        }
    }
}
