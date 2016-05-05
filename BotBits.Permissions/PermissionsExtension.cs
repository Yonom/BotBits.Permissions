using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BotBits.Commands;

namespace BotBits.Permissions
{
    public sealed class PermissionsExtension : Extension<PermissionsExtension>
    {
        private class Settings
        {
            public Group MinRespondingGroup { get; }
            public IPermissionProvider Provider { get; }

            public Settings(Group minRespondingGroup, IPermissionProvider provider)
            {
                this.MinRespondingGroup = minRespondingGroup;
                this.Provider = provider;
            }
        }

        protected override void Initialize(BotBitsClient client, object args)
        {
            var settings = (Settings)args;
            if (settings != null)
            {
                PermissionCommands.Of(client).Enable(settings.MinRespondingGroup, settings.Provider);
            }
        }

        public static bool LoadInto(BotBitsClient client)
        {
            return LoadInto(client, null);
        }
        
        public static bool LoadInto(BotBitsClient client, Group minRespondingGroup, IPermissionProvider provider)
        {
            if (!CommandsExtension.IsLoadedInto(client))
                throw new InvalidOperationException("You need to load CommandsExtension before you can enable permission commands!");

            return LoadInto(client, new Settings(minRespondingGroup, provider));
        }
    }
}
