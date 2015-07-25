using System.Data.Common;

namespace BotBits.Permissions
{
    public abstract class SqlDatabasePermissionProvider<TDbConnection, TDataAdapter> : DatabasePermissionProvider 
        where TDbConnection : DbConnection where TDataAdapter : DataAdapter
    {
        public string ConnectionString { get; private set; }
        public string TableName { get; private set; }

        private string SelectCommandText
        {
            get { return "SELECT * FROM " + TableName; }
        }

        protected SqlDatabasePermissionProvider(string connectionString, string tableName)
        {
            this.ConnectionString = connectionString;
            this.TableName = tableName;
            using (var conn = this.GetConnection(this.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        "CREATE TABLE IF NOT EXISTS `" + this.TableName +"` ( " +
                        "`" + Username + "` VARCHAR(50), " +
                        "`" + Group + "` INTEGER, " +
                        "`" + BanReason + "` TEXT, " +
                        "`" + BanTimeout + "` INTEGER, " +
                        "PRIMARY KEY(Username) " +
                        ");";
                    cmd.ExecuteNonQuery();
                }

                using (var adapter = this.GetAdapter(SelectCommandText, conn))
                    adapter.Fill(this.DataSet);
            }
        }

        public override void SetDataAsync(string storageName, PermissionData permissionData)
        {
            base.SetDataAsync(storageName, permissionData);

            using (var conn = this.GetConnection(this.ConnectionString))
            using (var adapter = this.GetAdapter(SelectCommandText, conn))
            using (this.GetCommandBuilder(adapter))
            {
                adapter.Update(this.DataSet);
            }
        }

        public abstract TDbConnection GetConnection(string connectionString);
        public abstract TDataAdapter GetAdapter(string selectCommandText, TDbConnection connection);
        public abstract DbCommandBuilder GetCommandBuilder(TDataAdapter adapter);
    }
}