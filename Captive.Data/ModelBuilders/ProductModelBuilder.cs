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
                .HasForeignKey(x => x.ProductId);

            entity.ToTable("products");
        }
    }
}
