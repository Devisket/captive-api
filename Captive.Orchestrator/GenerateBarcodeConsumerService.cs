using Captive.Orchestrator.Services.GenerateBarcodeService;
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
    public class GenerateBarcodeConsumerService : BackgroundService
    {

        private ILogger<GenerateBarcodeConsumerService> _logger;
        private readonly IRabbitConnectionManager _rabbitConnManager;
        private IConnection _connection;
        private IChannel _channel;
        private readonly IGenerateBarcodeService _generateBarcodeService;

        public GenerateBarcodeConsumerService(
            IRabbitConnectionManager rabbitConnManager, 
            ILogger<GenerateBarcodeConsumerService> logger,
            IGenerateBarcodeService generateBarcodeService)
        {
            _logger = logger;
            _rabbitConnManager = rabbitConnManager;
            _generateBarcodeService = generateBarcodeService;
        }



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connection = await _rabbitConnManager.GetRabbitMQConnectionAsync(stoppingToken);

            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(queue: "GenerateBarcode", durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var deserializedMessage = JsonConvert.DeserializeObject<GenerateBarcodeMessage>(message);

                    if(deserializedMessage == null)
                    {
                        _logger.LogError("Cannot serialize GenerateBarcodeMessage");
                        return;
                    }
                    else
                    {
                        await _generateBarcodeService.GenerateBarcode(deserializedMessage.BankId, deserializedMessage.BatchId, deserializedMessage.BarcodeService, deserializedMessage.CheckOrderBarcode);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return;
                }
            };

            await _channel.BasicConsumeAsync("GenerateBarcode", true, consumer, stoppingToken);
        }
    }
}
