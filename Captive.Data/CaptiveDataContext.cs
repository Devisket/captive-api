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
            modelBuilder.BuildBankBranchModel();
            modelBuilder.BuildBankInfoModel();
            modelBuilder.BuildOrderFilesModel();
            modelBuilder.BuildCheckOrdersModel();
            modelBuilder.BuildFileConfiguration();
            modelBuilder.BuildFormChecksTable();
            modelBuilder.OnBuildSeedTable();
            modelBuilder.BuildCheckInventoryTable();
            modelBuilder.BuildOrderFileLogModel();
            modelBuilder.BuildBatchFileModel();
            modelBuilder.BuildProductConfigurationModel();
            modelBuilder.BuildProductModel();
            modelBuilder.BuildCheckInventoryDetailModel();
            modelBuilder.BuildTagModel();
            modelBuilder.BuildTagMappingModel();    
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
