using Captive.Data;
using Captive.Data.UnitOfWork.Read;
using Captive.Data.UnitOfWork.Write;
using Captive.Processing.Processor;
using Microsoft.EntityFrameworkCore;
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
                options.UseMySQL(connString));

            services.AddScoped<IFileProcessor, FileProcessor>();
            services.AddScoped<IReadUnitOfWork, ReadUnitOfWork>();
            services.AddScoped<IWriteUnitOfWork, WriteUnitOfWork>();

            var assembly = Assembly.Load("Captive.Applications");
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        }
    }
}
