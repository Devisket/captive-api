using Captive.Barcode.BarcodeImplementation;
using Microsoft.Extensions.DependencyInjection;

namespace Captive.Barcode.Base
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBarcodeServices(this IServiceCollection services)
        {
            // Register all barcode service implementations
            services.AddTransient<IBarcodeService, MbtcBarcodeService>();
            
            // Register the factory
            services.AddTransient<IBarcodeServiceFactory, BarcodeServiceFactory>();
            
            return services;
        }
    }
} 