using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static class OrderFilesModelBuilder
    {
        public static void BuildOrderFilesModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<OrderFiles>();
            
            entity.Property(x=> x.BatchName)
                .IsRequired();

            entity.HasIndex(x=> x.BatchName);

            entity.Property(x => x.ProcessDate) .IsRequired();

            entity.ToTable("order_files");
        }
    }
}
