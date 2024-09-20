using Captive.Messaging;
using Captive.Messaging.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace Captive.Fileprocessor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            builder.Configuration
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json");
            builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>();
            builder.Services.AddSingleton<IRabbitConnectionManager, RabbitConnectionManager>();

            builder.Services.AddHostedService<FileProcessorConsumerService>();

            IHost host = builder.Build();


            host.Run();
        }
    }
}
