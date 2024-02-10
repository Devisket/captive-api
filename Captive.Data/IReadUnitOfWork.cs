using Captive.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Data
{
    public interface IReadUnitOfWork:IDisposable
    {
        public IReadRepository<OrderFileConfiguration> OrderFileConfigurations { get; }
        public IReadRepository<OrderFiles> OrderFiles { get; }
        public IReadRepository<BankInfo> Banks { get; }
        public IReadRepository<BankBranches> BankBranches { get; }
        public IReadRepository<CheckOrders> CheckOrders { get; }
        public IReadRepository<CheckTypes> CheckTypes { get; }
    }
}
