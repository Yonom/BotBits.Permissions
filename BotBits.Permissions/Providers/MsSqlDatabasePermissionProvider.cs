using System.Data.Common;
using System.Data.SqlClient;

namespace BotBits.Permissions
{
    public class MsSqlDatabasePermissionProvider : SqlDatabasePermissionProvider<SqlConnection, SqlDataAdapter>
    {
        public MsSqlDatabasePermissionProvider(string connectionString, string tableName)
            : base(connectionString, tableName)
        {
        }

        public override SqlConnection GetConnection(string connectionString)
        {
            return new SqlConnection(this.ConnectionString);
        }

        public override SqlDataAdapter GetAdapter(string selectCommandText, SqlConnection connection)
        {
            return new SqlDataAdapter(selectCommandText, connection);
        }

        public override DbCommandBuilder GetCommandBuilder(SqlDataAdapter adapter)
        {
            return new SqlCommandBuilder(adapter);
        }
    }
}