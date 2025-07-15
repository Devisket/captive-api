using Captive.Fileprocessor.Services.GenerateBarcodeService;
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
    public class GenerateBarcodeConsumerService : BackgroundService
    {

        private ILogger<GenerateBarcodeConsumerService> _logger;
        private readonly IRabbitConnectionManager _rabbitConnManager;
        private IConnection _connection;
        private IModel _channel;
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



        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connection = _rabbitConnManager.GetRabbitMQConnection();

            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "GenerateBarcode", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var deserializedMessage = JsonConvert.DeserializeObject<GenerateBarcodeMessage>(message);

                    if(deserializedMessage == null)
                    {
                        _logger.LogError("Cannot serialize GenerateBarcodeMessage");
                        _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
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
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    return;
                }
                
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume("GenerateBarcode", false, consumer);

            return Task.CompletedTask;
        }
    }
}
