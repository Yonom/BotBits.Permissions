using System;

namespace BotBits.Permissions
{
    public class PermissionData
    {
        public Group Group { get; private set; }
        public string BanReason { get; private set; }
        public DateTime BanTimeout { get; private set; }

        public PermissionData(Group group)
        {
            this.Group = @group;
        }

        public PermissionData(Group group, string banReason, DateTime banTimeout)
        {
            this.Group = @group;
            this.BanReason = banReason;
            this.BanTimeout = banTimeout;
        }
    }
}