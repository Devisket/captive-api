using Captive.Data.Models;
using Captive.Data.Repository.Read;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Data.UnitOfWork.Read
{
    public class ReadUnitOfWork : IReadUnitOfWork
    {
        private readonly CaptiveDataContext _dbContext;

        public ReadUnitOfWork(CaptiveDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        IReadRepository<OrderFileConfiguration> IReadUnitOfWork.OrderFileConfigurations => GetStandardRepository<OrderFileConfiguration>();

        IReadRepository<OrderFiles> IReadUnitOfWork.OrderFiles => GetStandardRepository<OrderFiles>();

        IReadRepository<BankInfo> IReadUnitOfWork.Banks => GetStandardRepository<BankInfo>();

        IReadRepository<BankBranches> IReadUnitOfWork.BankBranches => GetStandardRepository<BankBranches>();

        IReadRepository<CheckOrders> IReadUnitOfWork.CheckOrders => GetStandardRepository<CheckOrders>();

        IReadRepository<FormChecks> IReadUnitOfWork.FormChecks => GetStandardRepository<FormChecks>();
        IReadRepository<CheckInventory> IReadUnitOfWork.CheckInventory=> GetStandardRepository<CheckInventory>();

        public IReadRepository<T> GetStandardRepository<T>() where T : class => new ReadRepository<T>(_dbContext);

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
