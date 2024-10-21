using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static class FormChecksModelBuilder
    {
        public static void BuildFormChecksTable(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<FormChecks>();

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new { x.FormType, x.CheckType,});

            entity.Property(x => x.CheckType).IsRequired();

            entity.Property(x => x.FormType).IsRequired();

            entity.Property(x => x.Quantity).IsRequired();

            entity.Property(x => x.FileInitial).IsRequired();

            entity
                .Property(x => x.Description)
                .IsRequired(false);
                        
            entity.ToTable("form_checks");

        }
    }
}
