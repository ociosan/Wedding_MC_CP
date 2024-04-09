using System.Linq.Expressions;

namespace Core.Interfaces.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        void Update(T entity);
        void Delete(T entity);
        Task CreateAsync(T entity);
        Task<T> FindOneAsync(Expression<Func<T, bool>> expression);
        Task<IList<T>> FindAllAsync(Expression<Func<T, bool>>? expression = null, List<string>? includes = null);
        Task<bool> IsExistsAsync(Expression<Func<T, bool>> expression);
    }
}
