using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static class ProductTypeModelBuilder
    {
        public static void BuildProductTypeModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<ProductType>();

            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.BankInfo)
                .WithMany(x => x.ProductTypes)
                .HasForeignKey(x => x.BankInfoId);

            entity.Property(x => x.ProductName)
                .IsRequired();

            entity.ToTable("product_type");
        }
    }
}
