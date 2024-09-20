using Captive.Messaging.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Port = 5672,
                UserName = _configuration["Rabbitmq:Username"],
                Password = _configuration["Rabbitmq:Password"],
            };
        }

        public IConnection GetRabbitMQConnection()
        {
            return _connectionFactory.CreateConnection();
        }
    }
}
