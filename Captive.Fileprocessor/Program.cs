using Captive.Fileprocessor.Services.CheckOrderService;
using Captive.Fileprocessor.Services.DbfService;
using Captive.Fileprocessor.Services.FileProcessOrchestrator.cs;
using Captive.Fileprocessor.Services.GenerateBarcodeService;
using Captive.Fileprocessor.Services.Barcode;
using Captive.Fileprocessor.Services.Barcode.Implementations;
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
