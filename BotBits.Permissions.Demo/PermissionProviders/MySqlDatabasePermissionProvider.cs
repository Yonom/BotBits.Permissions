using System;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace BotBits.Permissions.Demo.PermissionProviders
{
    public class MySqlDatabasePermissionProvider : SqlDatabasePermissionProvider<MySqlConnection, MySqlDataAdapter>
    {
        public MySqlDatabasePermissionProvider(string connectionString)
            : base(connectionString)
        {
        }

        public override MySqlConnection GetConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
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