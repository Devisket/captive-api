
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
            
            entity.Property(x=> x.BranchAddress1).IsRequired(false);
            entity.Property(x => x.BranchAddress2).IsRequired(false);
            entity.Property(x => x.BranchAddress3).IsRequired(false);
            entity.Property(x => x.BranchAddress4).IsRequired(false);
            entity.Property(x => x.BranchAddress5).IsRequired(false);

            entity.Property(x=> x.BRSTNCode).IsRequired();

            entity.HasIndex(x => x.BRSTNCode);

            entity.ToTable("bank_branchs");

        }
    }
}
