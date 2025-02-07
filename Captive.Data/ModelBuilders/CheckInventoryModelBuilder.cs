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

            entity.HasMany(x => x.CheckInventoryDetails)
                .WithOne(x => x.CheckInventory)
                .HasForeignKey(x => x.CheckInventoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable("check_inventory");
        }
    }
}
