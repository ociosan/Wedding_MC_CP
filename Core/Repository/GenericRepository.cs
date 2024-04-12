using Core.Interfaces.Repository;
using Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Core.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly WeddingDBContext _weddingDBContext;
        private readonly DbSet<T> _db;

        public GenericRepository(WeddingDBContext weddingDBContext)
        {
            _weddingDBContext = weddingDBContext;
            _db = weddingDBContext.Set<T>();
        }

        public async Task CreateAsync(T entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task<T> FindOneAsync(Expression<Func<T, bool>> expression, List<string>? includes = null)
        {
            IQueryable<T> query = _db;

            if (includes != null)
            {
                foreach (var table in includes)
                    query = query.Include(table);
            }

            return await query.FirstAsync(expression);
        }

        public async Task<IList<T>> FindAllAsync(Expression<Func<T, bool>>? expression = null, List<string>? includes = null)
        {
            IQueryable<T> query = _db;
            if (expression != null)
                query = query.Where(expression);

            if (includes != null)
            {
                foreach (var table in includes)
                    query = query.Include(table);
            }

            return await query.ToListAsync();
        }

        public async Task<bool> IsExistsAsync(Expression<Func<T, bool>> expression)
        {
            IQueryable<T> query = _db;
            return await query.AnyAsync(expression);
        }

        #region Synchronous

        public void Delete(T entity)
        {
            _db.Remove(entity);
            Save();
        }

        public void Update(T entity)
        {
            _db.Update(entity);
            Save();
        }


        private void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool dispose)
        {
            if (dispose)
                _weddingDBContext.Dispose();
        }

        private void Save()
        {
            _weddingDBContext.SaveChanges();
        }

        #endregion
    }
}
