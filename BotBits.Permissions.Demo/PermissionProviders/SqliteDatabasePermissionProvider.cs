using System.Data.Common;
using System.Data.SQLite;

namespace BotBits.Permissions.Demo.PermissionProviders
{
    public class SQLiteDatabasePermissionProvider : SqlDatabasePermissionProvider<SQLiteConnection, SQLiteDataAdapter>
    {
        public SQLiteDatabasePermissionProvider(string connectionString, string tableName)
            : base(connectionString, tableName)
        {
        }

        public override SQLiteConnection GetConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }

        public override SQLiteDataAdapter GetAdapter(string selectCommandText, SQLiteConnection connection)
        {
            return new SQLiteDataAdapter(selectCommandText, connection);
        }

        public override DbCommandBuilder GetCommandBuilder(SQLiteDataAdapter adapter)
        {
            return new SQLiteCommandBuilder(adapter);
        }
    }
}