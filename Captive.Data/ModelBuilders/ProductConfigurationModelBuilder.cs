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
            entity.HasOne(x => x.ProductType)
                .WithMany(x => x.ProductConfiguration)
                .HasForeignKey(x => x.ProductTypeId);

            entity
                .HasOne(x => x.OrderFileConfiguration)
                .WithMany(x => x.ProductConfigurations)
                .HasForeignKey(x => x.OrderFileConfigurationId);

            entity.ToTable("product_configuration");
        }
    }
}
