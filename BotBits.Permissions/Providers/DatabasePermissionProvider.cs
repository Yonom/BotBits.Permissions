using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace BotBits.Permissions
{
    public class MsSqlDatabasePermissionProvider : SqlDatabasePermissionProvider<SqlConnection, SqlDataAdapter>
    {
        public MsSqlDatabasePermissionProvider(string connectionString) 
            : base(connectionString)
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

    public abstract class SqlDatabasePermissionProvider<TDbConnection, TDataAdapter> : DatabasePermissionProvider 
        where TDbConnection : DbConnection where TDataAdapter : DataAdapter
    {
        public string ConnectionString { get; private set; }
        private const string SelectCommandText = "SELECT * FROM BotBitsUsers";

        protected SqlDatabasePermissionProvider(string connectionString)
        {
            this.ConnectionString = connectionString;
            using (var conn = this.GetConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        "CREATE TABLE IF NOT EXISTS `BotBitsUsers` ( " +
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

            using (var conn = this.GetConnection(ConnectionString))
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

    public abstract class DatabasePermissionProvider : IPermissionProvider
    {
        protected const string Username = "Username";
        protected const string Group = "Group";
        protected const string BanReason = "BanReason";
        protected const string BanTimeout = "BanTimeout";
        protected DataSet DataSet { get; private set; }
        private DataTable DataTable { get { return this.DataSet.Tables["Table"]; } }

        protected DatabasePermissionProvider()
        {
            this.DataSet = new DataSet();
            this.DataSet.Tables.Add(new DataTable("Table"));
            this.DataTable.Columns.Add(new DataColumn(Username, typeof(string)));
            this.DataTable.Columns.Add(new DataColumn(Group, typeof(int)));
            this.DataTable.Columns.Add(new DataColumn(BanReason, typeof(string)));
            this.DataTable.Columns.Add(new DataColumn(BanTimeout, typeof(long)));
            this.DataTable.PrimaryKey = new[] {this.DataTable.Columns[Username]};
        }

        private void AddOrUpdate(string username, Action<DataRow> callback)
        {
            var row = this.DataTable.Rows.Find(username);
            if (row == null)
            {
                row = this.DataTable.NewRow();
                callback(row);
                this.DataTable.Rows.Add(row);
            }
            else
            {
                callback(row);
            }
        }

        public virtual void SetDataAsync(string storageName, PermissionData permissionData)
        {
            AddOrUpdate(storageName, row =>
            {
                row[Username] = storageName;
                row[Group] = permissionData.Group;
                row[BanReason] = permissionData.BanReason;
                row[BanTimeout] = permissionData.BanTimeout.Ticks;
            });
        }

        public virtual void GetDataAsync(string storageName, Action<PermissionData> callback)
        {
            var row = this.DataTable.Rows.Find(storageName);
            callback(row != null
                ? new PermissionData(
                    (Group)row.GetValue<int>(Group), 
                    row.GetValue<string>(BanReason),
                    new DateTime(row.GetValue<long>(BanTimeout)))
                : new PermissionData());
        }
    }
}
