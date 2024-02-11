using Captive.Data.Models;
using Captive.Data.Repository.Read;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Data.UnitOfWork.Read
{
    public interface IReadUnitOfWork : IDisposable
    {
        public IReadRepository<OrderFileConfiguration> OrderFileConfigurations { get; }
        public IReadRepository<OrderFiles> OrderFiles { get; }
        public IReadRepository<BankInfo> Banks { get; }
        public IReadRepository<BankBranches> BankBranches { get; }
        public IReadRepository<CheckOrders> CheckOrders { get; }
        public IReadRepository<FormChecks> FormChecks { get; }

        public IReadRepository<CheckInventory> CheckInventory { get; }
    }
}
