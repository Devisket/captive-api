using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static class CheckTypeModelBuilder
    {
        public static void BuildCheckTypeTable(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<CheckTypes>();

            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Bank)
                .WithMany(x => x.CheckTypes)
                .HasForeignKey(x => x.BankId);

            entity.Property(entity => entity.Name).IsRequired();

            entity.Property(entity => entity.Description).IsRequired(false);

            entity.ToTable("check_types");
        }
    }
}
