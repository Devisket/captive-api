﻿using Captive.Data.Models;
using Captive.Data.Repository.Read;

namespace Captive.Data.UnitOfWork.Read
{
    public interface IReadUnitOfWork : IDisposable
    {
        public IReadRepository<BankInfo> Banks { get; }
        public IReadRepository<BankBranches> BankBranches { get; }
        public IReadRepository<CheckInventory> CheckInventory { get; }
        public IReadRepository<CheckOrders> CheckOrders { get; }
        public IReadRepository<FormChecks> FormChecks { get; }
        public IReadRepository<OrderFile> OrderFiles { get; }
        public IReadRepository<OrderFileConfiguration> OrderFileConfigurations { get; }
        public IReadRepository<OrderFileLog> OrderFileLogs { get; }

        public IReadRepository<BatchFile> BatchFiles { get; }
        public IReadRepository<Product> ProductTypes { get; }
        public IReadRepository<ProductConfiguration> ProductConfigurations{ get; }

        
    }
}
