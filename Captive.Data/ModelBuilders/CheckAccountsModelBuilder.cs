using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static class CheckAccountsModelBuilder
    {
        public static void BuildCheckAccountsModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<CheckAccounts>();

            entity.Property(x => x.AccountNo).IsRequired();
            
            entity.HasIndex(x => x.AccountNo);

            entity.ToTable("check_accounts");
        }
    }
}
