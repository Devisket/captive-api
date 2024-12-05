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

            entity.Property(x=> x.BRSTN).IsRequired();

            entity.Property(x=> x.DeliverTo).IsRequired(false);

            entity.Property(x => x.AccountName).IsRequired();
                 
            entity.Property(x => x.Concode)
                .IsRequired(false);

            entity.Property(x => x.OrderQuanity).IsRequired();

            entity.HasMany(x => x.CheckInventoryDetail)
              .WithOne(x => x.CheckOrder);
              
            entity.ToTable("check_orders");
        }
    }
}
