using Captive.Data.Models;
using Captive.Data.Repository.Write;

namespace Captive.Data.UnitOfWork.Write
{
    public interface IWriteUnitOfWork:IDisposable
    {
        public IWriteRepository<OrderFileConfiguration> OrderFileConfiguration { get; }
        public IWriteRepository<OrderFiles> OrderFiles { get; }
        public IWriteRepository<CheckOrders> CheckOrders { get; }
        public IWriteRepository<FormChecks> FormChecks { get; }
        public IWriteRepository<CheckInventory> CheckInventory{ get; }

        Task Complete();
    }
}
