using Captive.Data.ModelBuilders;
using Microsoft.EntityFrameworkCore;

namespace Captive.Data
{
    public class CaptiveDataContext:DbContext
    {
        public CaptiveDataContext(DbContextOptions options):base(options) { 
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.BuildAccountAddressModel();
            modelBuilder.BuildAccountInfoModel();
            modelBuilder.BuildBankBranchModel();
            modelBuilder.BuildBankInfoModel();
            modelBuilder.BuildOrderFilesModel();
            modelBuilder.BuildCheckAccountsModel();
            modelBuilder.BuildCheckOrdersModel();

            base.OnModelCreating(modelBuilder);
        }

    }
}
