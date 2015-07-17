using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BotBits.Commands;
using BotBits.Events;

namespace BotBits.Permissions
{
    internal sealed class PermissionCommands : Package<PermissionCommands>
    {
        private int _enabled;
        internal Group MinRespondingGroup { get; private set; }
        internal IPermissionProvider Provider { get; private set; }

        internal void Enable(Group minRespondingGroup, IPermissionProvider provider)
        {
            if (Interlocked.Exchange(ref this._enabled, 1) == 1)
            {
                throw new InvalidOperationException("PermissionCommands have already been enabled.");
            }

            this.MinRespondingGroup = minRespondingGroup;
            this.Provider = provider;

            CommandLoader.Of(this.BotBits).Load(this);
            EventLoader.Of(this.BotBits).Load(this);
        }

        [EventListener]
        private void OnCommandException(CommandExceptionEvent e)
        {
            try
            {
                if (e.Source.ToPermissionInvokeSource().Group <= MinRespondingGroup) return;
            }
            catch (Exception) { }

            
            if (e.Exception is UnknownCommandException)
                e.Handled = false;
            if (e.Exception is AccessDeniedCommandException)
                e.Handled = false;
        }

        [EventListener(EventPriority.High)]
        private void OnPermission(PermissionEvent e)
        {
            if (e.Player.GetPermissionsIniting())
            {
                e.Player.SetPermissionsIniting(false);
            }
            else if(e.NewPermission != e.OldPermission)
            {
                this.Provider.SetDataAsync(e.Player.GetDatabaseName(), e.Player.GetPermissionData());
            }
        }

        [EventListener(EventPriority.High)]
        private void OnJoin(JoinEvent e)
        {
            this.Provider.GetDataAsync(e.Player.GetDatabaseName(), data =>
            {
                e.Player.SetPermissionsIniting(true);
                e.Player.SetPermissionData(data);
            });
        }

        [Command(0, "ban")]
        private void BanCommand(IInvokeSource source, ParsedRequest request)
        {
            
        }
    }
}
