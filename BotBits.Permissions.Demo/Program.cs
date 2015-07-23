﻿using BotBits.ChatExtras;
using BotBits.Commands;
using BotBits.Permissions.Demo.PermissionProviders;

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

            ChatExtrasExtension.LoadInto(bot, new CakeChatSyntaxProvider("Bot"));
            PermissionsExtension.LoadInto(bot, Group.Moderator, 
                new SQLiteDatabasePermissionProvider("Data Source=test.db;Version=3;"));

            // Login
            ConnectionManager.Of(bot)
                .GuestLogin()
                .CreateJoinRoom("PWAARLDluVa0I");

            while (true)
                CommandManager.Of(bot).ReadNextConsoleCommand();
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
