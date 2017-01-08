using System;
using System.Collections.Concurrent;
using BotBits.Commands;
using BotBits.Events;

namespace BotBits.Permissions
{
    public sealed class PermissionManager : EventListenerPackage<PermissionManager>
    {
        private readonly ConcurrentDictionary<Command, Group> _minPermissions = new ConcurrentDictionary<Command, Group>();

        [Obsolete("Invalid to use \"new\" on this class. Use the static .Of(botBits) method instead.", true)]
        public PermissionManager()
        {
        }

        public void Set(Command command, Group minPermission)
        {
            this._minPermissions[command] = minPermission;
        }

        public Group Get(Command command)
        {
            Group res;
            this._minPermissions.TryGetValue(command, out res);
            return res;
        }

        public bool Remove(Command command)
        {
            Group res;
            return this._minPermissions.TryRemove(command, out res);
        }

        [EventListener(GlobalPriority.BeforeMost)]
        private void OnJoin(JoinEvent e)
        {
            e.Player.MetadataChanged +=
                (o, args) =>
                {
                    if (args.Key == "Group")
                        new PermissionEvent(e.Player, (Group)args.OldValue,
                            (Group)args.NewValue)
                            .RaiseIn(this.BotBits);
                };
        }

        public void Admin(Player player)
        {
            player.SetGroup(Group.Admin);
        }

        public void Op(Player player)
        {
            player.SetGroup(Group.Operator);
        }

        public void Mod(Player player)
        {
            player.SetGroup(Group.Moderator);
        }

        public void Trust(Player player)
        {
            player.SetGroup(Group.Trusted);
        }

        public void User(Player player)
        {
            player.SetGroup(Group.User);
        }

        public void Limit(Player player)
        {
            player.SetGroup(Group.Limited);
        }

        public void Ban(Player player, string reason)
        {
            player.SetPermissionData(new PermissionData(Group.Banned, reason, default(DateTime)));
        }

        public void Ban(Player player, string reason, DateTime timeout)
        {
            player.SetPermissionData(new PermissionData(Group.Banned, reason, timeout));
        }
    }
}