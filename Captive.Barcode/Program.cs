using Captive.Barcode.Base;
using Captive.Barcode.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Captive.Barcode
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            
            // Configure Windows Service
            builder.Services.AddWindowsService(options =>
            {
                options.ServiceName = "Captive Barcode Service";
            });

            // Configure logging
            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddEventLog(settings =>
                {
                    settings.SourceName = "Captive Barcode Service";
                });
            });

            // Register barcode services
            builder.Services.AddBarcodeServices();
            
            // Register the main worker service
            builder.Services.AddHostedService<BarcodeWorkerService>();
            
            // Register the RabbitMQ consumer service
            builder.Services.AddHostedService<RabbitMQConsumerService>();

            // Configure service settings
            builder.Services.Configure<BarcodeServiceSettings>(
                builder.Configuration.GetSection("BarcodeService"));
                
            builder.Services.Configure<RabbitMQSettings>(
                builder.Configuration.GetSection("RabbitMQ"));

            var host = builder.Build();
            host.Run();
        }
    }
} 