using Captive.Data.Enums;
using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static class ProductConfigurationModelBuilder
    {
        public static void BuildProductConfigurationModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<ProductConfiguration>();

            entity.HasKey(x => x.Id);

            entity.Property(x => x.ConfigurationData).IsRequired(true);

            entity.Property(x => x.ConfigurationType).IsRequired().HasConversion(
                v => v.ToString(),
                v => (ConfigurationType)Enum.Parse(typeof(ConfigurationType), v.ToString()));

            entity.ToTable("product_configuration");
        }
    }
}
