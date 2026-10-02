using Captive.Messaging.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Captive.Messaging.Base
{
    public abstract class BaseProducer<T> : IProducer<T> where T : class
    {
        private readonly IRabbitConnectionManager _connectionFactory;

        public BaseProducer(IRabbitConnectionManager connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public abstract string queueName { get; }

        public async void ProduceMessage(T message)
        {
            using (var con = await _connectionFactory.GetRabbitMQConnectionAsync())
            {
                using (var channel = await con.CreateChannelAsync())
                {
                    await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                    string queueMessage = JsonConvert.SerializeObject(message);

                    var body = Encoding.UTF8.GetBytes(queueMessage);

                    await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body);
                }
            }
        }
    }
}
