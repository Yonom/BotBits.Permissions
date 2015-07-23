using System;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace BotBits.Permissions.Demo.PermissionProviders
{
    public class MySqlDatabasePermissionProvider : SqlDatabasePermissionProvider<MySqlConnection, MySqlDataAdapter>
    {
        private readonly string _connectionString;

        public MySqlDatabasePermissionProvider(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public override MySqlConnection GetConnection()
        {
            return new MySqlConnection(this._connectionString);
        }

        public override MySqlDataAdapter GetAdapter(string selectCommandText, MySqlConnection connection)
        {
            return new MySqlDataAdapter(selectCommandText, connection);
        }

        public override DbCommandBuilder GetCommandBuilder(MySqlDataAdapter adapter)
        {
            return new MySqlCommandBuilder(adapter);
        }
    }
}