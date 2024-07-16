using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
  public static class CheckInventoryDetailModelBuilder 
  {
    public static void BuildCheckInventoryDetailModel(this ModelBuilder modelBuilder) 
    {
      var entity = modelBuilder.Entity<CheckInventoryDetail>();
      
      entity.Property(x => x.Quantity).IsRequired(true);

      entity.ToTable("check_inventory_detail");

    }
  }

}
