using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BotBits.Events;

namespace BotBits.Permissions
{
    public sealed class PermissionManager : EventListenerPackage<PermissionManager>
    {
        private readonly ConcurrentDictionary<string, Group> _minPermissions = new ConcurrentDictionary<string, Group>(StringComparer.OrdinalIgnoreCase);

        public void Set(string commandName, Group minPermission)
        {
            this._minPermissions[commandName] = minPermission;
        }

        public Group Get(string commandName)
        {
            Group res;
            this._minPermissions.TryGetValue(commandName, out res);
            return res;
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