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

            entity.ToTable("product_configuration");
        }
    }
}
