﻿using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using PlayerIOClient;

namespace BotBits.Permissions
{
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

        private DataRow AddOrUpdate(string username)
        {
            var row = this.DataTable.Rows.Find(username);
            if (row == null)
            {
                row = this.DataTable.NewRow();
                this.DataTable.Rows.Add(row);
            }
            return row;
        }

        public virtual void SetDataAsync(string storageName, PermissionData permissionData)
        {
            DataRow row = AddOrUpdate(storageName);
            row[Username] = storageName;
            row[Group] = permissionData.Group;
            row[BanReason] = permissionData.BanReason;
            row[BanTimeout] = permissionData.BanTimeout.Ticks;
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