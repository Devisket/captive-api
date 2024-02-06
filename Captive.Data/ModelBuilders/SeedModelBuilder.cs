using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Data.ModelBuilders
{
    public static class SeedModelBuilder
    {
        public static void OnBuildSeedTable(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Seeds>();

            entity.HasKey(x => x.Id);
            entity.Property(x => x.SeedName).IsRequired();
            entity.Property(x => x.SeedDate).IsRequired();

            entity.ToTable("seed");
        }
    }
}
