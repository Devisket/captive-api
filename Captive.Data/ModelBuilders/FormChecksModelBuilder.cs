using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Data.ModelBuilders
{
    public static class FormChecksModelBuilder
    {
        public static void BuildFormChecksTable(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<FormChecks>();


            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.CheckType)
                .WithMany(x => x.FormChecks)
                .HasForeignKey(x => x.CheckTypeId);

            entity.HasOne(x => x.FormType)
                .WithMany(x => x.FormChecks)
                .HasForeignKey(x => x.FormTypeId);

            entity
                .Property(x => x.Description)
                .IsRequired(false);

            entity.ToTable("form_checks");

        }
    }
}
