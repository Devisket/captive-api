using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static class CheckValidationModelBuilder
    {
        public static void BuildCheckValidationModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<CheckValidation>();

            entity.Property(x => x.Name).IsRequired();

            entity.HasMany(x => x.Tags).WithOne(x => x.CheckValidation)
                .HasForeignKey(x => x.CheckValidationId);

            entity.HasMany(x => x.CheckInventory)
                .WithOne(x => x.CheckValidation).HasForeignKey(x => x.CheckValidationId);

            entity.ToTable("check_validation");
        }
    }
}
