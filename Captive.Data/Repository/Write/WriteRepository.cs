
namespace Captive.Data.Repository.Write
{
    public class WriteRepository<T> : IWriteRepository<T> where T : class
    {
        private readonly CaptiveDataContext _context;

        public WriteRepository(CaptiveDataContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            await _context.Set<T>().AddAsync(entity, cancellationToken);
        }
        public async Task AddRange(T[] entity, CancellationToken cancellationToken)
        {
            await _context.Set<T>().AddRangeAsync(entity, cancellationToken);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void Update(T[] entity)
        {
            _context.Set<T>().UpdateRange(entity);
        }

        public void UpdateRange(IEnumerable<T> range)
        {
            _context.Set<T>().UpdateRange(range);
        }
    }
}