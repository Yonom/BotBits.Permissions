using System;
using BotBits.ChatExtras;

namespace BotBits.Permissions
{
    internal class ChatExtrasServices
    {
        public static bool IsAvailable()
        {
            try
            {
                AttemptUse();
            }
            catch
            {
                return false;
            }

            return true;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private static Type AttemptUse()
        {
            return typeof(ChatFormatsExtension);
        }
    }
}