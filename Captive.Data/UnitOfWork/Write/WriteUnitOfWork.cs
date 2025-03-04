using Captive.Data.Models;
using Captive.Data.Repository.Write;

namespace Captive.Data.UnitOfWork.Write
{
    public class WriteUnitOfWork : IWriteUnitOfWork
    {
        private readonly CaptiveDataContext _dbContext;

        public WriteUnitOfWork(CaptiveDataContext dbContext) 
        {
            _dbContext = dbContext;
        }
        public IWriteRepository<BankInfo> BankInfo => GetStandardRepository<BankInfo>();
        public IWriteRepository<BatchFile> BatchFiles => GetStandardRepository<BatchFile>();
        public IWriteRepository<CheckInventory> CheckInventory => GetStandardRepository<CheckInventory>();
        public IWriteRepository<CheckInventoryDetail> CheckInventoryDetails => GetStandardRepository<CheckInventoryDetail>();
        public IWriteRepository<CheckOrders> CheckOrders => GetStandardRepository<CheckOrders>();
        public IWriteRepository<FormChecks> FormChecks => GetStandardRepository<FormChecks>();
        public IWriteRepository<FloatingCheckOrder> FloatingCheckOrders => GetStandardRepository<FloatingCheckOrder>();
        public IWriteRepository<OrderFile> OrderFiles => GetStandardRepository<OrderFile>();
        public IWriteRepository<OrderFileLog> OrderFileLogs => GetStandardRepository<OrderFileLog>();
        public IWriteRepository<ProductConfiguration> ProductConfigurations => GetStandardRepository<ProductConfiguration>();
        public IWriteRepository<Product> ProductTypes => GetStandardRepository<Product>();
        public IWriteRepository<BankBranches> BankBranches => GetStandardRepository<BankBranches>();
        public IWriteRepository<Tag> Tags => GetStandardRepository<Tag>();
        public IWriteRepository<TagMapping> TagMappings => GetStandardRepository<TagMapping>();
        public IWriteRepository<T> GetStandardRepository<T>() where T : class => new WriteRepository<T>(_dbContext);

        public async Task Complete(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
