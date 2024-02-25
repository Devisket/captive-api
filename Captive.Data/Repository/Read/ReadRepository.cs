using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Captive.Data.Repository.Read
{
    public class ReadRepository<T> : IReadRepository<T> where T : class
    {

        private readonly CaptiveDataContext _dbContext;

        public ReadRepository(CaptiveDataContext context)
        {
            _dbContext = context;
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> expression) => _dbContext.Set<T>().Where(expression);

        public IQueryable<T> GetAll() => _dbContext.Set<T>().AsNoTracking();

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default) => await _dbContext.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
    }
}
