
using Captive.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data.ModelBuilders
{
    public static class BankBranchModelBuilder
    {
        public static void BuildBankBranchModel(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<BankBranches>();

            entity.HasKey(x=> x.Id);

            entity.Property(x=> x.BranchName).IsRequired();
            
            entity.Property(x=> x.BranchAddress).IsRequired(false);

            entity.Property(x=> x.BRSTNCode).IsRequired();

            entity.HasIndex(x => x.BRSTNCode);

            entity.ToTable("bank_branchs");

        }
    }
}
