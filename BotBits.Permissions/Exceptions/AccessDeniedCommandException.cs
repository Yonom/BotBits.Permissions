using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BotBits.Commands;

namespace BotBits.Permissions
{
    /// <summary>
    ///     The exception that is thrown when a user that does not have the required rights attempts to run a command
    /// </summary>
    public class AccessDeniedCommandException : CommandException
    {
        public AccessDeniedCommandException() : base(true)
        {
        }

        public AccessDeniedCommandException(string message)
            : base(message, true)
        {
        }

        public AccessDeniedCommandException(string message, Exception innerException)
            : base(message, innerException, true)
        {
        }
    }
}
