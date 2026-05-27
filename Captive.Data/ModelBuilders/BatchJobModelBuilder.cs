using Captive.Data.Enums;
using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static class BatchJobModelBuilder
    {
        public static void BuildBatchJobModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<BatchJob>();

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Status).IsRequired().HasConversion(
                v => v.ToString(),
                v => (BatchJobStatus)Enum.Parse(typeof(BatchJobStatus), v));

            entity.Property(x => x.Progress).IsRequired().HasDefaultValue(0);
            entity.Property(x => x.CurrentStep).IsRequired(false).HasDefaultValue(null);
            entity.Property(x => x.Warnings).IsRequired(false).HasDefaultValue(null);
            entity.Property(x => x.ErrorMessage).IsRequired(false).HasDefaultValue(null);
            entity.Property(x => x.ForceProcess).IsRequired().HasDefaultValue(false);
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.UpdatedAt).IsRequired();

            entity.HasOne(x => x.BatchFile)
                .WithMany()
                .HasForeignKey(x => x.BatchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable("batch_jobs");
        }
    }
}
