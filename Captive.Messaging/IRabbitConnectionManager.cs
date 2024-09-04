using RabbitMQ.Client;

namespace Captive.Messaging
{
    public interface IRabbitConnectionManager
    {
        IConnectionFactory GetRabbitMQConnection();
    }
}
