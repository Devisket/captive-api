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

        public IWriteRepository<OrderFileConfiguration> OrderFileConfiguration => GetStandardRepository<OrderFileConfiguration>();

        public IWriteRepository<OrderFiles> OrderFiles => GetStandardRepository<OrderFiles>();

        public IWriteRepository<CheckOrders> CheckOrders => GetStandardRepository<CheckOrders>();

        public IWriteRepository<FormChecks> FormChecks => GetStandardRepository<FormChecks>();

        public IWriteRepository<CheckInventory> CheckInventory => GetStandardRepository<CheckInventory>();

        public IWriteRepository<T> GetStandardRepository<T>() where T : class => new WriteRepository<T>(_dbContext);

        public async Task Complete()
        {
            await _dbContext.SaveChangesAsync();
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
