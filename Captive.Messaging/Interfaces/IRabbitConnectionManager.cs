using RabbitMQ.Client;

namespace Captive.Messaging.Interfaces
{
    public interface IRabbitConnectionManager
    {
        IConnection GetRabbitMQConnection();
    }
}
