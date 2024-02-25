using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;


namespace Captive.Data.ModelBuilders
{
    public  static class OrderFileLogModelBuilder
    {
        public static void BuildOrderFileLogModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<OrderFileLog>();

            entity.HasKey(x => x.Id);

            entity.Property(x => x.LogType).HasConversion<string>();

            entity.Property(x => x.LogDate).IsRequired();

            entity.Property(x => x.LogMessage).IsRequired();

            entity.HasOne(x => x.OrderFile)
                .WithMany(x => x.OrderFileLogs)
                .HasForeignKey(x => x.OrderFileId);

            entity.ToTable("order_file_logs");
        }

    }
}
