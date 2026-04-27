using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static class CheckInventoryMappingModelBuilder
    {
        public static void BuildCheckInventoryMappingModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<CheckInventoryMapping>();

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.CheckInventoryId);
            entity.HasIndex(x => x.BranchId);
            entity.HasIndex(x => x.ProductId);
            entity.HasIndex(x => x.FormCheckType);

            entity.Property(x => x.FormCheckType).HasMaxLength(50).IsRequired(false);
            entity.Property(x => x.BranchId).IsRequired(false);
            entity.Property(x => x.ProductId).IsRequired(false);

            entity.HasOne(x => x.CheckInventory)
                .WithMany(x => x.Mappings)
                .HasForeignKey(x => x.CheckInventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable("check_inventory_mapping");
        }
    }
}
