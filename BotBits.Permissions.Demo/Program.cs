using System;
using System.Collections.Generic;
using System.Linq;
using BotBits.Commands;
using BotBits.Events;

namespace BotBits.Permissions.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new BotBitsClient();

            EventLoader.Of(bot).LoadStatic<Program>();

            CommandsExtension.LoadInto(bot, '!');
            CommandLoader.Of(bot).LoadStatic<Program>();

            PermissionsExtension.LoadInto(bot, Group.Moderator, new SimplePermissionProvider("processor"));

            // Login
            ConnectionManager.Of(bot)
                .GuestLogin()
                .CreateJoinRoom("PW01");

            while (true)
                CommandManager.Of(bot).ReadNextConsoleCommand();
        }

        
        [EventListener]
        static void OnJoin(JoinEvent e)
        {
            if (e.Username == "processor")
            {
                e.Player.SetGroup(Group.Admin);
            }
        }

        [Command(0, "hi")]
        static void HiCommand(IInvokeSource source, ParsedRequest request)
        {
            Group.Moderator.RequireFor(source);

            var player = source.ToPlayerInvokeSource().Player;
            source.Reply("Hello world {0}!", player.Username);
        }
    }
}
