using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Data.ModelBuilders
{
    public static class AccountAddressModelbuilder
    {
        public static void BuildAccountAddressModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<AccountAddresses>();

            entity.HasKey(x => x.Id);

            entity.Property(x=> x.Address1).IsRequired();

            entity.Property(x=> x.Address2).IsRequired(false);

            entity.Property(x=> x.Address3).IsRequired(false);

            entity.ToTable("account_addresses");
        }
    }
}
