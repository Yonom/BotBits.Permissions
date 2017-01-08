using BotBits.ChatExtras;
using BotBits.Commands;
using BotBits.Permissions.Demo.PermissionProviders;

namespace BotBits.Permissions.Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var bot = new BotBitsClient();

            EventLoader.Of(bot).LoadStatic<Program>();

            CommandsExtension.LoadInto(bot, '!');
            CommandLoader.Of(bot).LoadStatic<Program>();

            ChatFormatsExtension.LoadInto(bot, new CakeChatSyntaxProvider("Bot"));
            PermissionsExtension.WithCommandsLoadInto(bot, Group.Moderator,
                new SQLiteDatabasePermissionProvider("Data Source=test.db;Version=3;", "BotBitsUsers"));

            // Login
            Login.Of(bot)
                .AsGuest()
                .CreateJoinRoom("PW01");

            while (true) CommandManager.Of(bot).ReadNextConsoleCommand();
        }

        [Command(0, "hi")]
        private static void HiCommand(IInvokeSource source, ParsedRequest request)
        {
            Group.Moderator.RequireFor(source);

            var player = source.ToPlayerInvokeSource().Player;
            source.Reply("Hello world {0}!", player.Username);
        }
    }
}