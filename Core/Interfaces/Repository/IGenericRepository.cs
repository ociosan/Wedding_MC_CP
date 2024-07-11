using static Dapper.SqlMapper;

namespace Core.Interfaces.Repository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task InsertInto(string sqlInsertStatement, TEntity entity);
        Task<int> Update(string sqlUpdateStatement);
        Task<TEntity> SelectOneRow(string sqlSelectStatement);
        Task<IEnumerable<TEntity>> GetList(string sqlSelectStatement);
    }
}
