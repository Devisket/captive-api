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
        private IChannel _channel;

        public DbfRequestConsumerService(IRabbitConnectionManager rabbitConnManager, IFileProcessOrchestratorService fileOrchestrator, ILoggerFactory loggerFactory, IDbfService dbfService)
        {
            _logger = loggerFactory.CreateLogger<DbfRequestConsumerService>();
            _rabbitConnManager = rabbitConnManager;
            _dbfService = dbfService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connection = await _rabbitConnManager.GetRabbitMQConnectionAsync(stoppingToken);

            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(queue: "DbfGenerate", durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var dbfGenerateMessage = JsonConvert.DeserializeObject<DbfGenerateMessage>(message);
                    await _dbfService.GenerateDbfFile(dbfGenerateMessage.BatchId, dbfGenerateMessage.outputFolder);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            await _channel.BasicConsumeAsync("DbfGenerate", false, consumer, stoppingToken);
        }
    }
}
