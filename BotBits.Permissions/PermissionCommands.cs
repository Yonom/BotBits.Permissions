﻿using System;
using System.Linq;
using System.Threading;
using BotBits.ChatExtras;
using BotBits.Commands;
using BotBits.Events;

namespace BotBits.Permissions
{
    internal sealed class PermissionCommands : Package<PermissionCommands>
    {
        private int _enabled;

        [Obsolete("Invalid to use \"new\" on this class. Use the static .Of(botBits) method instead.", true)]
        public PermissionCommands()
        {
        }

        internal Group MinRespondingGroup { get; private set; }
        internal IPermissionProvider Provider { get; private set; }

        internal void Enable(Group minRespondingGroup, IPermissionProvider provider)
        {
            if (Interlocked.Exchange(ref this._enabled, 1) == 1)
            {
                throw new InvalidOperationException("PermissionCommands have already been enabled.");
            }

            this.MinRespondingGroup = minRespondingGroup;
            this.Provider = provider;

            CommandLoader.Of(this.BotBits).Load(this);
            EventLoader.Of(this.BotBits).Load(this);
        }

        [EventListener]
        private void On(CommandExceptionEvent e)
        {
            try
            {
                if (e.Source.ToPermissionInvokeSource().Group < this.MinRespondingGroup)
                {
                    if (e.Exception is InvalidInvokeOriginCommandException ||
                        e.Exception is InvalidInvokeSourceCommandException ||
                        e.Exception is AccessDeniedCommandException ||
                        e.Exception is SyntaxCommandException) e.Handled = true;
                    return;
                }
            }
            catch (InvalidInvokeSourceCommandException)
            {
            }

            if (e.Exception is UnknownCommandException) e.Handled = false;
        }

        [EventListener]
        private void On(InitEvent e)
        {
            this.Provider.GetDataAsync(e.Player.GetDatabaseName(), e.Player.SetPermissionData);
        }

        [EventListener]
        private void On(JoinEvent e)
        {
            this.Provider.GetDataAsync(e.Player.GetDatabaseName(), e.Player.SetPermissionData);
        }

        [EventListener]
        private void On(PermissionEvent e)
        {
            if (e.NewPermission == Group.Banned)
            {
                if (e.Player.GetBanTimeout() != default(DateTime) && e.Player.GetBanTimeout() < DateTime.UtcNow)
                {
                    this.SetPermissions(e.Player.Username, new PermissionData(Group.User));
                }
                else if (Actions.Of(this.BotBits).AccessRight == AccessRight.Owner)
                {
                    e.Player.Kick("Banned. {0}", GetBanString(e.Player.GetPermissionData()));
                }
            }
        }

        private void ComparePermissions(IInvokeSource source, string username, Action callback)
        {
            this.GetRankAsync(source, username, data =>
            {
                if (source.GetGroup() <= data.Group) throw new AccessDeniedCommandException("The target player is higher or equally ranked.");

                callback();
            });
        }

        private void CompareAndSetPermissions(IInvokeSource source, ParsedRequest request, PermissionData pdata)
        {
            var username = request.GetUsernameIn(this.BotBits, 0);
            this.ComparePermissions(source, username, () =>
            {
                this.SetPermissions(username, pdata);
                source.Reply(GetRankString(username, pdata));
            });
        }

        private void SetPermissions(string username, PermissionData data)
        {
            var players = Players.Of(this.BotBits).FromUsername(username);
            var databaseName = PlayerExtensions.GetDatabaseName(username);
            foreach (var player in players)
            {
                player.SetPermissionData(data);
            }

            this.Provider.SetDataAsync(databaseName, data);
        }

        private void GetRankAsync(IInvokeSource source, string username, Action<PermissionData> callback)
        {
            var player = Players.Of(this.BotBits).FromUsername(username).FirstOrDefault();
            if (player != null)
            {
                callback(player.GetPermissionData());
            }
            else
            {
                this.Provider.GetDataAsync(PlayerExtensions.GetDatabaseName(username),
                    data =>
                    {
                        try
                        {
                            callback(data);
                        }
                        catch (CommandException ex)
                        {
                            source.Reply(ex.Message);
                        }
                    });
            }
        }

        private static string GetRankString(string username, PermissionData data)
        {
            var res = $"{username.ToUpper()} is {data.Group.ToString().ToLower()}.";
            if (data.Group == Group.Banned) res += " " + GetBanString(data);
            return res;
        }

        private static string GetBanString(PermissionData data)
        {
            var res = string.Empty;
            if (!string.IsNullOrEmpty(data.BanReason)) res += "Reason: " + data.BanReason + " ";
            if (data.BanTimeout != default(DateTime)) res += "Expires in: " + GetTimeLeft(data.BanTimeout);
            return res;
        }

        private static string GetTimeLeft(DateTime yourDate)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = yourDate - DateTime.UtcNow;
            if (ts.Ticks < 0) return "<Expired>";
            var delta = ts.TotalSeconds;

