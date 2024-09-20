using Captive.Messaging.Interfaces;
using RabbitMQ.Client;


namespace Captive.Messaging
{
    public class ConsumerManager
    {
        private readonly IRabbitConnectionManager _connectionManager;
        private readonly IConnectionFactory _connectionFactory;

        public ConsumerManager(IRabbitConnectionManager connectionManager) {
            _connectionManager = connectionManager;

        }

        
    }
}
