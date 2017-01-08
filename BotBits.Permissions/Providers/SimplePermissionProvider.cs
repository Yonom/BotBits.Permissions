using System;
using System.Collections.Generic;
using System.Linq;

namespace BotBits.Permissions
{
    public class SimplePermissionProvider : IPermissionProvider
    {
        public SimplePermissionProvider(params string[] admins)
        {
            this.UserDatas = new Dictionary<string, PermissionData>();
            foreach (var admin in admins.Select(a => a.ToLowerInvariant()))
            {
                this.UserDatas.Add(admin, new PermissionData(Group.Admin));
            }
        }

        public Dictionary<string, PermissionData> UserDatas { get; }

        public void SetDataAsync(string storageName, PermissionData permissionData)
        {
            this.UserDatas[storageName] = permissionData;
        }

        public void GetDataAsync(string storageName, Action<PermissionData> callback)
        {
            PermissionData value;
            this.UserDatas.TryGetValue(storageName, out value);
            callback(value);
        }
    }
}