using Captive.Data;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Messaging;
using Captive.Messaging.Models;
using Captive.Messaging.Producers.Messages;
using Captive.Processing.Processor.ExcelFileProcessor;
using Captive.Processing.Processor.TextFileProcessor;
using Captive.Reports;
using Captive.Reports.BlockReport;
using Captive.Reports.PackingReport;
using Captive.Reports.PrinterFileReport;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Reflection;
using Captive.Utility;
using MediatR;
using Captive.Commands.Pipelines;
using Captive.Messaging.Interfaces;
using Captive.Applications.FormsChecks.Services;
using Captive.Applications.CheckOrder.Services;
using Captive.Applications.Util;
using Captive.Applications.Orderfiles.Services;
using Captive.Applications.Batch.Services;

namespace Captive.Commands.Extensions
{
    public static class ServiceConfigurationExtensions
    {
        public static void ConfigureExtensionServices(this IServiceCollection services, IConfiguration config)
        {
            var connString = config.GetConnectionString("DefaultConnection");

            if (connString == null)
            {
                throw new ArgumentNullException("DefaultConnection");
            }

            services
                 .AddDbContext<CaptiveDataContext>(options =>
                options.UseSqlServer(connString, b => b.MigrationsAssembly("Captive.Commands")), ServiceLifetime.Scoped);
            services.AddScoped<ITextFileProcessor, TextFileProcessor>();
            services.AddScoped<IReadUnitOfWork, ReadUnitOfWork>();
            services.AddScoped<IWriteUnitOfWork, WriteUnitOfWork>();
            services.AddScoped<IPrinterFileReport, PrinterFileReport>();
            services.AddScoped<IReportGenerator, ReportGenerator>();
            services.AddScoped<IBlockReport, BlockReport>();
            services.AddScoped<IPackingReport, PackingReport>();
            services.AddScoped<IExcelFileProcessor, ExcelFileProcessor>();
            services.AddScoped<IFormsChecksService, FormsChecksService>();
            services.AddScoped<ICheckOrderService, CheckOrderService>();
            services.AddScoped<IStringService, StringService>();
            services.AddScoped<IOrderFileService, OrderFileService>();
            services.AddSingleton<IConnectionFactory, ConnectionFactory>();
            services.AddScoped<IBatchService, BatchService>();
            services.AddSignalR();
            services.AddSingleton<IRabbitConnectionManager, RabbitConnectionManager>();
            services.AddScoped<IProducer<FileUploadMessage>, FileUploadProducerMessage>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DatabasePipeline<,>));
            var assembly = Assembly.Load("Captive.Applications");
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        }

        public static async Task MigrateDatabase(this WebApplication webApplication, ILogger logger)
        {
            try
            {
                using (var scope = webApplication.Services.CreateScope()) 
                {
                    var context = scope.ServiceProvider.GetRequiredService<CaptiveDataContext>();

                    logger.LogInformation("Checking for pending migration");

                    var pendingMigration = await context.Database.GetPendingMigrationsAsync();
                    
                    if (pendingMigration.Any())
                    {
                        logger.LogInformation($"There are {pendingMigration.Count()} pending migrations");

                        await context.Database.MigrateAsync();

                        logger.LogInformation("Migration successfully run");
                    }
                }                
            }
            catch (Exception ex) 
            {
                logger.LogError(ex.Message);
            }

            return;
        }
    }
}
 