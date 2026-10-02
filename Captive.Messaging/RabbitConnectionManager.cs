using Captive.Messaging.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Captive.Messaging
{
    public class RabbitConnectionManager : IRabbitConnectionManager
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IConfiguration _configuration;

        public RabbitConnectionManager(IConnectionFactory connectionFactory, IConfiguration configuration)
        {
            _configuration = configuration;

            _connectionFactory = new ConnectionFactory()
            {
                HostName = _configuration["Rabbitmq:Hostname"],
                Port = int.Parse(_configuration["Rabbitmq:Port"]!),
                UserName = _configuration["Rabbitmq:Username"],
                Password = _configuration["Rabbitmq:Password"],
            };
        }

        public Task<IConnection> GetRabbitMQConnectionAsync(CancellationToken cancellationToken = default)
        {
            return _connectionFactory.CreateConnectionAsync(cancellationToken);
        }
    }
}
