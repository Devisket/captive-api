using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static  class CheckOrdersModelBuilder
    {
        public static void BuildCheckOrdersModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<CheckOrders>();

            entity.Property(x=> x.AccountNo).IsRequired();

            entity.Property(x=> x.OrderFileId).IsRequired();

            entity.Property(x=> x.BRSTN).IsRequired();

            entity.Property(x=> x.DeliverTo).IsRequired(false);

            entity.Property(x => x.AccountName).IsRequired();
                        
            entity.HasOne(x => x.OrderFile)
                .WithMany(x => x.CheckOrders)
                .HasForeignKey(x => x.OrderFileId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(x => x.FormChecks)
                .WithMany(x => x.CheckOrders)
                .HasForeignKey(x => x.FormCheckId);

            entity.Property(x => x.Concode)
                .IsRequired(false);

            entity.Property(x => x.OrderQuanity).IsRequired();

            entity.ToTable("check_orders");
        }
    }
}
