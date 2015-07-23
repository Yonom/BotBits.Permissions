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
            using (var conn = this.GetConnection())
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

        public override void GetDataAsync(string storageName, Action<PermissionData> callback)
        {
            base.GetDataAsync(storageName, callback);
        }

        public override void SetDataAsync(string storageName, PermissionData permissionData)
        {
            base.SetDataAsync(storageName, permissionData);

            using (var conn = this.GetConnection())
            using (var adapter = this.GetAdapter(conn))
            using (new SQLiteCommandBuilder(adapter))
            {
                adapter.Update(this.DataSet);
            }
        }
        
        private SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(this._connectionString);
        }

        private SQLiteDataAdapter GetAdapter(SQLiteConnection connection)
        {
            return new SQLiteDataAdapter("SELECT * FROM BotBitsUsers", connection);
        }
    }
}