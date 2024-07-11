using Core.Interfaces.Helper;
using Core.Interfaces.Repository;
using Dapper;

namespace Core.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class 
    {
        private readonly IDapperDbHelper _dapperDbHelper;

        public GenericRepository(IDapperDbHelper dapperDbHelper)
        {
            _dapperDbHelper = dapperDbHelper;
        }

        public async Task InsertInto(string sqlInsertStatement, TEntity entity)
        {
            try
            {
                using (_dapperDbHelper.Connection)
                {
                    _dapperDbHelper.Connection.Open();
                    await _dapperDbHelper.Connection.ExecuteAsync(sqlInsertStatement, entity);
                }
            }
            finally
            {
                _dapperDbHelper.CloseConnection();
            }
        }

        public async Task<int> Update(string sqlUpdateStatement)
        {
            try
            {
                using (_dapperDbHelper.Connection)
                {
                    _dapperDbHelper.Connection.Open();
                    return await _dapperDbHelper.Connection.ExecuteAsync(sqlUpdateStatement);
                }
            }
            finally
            {
                _dapperDbHelper.CloseConnection();
            }
        }

        public async Task<TEntity> SelectOneRow(string sqlSelectStatement)
        {
            try
            {
                using (_dapperDbHelper.Connection)
                {
                    _dapperDbHelper.Connection.Open();
                    return await _dapperDbHelper.Connection.QueryFirstOrDefaultAsync<TEntity>(sqlSelectStatement);
                }
            }
            finally
            {
                _dapperDbHelper.CloseConnection();
            }
        }

        public async Task<IEnumerable<TEntity>> GetList(string sqlSelectStatement)
        {
            try
            {
                using (_dapperDbHelper.Connection)
                {
                    _dapperDbHelper.Connection.Open();
                    return await _dapperDbHelper.Connection.QueryAsync<TEntity>(sqlSelectStatement);
                }
            }
            finally
            {
                _dapperDbHelper.CloseConnection();
            }
        }
    }
}
