using Captive.Orchestrator.Services.CheckOrderService;
using Captive.Orchestrator.Services.DbfService;
using Captive.Orchestrator.Services.FileProcessOrchestrator.cs;
using Captive.Orchestrator.Services.GenerateBarcodeService;
using Captive.Orchestrator.Services.Barcode;
using Captive.Orchestrator.Services.Barcode.Implementations;
using Captive.Messaging;
using Captive.Messaging.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace Captive.Orchestrator
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
            builder.Services.AddScoped<IGenerateBarcodeService, GenerateBarcodeService>();  

            builder.Services.AddScoped<ICheckOrderService, CheckOrderService>();
            
            // Register HttpClient
            builder.Services.AddHttpClient();
            
            // Register barcode implementations
            builder.Services.AddScoped<IBarcodeImplementationService, MTBCBarcodeService>();
            // Add more barcode implementations here as needed
            
            // Register barcode factory
            builder.Services.AddScoped<IBarcodeImplementationFactory, BarcodeImplementationFactory>();
            
            builder.Services.AddHostedService<FileProcessorConsumerService>();
            builder.Services.AddHostedService<DbfRequestConsumerService>();
            builder.Services.AddHostedService<GenerateBarcodeConsumerService>();
            builder.Services.AddHostedService<SampleConsumer>();

            IHost host = builder.Build();

            host.Run();
        }
    }
}
