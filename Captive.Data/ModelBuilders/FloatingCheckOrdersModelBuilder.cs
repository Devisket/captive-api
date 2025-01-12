using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static class FloatingCheckOrdersModelBuilder
    {
        public static void BuildFloatingCheckOrdersModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<FloatingCheckOrder>();
            entity.ToTable("floating_check_orders");
        }
    }
}
