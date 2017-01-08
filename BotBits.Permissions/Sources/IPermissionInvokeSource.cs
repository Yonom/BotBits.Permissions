using BotBits.Commands;

namespace BotBits.Permissions
{
    public interface IPermissionInvokeSource : IInvokeSource
    {
        Group Group { get; }
    }
}