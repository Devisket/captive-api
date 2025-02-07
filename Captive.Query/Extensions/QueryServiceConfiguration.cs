using Captive.Data;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Messaging.Models;
using Captive.Messaging.Producers.Messages;
using Captive.Messaging;
using Captive.Processing.Processor.ExcelFileProcessor;
using Captive.Processing.Processor.TextFileProcessor;
using Captive.Reports;
using Captive.Reports.BlockReport;
using Captive.Reports.PackingReport;
using Captive.Reports.PrinterFileReport;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Reflection;
using Captive.Messaging.Interfaces;
using Captive.Applications.CheckOrder.Services;
using Captive.Applications.FormsChecks.Services;
using Captive.Applications.Util;
using Captive.Applications.Batch.Services;
using Captive.Applications.CheckValidation.Services;
using Captive.Applications.CheckInventory.Services;
using Captive.Applications.Orderfiles.Services;

namespace Captive.Queries.Extensions
{
    public static class QueryServiceConfiguration
    {
        public static void ConfigureQueryServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connString = configuration.GetConnectionString("DefaultConnection");

            if (connString == null)
            {
                throw new ArgumentNullException("DefaultConnection");
            }

            services
                 .AddDbContext<CaptiveDataContext>(options =>
                options.UseSqlServer(connString));

            services.AddScoped<ITextFileProcessor, TextFileProcessor>();
            services.AddScoped<IReadUnitOfWork, ReadUnitOfWork>();
            services.AddScoped<IWriteUnitOfWork, WriteUnitOfWork>();
            services.AddScoped<IPrinterFileReport, PrinterFileReport>();
            services.AddScoped<IReportGenerator, ReportGenerator>();
            services.AddScoped<IBlockReport, BlockReport>();
            services.AddScoped<IPackingReport, PackingReport>();
            services.AddSignalR();
            services.AddScoped<IBatchService, BatchService>();
            services.AddScoped<IExcelFileProcessor, ExcelFileProcessor>();
            services.AddScoped<IFormsChecksService, FormsChecksService>();
            services.AddScoped<ICheckOrderService, CheckOrderService>();
            services.AddScoped<IStringService, StringService>();
            services.AddScoped<ICheckValidationService, CheckValidationService>();
            services.AddScoped<ICheckInventoryService,CheckInventoryService>();
            services.AddSingleton<IConnectionFactory, ConnectionFactory>();
            services.AddSingleton<IRabbitConnectionManager, RabbitConnectionManager>();
            services.AddScoped<IProducer<DbfGenerateMessage>, DbfProducerMessage>();
            services.AddScoped<IProducer<FileUploadMessage>, FileUploadProducerMessage>();
            services.AddScoped<IOrderFileService, OrderFileService>();

            var assembly = Assembly.Load("Captive.Applications");
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        }
    }
}
