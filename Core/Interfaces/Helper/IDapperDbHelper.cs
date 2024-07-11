using System.Data.Common;
using System.Data.SqlClient;

namespace Core.Interfaces.Helper
{
    public interface IDapperDbHelper
    {
        public SqlConnection Connection { get; }
        DbTransaction Transaction { get; }
        void CloseConnection();
    }
}
