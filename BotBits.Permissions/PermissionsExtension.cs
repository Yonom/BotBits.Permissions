using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BotBits.Permissions
{
    public class PermissionsExtension : Extension<PermissionsExtension>
    {
        public static void LoadInto(BotBitsClient client)
        {
            LoadInto(client, null);
        }
    }
}
