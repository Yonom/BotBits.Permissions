using System;
using MySql.Data.MySqlClient;

namespace BotBits.Permissions.Demo.PermissionProviders
{
    public class MySqlDatabasePermissionProvider : DatabasePermissionProvider
    {
        private readonly string _connectionString;

        public MySqlDatabasePermissionProvider(string connectionString)
        {
            this._connectionString = connectionString;
            using (var conn = new MySqlConnection(this._connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(
                    "CREATE TABLE IF NOT EXISTS `BotBitsUsers` ( " +
                    "`" + Username + "` VARCHAR(50), " +
                    "`" + Group + "` INTEGER, " +
                    "`" + BanReason + "` TEXT, " +
                    "`" + BanTimeout + "` INTEGER, " +
                    "PRIMARY KEY(Username) " +
                    ");", conn))
                    cmd.ExecuteNonQuery();

                using (var adapter = this.GetAdapter(conn))
                    adapter.Fill(this.DataSet);
            }
        }

        public override void SetDataAsync(string storageName, PermissionData permissionData)
        {
            base.SetDataAsync(storageName, permissionData);

            using (var conn = new MySqlConnection(this._connectionString))
            using (var adapter = this.GetAdapter(conn))
            using (new MySqlCommandBuilder(adapter))
            {
                adapter.Update(this.DataSet);
            }
        }

        private MySqlDataAdapter GetAdapter(MySqlConnection connection)
        {
            return new MySqlDataAdapter("SELECT * FROM BotBitsUsers", connection);
        }
    }
}