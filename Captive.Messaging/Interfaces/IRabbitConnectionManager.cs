using RabbitMQ.Client;

namespace Captive.Messaging.Interfaces
{
    public interface IRabbitConnectionManager
    {
        Task<IConnection> GetRabbitMQConnectionAsync(CancellationToken cancellationToken = default);
    }
}
