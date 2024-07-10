using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static class OrderFilesModelBuilder
    {
        public static void BuildOrderFilesModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<OrderFile>();
            
            entity.Property(x=> x.FileName)
                .IsRequired();

            entity.HasIndex(x=> x.FileName);

            entity.Property(x => x.Status)
                .HasConversion<string>(); 

            entity.Property(x => x.ProcessDate)
                .IsRequired();

            entity.HasMany(x => x.CheckOrders)
                .WithOne(x => x.OrderFile)
                .HasForeignKey(x => x.OrderFileId);

            entity.HasMany(x => x.OrderFileLogs)
                .WithOne(x => x.OrderFile)
                .HasForeignKey(x => x.OrderFileId);

            entity.ToTable("order_files");
        }
    }
}
