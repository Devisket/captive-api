using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static class ProductModelBuilder
    {
        public static void BuildProductModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Product>();

            entity.HasKey(x => x.Id);

            entity.Property(x => x.ProductName)
                .IsRequired();

            entity.HasMany(x => x.FormChecks)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.OrderFiles)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            entity.HasOne(x => x.ProductConfiguration).WithOne(x => x.Product)
                .HasForeignKey<ProductConfiguration>(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable("products");
        }
    }
}
