using Core.Interfaces.Helper;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Core.Helpers
{
    public class DapperDbHelper : IDapperDbHelper
    {
        private SqlConnection connection;
        private DbTransaction transaction;

        public SqlConnection Connection
        {
            get
            {
                if (connection == null) { connection = new SqlConnection(Environment.GetEnvironmentVariable("DbConnectionString")); }
                return connection;
            }
        }

        public DbTransaction Transaction
        {
            get
            {
                if (transaction == null) { transaction = Connection.BeginTransaction(IsolationLevel.ReadCommitted); }
                return transaction;
            }
        }

        public void CloseConnection()
        {
            if(connection != null)
            {
                connection.Close();
                connection.Dispose();
                connection = null;
            }
        }
    }

    public class DataParameter
    {
        public DbType DbType { get; set; }
        public object Value { get; set; }
        public string Name { get; set; }
    }
}
