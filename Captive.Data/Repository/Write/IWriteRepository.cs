
namespace Captive.Data.Repository.Write
{
    public interface IWriteRepository<T> where T : class
    {
        IQueryable<T> GetAll();

        void UpdateRange(IEnumerable<T> range);
        void Update(T entity);

        void Delete(T entity);

        Task AddAsync(T entity, CancellationToken cancellationToken);

        Task AddRange(T[] entity, CancellationToken cancellationToken);
    }
}
