using Captive.Fileprocessor.Services.CheckOrderService;
using Captive.Fileprocessor.Services.DbfService;
using Captive.Fileprocessor.Services.FileProcessOrchestrator.cs;
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
            builder.Services.AddScoped<IFileProcessOrchestratorService, FileProcessOrchestratorService>();
            builder.Services.AddScoped<IDbfService,DbfService>();

            builder.Services.AddScoped<ICheckOrderService, CheckOrderService>();
            builder.Services.AddHostedService<FileProcessorConsumerService>();
            builder.Services.AddHostedService<DbfRequestConsumerService>();
            builder.Services.AddHostedService<SampleConsumer>();

            IHost host = builder.Build();

            host.Run();
        }
    }
}
