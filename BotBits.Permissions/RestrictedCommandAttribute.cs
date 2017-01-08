using System;
using BotBits.Commands;

namespace BotBits.Permissions
{
    public class RestrictedCommandAttribute : CommandAttribute
    {
        public RestrictedCommandAttribute(Group minGroup, int minArgs, params string[] names) : base(minArgs, names)
        {
            this.MinGroup = minGroup;
        }

        public Group MinGroup { get; }

        protected override Action<IInvokeSource, ParsedRequest> DoTransformations(BotBitsClient client, Command command, Action<IInvokeSource, ParsedRequest> callback)
        {
            return (source, req) =>
            {
                PermissionManager.Of(client).Get(command).RequireFor(source);
                callback(source, req);
            };
        }

        protected override void OnAdd(BotBitsClient client, Command command)
        {
            PermissionManager.Of(client).Set(command, this.MinGroup);
        }

        protected override void OnRemove(BotBitsClient client, Command command)
        {
            PermissionManager.Of(client).Remove(command);
        }
    }
}