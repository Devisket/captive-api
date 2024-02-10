using System.Linq.Expressions;
namespace Captive.Data
{
    public class ReadRepository<T> : IReadRepository<T> where T : class
    {

        private readonly CaptiveDataContext _dbContext;

        public ReadRepository(CaptiveDataContext context) 
        {
            _dbContext = context;
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> expression) => _dbContext.Set<T>().Where(expression);

        public IEnumerable<T> GetAll() => _dbContext.Set<T>().ToList();

        public T GetById(in Guid id) => _dbContext.Set<T>().Find(id);
    }
}
