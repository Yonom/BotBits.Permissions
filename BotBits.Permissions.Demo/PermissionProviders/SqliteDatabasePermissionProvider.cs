using System;
using System.Data.SQLite;

namespace BotBits.Permissions.Demo.PermissionProviders
{
    public class SQLiteDatabasePermissionProvider : DatabasePermissionProvider
    {
        private readonly string _connectionString;

        public SQLiteDatabasePermissionProvider(string connectionString)
        {
            this._connectionString = connectionString;
            using (var conn = new SQLiteConnection(this._connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS `BotBitsUsers` ( " +
                    "`" + Username +"` VARCHAR(50), " +
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

            using (var conn = new SQLiteConnection(this._connectionString))
            using (var adapter = this.GetAdapter(conn))
            using (new SQLiteCommandBuilder(adapter))
            {
                adapter.Update(this.DataSet);
            }
        }
        
        private SQLiteDataAdapter GetAdapter(SQLiteConnection connection)
        {
            return new SQLiteDataAdapter("SELECT * FROM BotBitsUsers", connection);
        }
    }
}