
namespace Captive.Data.Repository.Write
{
    public class WriteRepository<T> : IWriteRepository<T> where T : class
    {
        private readonly CaptiveDataContext _context;

        public WriteRepository(CaptiveDataContext context)
        {
            _context = context;
        }

        public async Task Add(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void UpdateRange(IEnumerable<T> range)
        {
            _context.Set<T>().UpdateRange(range);
        }
    }
}