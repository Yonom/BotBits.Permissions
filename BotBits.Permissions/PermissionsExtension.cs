using System;
using BotBits.Commands;

namespace BotBits.Permissions
{
    public sealed class PermissionsExtension : Extension<PermissionsExtension>
    {
        [Obsolete("Invalid to use \"new\" on this class. Use the static .Of(botBits) method instead.", true)]
        public PermissionsExtension()
        {
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

        public static bool WithCommandsLoadInto(BotBitsClient client, Group minRespondingGroup, IPermissionProvider provider)
        {
            if (!ChatExtrasServices.IsAvailable())
                throw new InvalidOperationException("BotBits.ChatExtras must be installed for permission commands to work.");
            if (!CommandsExtension.IsLoadedInto(client))
                throw new InvalidOperationException("BotBits.CommandsExtension must be loaded for permission commands to work.");

            return LoadInto(client, new Settings(minRespondingGroup, provider));
        }

        private class Settings
        {
            public Settings(Group minRespondingGroup, IPermissionProvider provider)
            {
                this.MinRespondingGroup = minRespondingGroup;
                this.Provider = provider;
            }

            public Group MinRespondingGroup { get; }
            public IPermissionProvider Provider { get; }
        }
    }
}