using Captive.Data;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Messaging.Models;
using Captive.Messaging.Producers.Messages;
using Captive.Messaging.Producers;
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
            services.AddScoped<IExcelFileProcessor, ExcelFileProcessor>();

            services.AddSingleton<IConnectionFactory, ConnectionFactory>();
            services.AddSingleton<IRabbitConnectionManager, RabbitConnectionManager>();
            services.AddScoped<IProducer<FileUploadMessage>, FileUploadProducerMessage>();

            var assembly = Assembly.Load("Captive.Applications");
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        }
    }
}
