using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static class FormTypeModelBuilder
    {
        public static void BuildFormTypeTable(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<FormTypes>();

            entity.HasKey(x => x.Id);

            entity.Property(x => x.FormType).IsRequired();

            entity.Property(x => x.Description).IsRequired(false);

            entity.HasOne(x => x.Bank)
                .WithMany(x => x.FormTypes).HasForeignKey(x => x.BankId);

            entity.ToTable("form_types");
        }
    }
}
