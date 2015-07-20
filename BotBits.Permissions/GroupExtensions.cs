using System.Runtime.CompilerServices;
using BotBits.Commands;

namespace BotBits.Permissions
{
    public static class GroupExtensions
    {
        public static void RequireFor(this Group group, IInvokeSource source,
            string errorMessage = "You are not allowed to run this command!")
        {
            if (source.GetGroup() < group)
                throw new AccessDeniedCommandException(errorMessage);
        }

        public static void RequireFor(this Group group, Player player,
            string errorMessage = "You are not allowed to run this command!")
        {
            if (player.GetGroup() < group)
                throw new AccessDeniedCommandException(errorMessage);
        }
    }
}
