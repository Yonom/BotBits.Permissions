﻿using BotBits.Commands;

namespace BotBits.Permissions
{
    internal class PlayerPermissionInvokeSourceAdapter : PlayerInvokeSource, IPermissionInvokeSource
    {
        public PlayerPermissionInvokeSourceAdapter(PlayerInvokeSource source)
            : base(source.Player, source.Origin, source.Reply)
        {
        }

        public Group Group
        {
            get { return this.Player.GetGroup(); }
        }
    }
}