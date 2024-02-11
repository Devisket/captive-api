using Captive.Data;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Processing.Processor;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Captive.Commands.Extensions
{
    public static class ServiceConfigurationExtensions
    {
        public static void ConfigureExtensionServices(this IServiceCollection services, IConfiguration config)
        {
            var connString = config.GetConnectionString("TestConfiguration");

            if (connString == null)
            {
                throw new ArgumentNullException("Connection string");
            }

            services
                 .AddDbContext<CaptiveDataContext>(options =>
                options.UseMySQL(connString, b => b.MigrationsAssembly("Captive.Commands")));
         
            services.AddScoped<IFileProcessor, FileProcessor>();
            services.AddScoped<IReadUnitOfWork, ReadUnitOfWork>();
            services.AddScoped<IWriteUnitOfWork, WriteUnitOfWork>();

            var assembly = Assembly.Load("Captive.Applications");
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        }
    }
}
 