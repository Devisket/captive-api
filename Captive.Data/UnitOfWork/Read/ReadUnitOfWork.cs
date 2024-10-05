using Captive.Data.Models;
using Captive.Data.Repository.Read;


namespace Captive.Data.UnitOfWork.Read
{
    public class ReadUnitOfWork : IReadUnitOfWork
    {
        private readonly CaptiveDataContext _dbContext;

        public ReadUnitOfWork(CaptiveDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        IReadRepository<BankInfo> IReadUnitOfWork.Banks => GetStandardRepository<BankInfo>();
        IReadRepository<BankBranches> IReadUnitOfWork.BankBranches => GetStandardRepository<BankBranches>();
        IReadRepository<CheckOrders> IReadUnitOfWork.CheckOrders => GetStandardRepository<CheckOrders>();
        IReadRepository<CheckInventory> IReadUnitOfWork.CheckInventory=> GetStandardRepository<CheckInventory>();
        IReadRepository<FormChecks> IReadUnitOfWork.FormChecks => GetStandardRepository<FormChecks>();
        IReadRepository<OrderFile> IReadUnitOfWork.OrderFiles => GetStandardRepository<OrderFile>();
        IReadRepository<OrderFileLog> IReadUnitOfWork.OrderFileLogs => GetStandardRepository<OrderFileLog>();
        IReadRepository<BatchFile> IReadUnitOfWork.BatchFiles => GetStandardRepository<BatchFile>();
        IReadRepository<Product> IReadUnitOfWork.ProductTypes => GetStandardRepository<Product>();
        IReadRepository<ProductConfiguration> IReadUnitOfWork.ProductConfigurations => GetStandardRepository<ProductConfiguration>();
        IReadRepository<Tag> IReadUnitOfWork.Tags => GetStandardRepository<Tag>();
        IReadRepository<TagMapping> IReadUnitOfWork.TagsMapping => GetStandardRepository<TagMapping>();

        public IReadRepository<T> GetStandardRepository<T>() where T : class => new ReadRepository<T>(_dbContext);

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
