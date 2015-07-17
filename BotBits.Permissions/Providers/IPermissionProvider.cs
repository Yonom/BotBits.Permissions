using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BotBits.Permissions
{
    public interface IPermissionProvider
    {
        void SetDataAsync(string storageName, PermissionData permissionData);
        void GetDataAsync(string storageName, Action<PermissionData> callback);
    }
}
