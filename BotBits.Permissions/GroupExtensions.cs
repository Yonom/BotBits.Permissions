using BotBits.Commands;

namespace BotBits.Permissions
{
    public static class GroupExtensions
    {
        public static void RequireFor(this Group group, IInvokeSource source)
        {
            try
            {
                var permissionInvokeSource = source.ToPermissionInvokeSource();
                if (permissionInvokeSource.Group < group)
                    throw new AccessDeniedCommandException("You are not allowed to run this command!");
            }
            catch (InvalidInvokeSourceCommandException)
            {
                // allow external commands
            }
        }
    }
}
