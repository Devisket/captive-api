using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static  class CheckInventoryModelBuilder
    {
        public static void BuildCheckInventoryTable(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<CheckInventory>();

            entity.HasKey(x => x.Id);

            entity.Property(x => x.CheckOrderId).IsRequired(false);

            entity.HasOne(x=> x.FormChecks)
                .WithMany(x => x.CheckInventory)
                .HasForeignKey(x => x.FormCheckId);

            entity.Property(x => x.Quantity).IsRequired();

            entity.HasOne(x => x.BankBranch)
                .WithMany(x => x.CheckInventory)
                .HasForeignKey(x => x.BranchId);

            entity.ToTable("check_inventory");
        }
    }
}
