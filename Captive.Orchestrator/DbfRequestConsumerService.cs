using Captive.Orchestrator.Services.DbfService;
using Captive.Messaging.Interfaces;
using Captive.Messaging.Models;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection _connection;
        private IChannel _channel;

        public DbfRequestConsumerService(IRabbitConnectionManager rabbitConnManager, ILoggerFactory loggerFactory, IServiceScopeFactory scopeFactory)
        {
            _logger = loggerFactory.CreateLogger<DbfRequestConsumerService>();
            _rabbitConnManager = rabbitConnManager;
            _scopeFactory = scopeFactory;
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
                    using var scope = _scopeFactory.CreateScope();
                    var dbfService = scope.ServiceProvider.GetRequiredService<IDbfService>();
                    await dbfService.GenerateDbfFile(dbfGenerateMessage.BatchId, dbfGenerateMessage.outputFolder);
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
