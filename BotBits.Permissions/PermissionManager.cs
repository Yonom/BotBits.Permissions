using System;
using BotBits.Events;

namespace BotBits.Permissions
{
    public sealed class PermissionManager : EventListenerPackage<PermissionManager>
    {
        [EventListener(EventPriority.Highest)]
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