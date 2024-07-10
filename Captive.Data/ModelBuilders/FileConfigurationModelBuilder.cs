using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Data.ModelBuilders
{
    public static class FileConfigurationModelBuilder
    {
        public static void BuildFileConfiguration(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<OrderFileConfiguration>();
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).IsRequired();

            entity.HasIndex(x => x.Name);

            entity.Property(x => x.ConfigurationData).IsRequired();

            entity.Property(x => x.ConfigurationType).IsRequired().HasConversion<string>();

            entity.ToTable("order_file_configuration");
        }
    }
}
