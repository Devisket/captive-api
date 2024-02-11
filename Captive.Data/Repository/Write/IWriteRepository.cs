
namespace Captive.Data.Repository.Write
{
    public interface IWriteRepository<in T> where T : class
    {

        void UpdateRange(IEnumerable<T> range);
        void Update(T entity);

        void Delete(T entity);

        Task Add(T entity);
    }
}
