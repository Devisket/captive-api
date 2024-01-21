using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static class AccountInfoModelBuilder
    {
        public static void BuildAccountInfoModel(this ModelBuilder modelBuilder)
        {
            var entityBuilder = modelBuilder.Entity<AccountInfo>();

            entityBuilder.HasKey(x => x.Id);

            entityBuilder.Property(x=>x.FirstName).IsRequired();

            //One-to-one configuration for CheckAccounts table
            entityBuilder.HasOne(x => x.CheckAccount)
                .WithOne(x => x.AccountInfo)
                .HasForeignKey<AccountInfo>(x => x.CheckAccountId);

            //One-to-one configuration for AccountAddresses table
            entityBuilder.HasOne(x => x.AccountAddress).WithOne(x => x.AccountInfo).HasForeignKey<AccountInfo>(x => x.AccountAddressId);

            entityBuilder.ToTable("account_info");
        }
    }
}
