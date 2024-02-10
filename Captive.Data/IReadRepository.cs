using System.Linq.Expressions;
namespace Captive.Data
{
    public interface IReadRepository<T> where T : class
    {
       T GetById(in Guid id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T,bool>> expression);
    }
}
