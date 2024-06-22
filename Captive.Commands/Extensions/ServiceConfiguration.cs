﻿using Captive.Data;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Processing.Processor.ExcelFileProcessor;
using Captive.Processing.Processor.TextFileProcessor;
using Captive.Reports;
using Captive.Reports.BlockReport;
using Captive.Reports.PackingReport;
using Captive.Reports.PrinterFileReport;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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
                options.UseSqlServer(connString, b => b.MigrationsAssembly("Captive.Commands")));
         
            services.AddScoped<ITextFileProcessor, TextFileProcessor>();
            services.AddScoped<IReadUnitOfWork, ReadUnitOfWork>();
            services.AddScoped<IWriteUnitOfWork, WriteUnitOfWork>();
            services.AddScoped<IPrinterFileReport, PrinterFileReport>();
            services.AddScoped<IReportGenerator, ReportGenerator>();
            services.AddScoped<IBlockReport, BlockReport>();
            services.AddScoped<IPackingReport, PackingReport>();
            services.AddScoped<IExcelFileProcessor, ExcelFileProcessor>();


            var assembly = Assembly.Load("Captive.Applications");
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        }
    }
}
 