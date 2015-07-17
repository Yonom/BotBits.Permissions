using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BotBits.Commands;

namespace BotBits.Permissions
{
    public interface IPermissionInvokeSource : IInvokeSource
    {
        Group Group { get; }
    }
}
