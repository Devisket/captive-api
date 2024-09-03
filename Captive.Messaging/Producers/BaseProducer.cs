
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Captive.Messaging.Producers
{
    public abstract class BaseProducer<T> : IProducer<T> where T : class
    {
        private readonly IConnectionFactory _connectionFactory;

        public BaseProducer(IConnectionFactory connectionFactory) { 
            _connectionFactory = connectionFactory;
        }
        public abstract string queueName { get;}

        public async void ProduceMessage(T message)
        {
            using (var con = _connectionFactory.CreateConnection()) { 
                using(var channel = con.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName, false, exclusive: false, autoDelete: false, arguments: null);

                    string queueMessage = JsonConvert.SerializeObject(message);

                    var body = Encoding.UTF8.GetBytes(queueMessage);

                    await Task.Run(() => channel.BasicPublish(exchange: string.Empty, routingKey: queueName, basicProperties: null, body: body));
                }
            }
        }
    }
}
