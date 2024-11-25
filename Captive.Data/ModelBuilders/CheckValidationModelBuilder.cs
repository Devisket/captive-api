using Captive.Data.Enums;
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

            entity.Property(x => x.ValidationType).HasConversion(
                    v => v.ToString(),
                    v => (ValidationType)Enum.Parse(typeof(ValidationType), v.ToString())
                );

            entity.HasMany(x => x.Tags).WithOne(x => x.CheckValidation)
                .HasForeignKey(x => x.CheckValidationId);

            entity.HasOne(x => x.CheckInventory)
                .WithOne(x => x.CheckValidation)
                .HasForeignKey<CheckInventory>(x => x.CheckValidationId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            entity.ToTable("check_validation");
        }
    }
}
