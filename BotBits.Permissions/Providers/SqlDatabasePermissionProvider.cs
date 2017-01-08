using System;
using System.Data.Common;

namespace BotBits.Permissions
{
    public abstract class SqlDatabasePermissionProvider<TDbConnection, TDataAdapter> : DatabasePermissionProvider, IDisposable
        where TDbConnection : DbConnection where TDataAdapter : DataAdapter
    {
        private readonly TDataAdapter _adapter;
        private readonly TDbConnection _conn;
        private readonly DbCommandBuilder _commandBuilder;

        protected SqlDatabasePermissionProvider(string connectionString, string tableName)
        {
            this.ConnectionString = connectionString;
            this.TableName = tableName;


            this._conn = this.GetConnection(this.ConnectionString);
            try
            {
                this._conn.Open();
                using (var cmd = this._conn.CreateCommand())
                {
                    cmd.CommandText =
                        "CREATE TABLE IF NOT EXISTS `" + this.TableName + "` ( " +
                        "`" + Username + "` VARCHAR(50), " +
                        "`" + Group + "` INTEGER, " +
                        "`" + BanReason + "` TEXT, " +
                        "`" + BanTimeout + "` BIGINT, " +
                        "PRIMARY KEY(Username) " +
                        ");";
                    cmd.ExecuteNonQuery();
                }

                this._adapter = this.GetAdapter(this.SelectCommandText, this._conn);
                this._commandBuilder = this.GetCommandBuilder(this._adapter);
                this._adapter.Fill(this.DataSet);
            }
            finally
            {
                this._conn.Close();
            }
        }

        public string ConnectionString { get; }
        public string TableName { get; }

        private string SelectCommandText
        {
            get { return "SELECT * FROM " + this.TableName; }
        }

        public void Dispose()
        {
            this._commandBuilder.Dispose();
            this._adapter.Dispose();
            this._conn.Dispose();
        }

        public override void SetDataAsync(string storageName, PermissionData permissionData)
        {
            base.SetDataAsync(storageName, permissionData);
            this._adapter.Update(this.DataSet);
        }

        public abstract TDbConnection GetConnection(string connectionString);
        public abstract TDataAdapter GetAdapter(string selectCommandText, TDbConnection connection);
        public abstract DbCommandBuilder GetCommandBuilder(TDataAdapter adapter);
    }
}