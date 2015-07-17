﻿using System;

namespace BotBits.Permissions
{
    public static class PlayerExtensions
    {
        public static DateTime GetBanTimeout(this Player p)
        {
            return p.Get<DateTime>("BanTimeout");
        }

        public static void SetBanTimeout(this Player p, DateTime timeout)
        {
            p.Set("BanTimeout", timeout);
        }

        public static string GetBanReason(this Player p)
        {
            return p.Get<string>("BanReason");
        }

        public static void SetBanReason(this Player p, string reason)
        {
            p.Set("BanReason", reason);
        }

        public static Group GetGroup(this Player p)
        {
            return p.Get<Group>("Group");
        }

        public static void SetGroup(this Player p, Group @group)
        {
            p.Set("Group", @group);
        }

        public static string GetDatabaseName(this Player p)
        {
            if (p.IsGuest) return "-guest-";
            return p.Username;
        }

        internal static PermissionData GetPermissionData(this Player p)
        {
            return new PermissionData(p.GetGroup(), p.GetBanReason(), p.GetBanTimeout());
        }

        internal static void SetPermissionData(this Player p, PermissionData permissionData)
        {
            p.SetBanReason(permissionData.BanReason);
            p.SetBanTimeout(permissionData.BanTimeout);
            // Group is last, because of the event that will be fired
            p.SetGroup(permissionData.Group);
        }

        internal static bool GetPermissionsIniting(this Player p)
        {
            return p.Get<bool>("PermissionsIniting");
        }

        internal static void SetPermissionsIniting(this Player p, bool inited)
        {
            p.Set("PermissionsIniting", inited);
        }
    }
}