using Captive.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captive.Data
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

        IReadRepository<CheckTypes> IReadUnitOfWork.CheckTypes => GetStandardRepository<CheckTypes>();

        public IReadRepository<T> GetStandardRepository<T>() where T : class => new ReadRepository<T>(_dbContext);

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
