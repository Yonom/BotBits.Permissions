using System;
using System.Data.Common;
using System.Data.SQLite;

namespace BotBits.Permissions.Demo.PermissionProviders
{
    public class SQLiteDatabasePermissionProvider : SqlDatabasePermissionProvider<SQLiteConnection, SQLiteDataAdapter>
    {
        private readonly string _connectionString;

        public SQLiteDatabasePermissionProvider(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public override SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(this._connectionString);
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