            if (delta < 1 * MINUTE)
            {
                return (int)Math.Round(ts.TotalSeconds) == 1 ? "one second" : Math.Round(ts.TotalSeconds) + " seconds";
            }
            if (delta < 2 * MINUTE)
            {
                return "a minute";
            }
            if (delta < 45 * MINUTE)
            {
                return Math.Round(ts.TotalMinutes) + " minutes";
            }
            if (delta < 90 * MINUTE)
            {
                return "an hour";
            }
            if (delta < 24 * HOUR)
            {
                return Math.Round(ts.TotalHours) + " hours";
            }
            if (delta < 48 * HOUR)
            {
                return "tomorrow";
            }
            if (delta < 30 * DAY)
            {
                return Math.Round(ts.TotalDays) + " days";
            }
            if (delta < 12 * MONTH)
            {
                var months = Convert.ToInt32(Math.Floor(ts.TotalDays / 30));
                return months <= 1 ? "one month" : months + " months";
            }
            var years = Convert.ToInt32(Math.Floor(ts.TotalDays / 365));
            return years <= 1 ? "one year" : years + " years";
        }

        [RestrictedCommand(Group.User, 0, "getrank", Usage = "[username]")]
        private void GetRankCommand(IInvokeSource source, ParsedRequest request)
        {
            this.MinRespondingGroup.RequireFor(source);

            var respond = new Action<string, PermissionData>((username, data) =>
                source.Reply(GetRankString(username, data)));

            if (request.Count >= 1)
            {
                var username = request.GetUsernameIn(this.BotBits, 0);
                this.GetRankAsync(source, username, data => respond(username, data));
            }
            else
            {
                var player = source.ToPlayerInvokeSource().Player;
                respond(player.Username, player.GetPermissionData());
            }
        }

        [RestrictedCommand(Group.External, 1, "admin", Usage = "username")]
        private void AdminCommand(IInvokeSource source, ParsedRequest request)
        {
            this.MinRespondingGroup.RequireFor(source);

            this.CompareAndSetPermissions(source, request,
                new PermissionData(Group.Admin));
        }

        [RestrictedCommand(Group.Admin, 1, "op", Usage = "username")]
        private void OpCommand(IInvokeSource source, ParsedRequest request)
        {
            this.MinRespondingGroup.RequireFor(source);

            this.CompareAndSetPermissions(source, request,
                new PermissionData(Group.Operator));
        }

        [RestrictedCommand(Group.Operator, 1, "mod", Usage = "username")]
        private void ModCommand(IInvokeSource source, ParsedRequest request)
        {
            this.MinRespondingGroup.RequireFor(source);

            this.CompareAndSetPermissions(source, request,
                new PermissionData(Group.Moderator));
        }

        [RestrictedCommand(Group.Operator, 1, "tempmod", Usage = "username")]
        private void TempModCommand(IInvokeSource source, ParsedRequest request)
        {
            this.MinRespondingGroup.RequireFor(source);

            var username = request.GetUsernameIn(this.BotBits, 0);
            this.ComparePermissions(source, username, () =>
            {
                var data = new PermissionData(Group.Moderator);
                var players = Players.Of(this.BotBits).FromUsername(username);
                foreach (var player in players)
                {
                    player.SetPermissionData(data);
                }
                source.Reply("{0} is now temporary moderator.", username);
            });
        }

        [RestrictedCommand(Group.Operator, 1, "trust", Usage = "username")]
        private void TrustCommand(IInvokeSource source, ParsedRequest request)
        {
            this.MinRespondingGroup.RequireFor(source);

            this.CompareAndSetPermissions(source, request,
                new PermissionData(Group.Trusted));
        }

        [RestrictedCommand(Group.Operator, 1, "user", Usage = "username")]
        private void UserCommand(IInvokeSource source, ParsedRequest request)
        {
            this.MinRespondingGroup.RequireFor(source);

            this.CompareAndSetPermissions(source, request,
                new PermissionData(Group.User));
        }

        [RestrictedCommand(Group.Operator, 1, "limit", Usage = "username")]
        private void LimitCommand(IInvokeSource source, ParsedRequest request)
        {
            this.MinRespondingGroup.RequireFor(source);

            this.CompareAndSetPermissions(source, request,
                new PermissionData(Group.Limited));
        }

        [RestrictedCommand(Group.Operator, 1, "ban", Usage = "username [reason]")]
        private void BanCommand(IInvokeSource source, ParsedRequest request)
        {
            this.MinRespondingGroup.RequireFor(source);

            this.CompareAndSetPermissions(source, request,
                new PermissionData(Group.Banned, request.GetTrail(1), default(DateTime)));
        }

        [RestrictedCommand(Group.Operator, 2, "tempban", Usage = "username duration [reason]")]
        private void TempBanCommand(IInvokeSource source, ParsedRequest request)
        {
            this.MinRespondingGroup.RequireFor(source);

            DateTime timeout;
            try
            {
                var duration = TimeSpan.Parse(request.Args[1]);
                timeout = DateTime.UtcNow.Add(duration);
            }
            catch (Exception ex)
            {
                throw new CommandException("Unable to parse parameter: duration", ex);
            }

            this.CompareAndSetPermissions(source, request,
                new PermissionData(Group.Banned, request.GetTrail(2), timeout));
        }
    }
}