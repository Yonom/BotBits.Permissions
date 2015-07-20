using System;

namespace BotBits.Permissions
{
    public struct PermissionData
    {
        public Group Group { get; private set; }
        public string BanReason { get; private set; }
        public DateTime BanTimeout { get; private set; }

        public PermissionData(Group group) : this()
        {
            this.Group = @group;
        }

        public PermissionData(Group group, string banReason, DateTime banTimeout)
            : this()
        {
            this.Group = @group;
            this.BanReason = banReason;
            this.BanTimeout = banTimeout;
        }
    }
}