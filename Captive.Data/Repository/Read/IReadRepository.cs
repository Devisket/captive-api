using System.Linq.Expressions;
namespace Captive.Data.Repository.Read
{
    public interface IReadRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        IQueryable<T> Find(Expression<Func<T, bool>> expression);
    }
}
