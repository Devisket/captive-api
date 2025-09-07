using Captive.Messaging.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Captive.Orchestrator
{
    public class SampleConsumer : BackgroundService
    {
        private readonly IRabbitConnectionManager _rabbitConnManager;
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;

        public SampleConsumer(IRabbitConnectionManager rabbitConnManager, ILoggerFactory loggerFactory)
        {
            this._logger = loggerFactory.CreateLogger<SampleConsumer>();
            _rabbitConnManager = rabbitConnManager;

            _connection = _rabbitConnManager.GetRabbitMQConnection();

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("sample.exchange", ExchangeType.Topic);
            _channel.QueueDeclare(queue: "sample", durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind("sample", "sample.exchange", "sample.*", null);
            _channel.BasicQos(0, 1, false);
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // received message
                var content = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());

                // handle the received message
                HandleMessage(content);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume("sample", false, consumer);

            return Task.CompletedTask;
        }

        private void HandleMessage(string content)
        {
            // we just print this message
            _logger.LogInformation($"consumer received {content}");
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
