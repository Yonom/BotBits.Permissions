using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BotBits.Commands;

namespace BotBits.Permissions
{
    public class RestrictedCommandAttribute : CommandAttribute
    {
        public Group MinGroup { get; }

        public RestrictedCommandAttribute(Group minGroup, int minArgs, params string[] names) : base(minArgs, names)
        {
            this.MinGroup = minGroup;
        }

        protected override Action<IInvokeSource, ParsedRequest> DoTransformations(BotBitsClient client, Command command, Action<IInvokeSource, ParsedRequest> callback)
        {
            foreach (var type in command.Names)
            {
                PermissionManager.Of(client).Set(type, this.MinGroup);
            }

            return (source, req) =>
            {
                PermissionManager.Of(client).Get(req.Type).RequireFor(source);
                callback(source, req);
            };
        }
    }
}
