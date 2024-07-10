using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static class BatchFileModelBuilder
    {
        public static void BuildBatchFileModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<BatchFile>();

            entity.HasKey(x => x.Id);
            entity.Property(x => x.UploadDate).IsRequired();

            entity.Property(x => x.BatchFileStatus).IsRequired();

            entity.Property(x => x.BatchName)
                .IsRequired();

            entity.HasMany(x => x.OrderFiles)
                .WithOne(x => x.BatchFile)
                .HasForeignKey(x => x.BatchFileId);

            entity.ToTable("batch_files");
        }
    }
}
