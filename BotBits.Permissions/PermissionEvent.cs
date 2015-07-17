namespace BotBits.Permissions
{
    public sealed class PermissionEvent : Event<PermissionEvent>
    {
        internal PermissionEvent(Player player, Group oldPermission, Group newPermission)
        {
            this.Player = player;
            this.OldPermission = oldPermission;
            this.NewPermission = newPermission;
        }

        public Player Player { get; private set; }
        public Group OldPermission { get; private set; }
        public Group NewPermission { get; private set; }
    }
}