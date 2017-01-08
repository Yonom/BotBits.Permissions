using System;
using BotBits.Commands;

namespace BotBits.Permissions
{
    /// <summary>
    ///     The exception that is thrown when a user that does not have the required rights attempts to run a command
    /// </summary>
    public class AccessDeniedCommandException : CommandException
    {
        public AccessDeniedCommandException()
        {
        }

        public AccessDeniedCommandException(string message)
            : base(message)
        {
        }

        public AccessDeniedCommandException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}