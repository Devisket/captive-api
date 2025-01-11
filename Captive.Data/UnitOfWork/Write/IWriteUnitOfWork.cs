using Captive.Data.Models;
using Captive.Data.Repository.Write;

namespace Captive.Data.UnitOfWork.Write
{
    public interface IWriteUnitOfWork : IDisposable
    {
        public IWriteRepository<BankInfo> BankInfo { get; }
        public IWriteRepository<BankBranches> BankBranches{ get; }
        public IWriteRepository<CheckInventory> CheckInventory { get; }
        public IWriteRepository<CheckInventoryDetail> CheckInventoryDetails { get; }
        public IWriteRepository<CheckOrders> CheckOrders { get; }
        public IWriteRepository<FormChecks> FormChecks { get; }
        public IWriteRepository<FloatingCheckOrder> FloatingCheckOrders { get; }
        public IWriteRepository<OrderFileLog> OrderFileLogs { get; }
        public IWriteRepository<OrderFile> OrderFiles { get; }
        public IWriteRepository<BatchFile> BatchFiles { get; }
        public IWriteRepository<Product> ProductTypes { get; }
        public IWriteRepository<ProductConfiguration> ProductConfigurations { get; }
        public IWriteRepository<Tag> Tags{ get; }
        public IWriteRepository<TagMapping> TagMappings { get; }
        public IWriteRepository<CheckValidation> CheckValidations { get; }

        Task Complete(CancellationToken cancellationToken = default);
    }
}
