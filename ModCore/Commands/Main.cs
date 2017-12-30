using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Humanizer;
using Humanizer.Localisation;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Listeners;
using ModCore.Logic.Utils;

namespace ModCore.Commands
{
    public class Main
    {
        private static readonly Regex SpaceReplacer = new Regex(" {2,}", RegexOptions.Compiled);

        public SharedData Shared { get; }
        public DatabaseContextBuilder Database { get; }
        public InteractivityExtension Interactivity { get; }
        public StartTimes StartTimes { get; }

        public Main(SharedData shared, DatabaseContextBuilder db, InteractivityExtension interactive,
            StartTimes starttimes)
        {
            this.Database = db;
            this.Shared = shared;
            this.Interactivity = interactive;
            this.StartTimes = starttimes;
        }

        [Command("ping"), Description("Check ModCore's API connection status.")]
        public async Task PingAsync(CommandContext ctx)
        {
            await ctx.RespondAsync($"Pong: ({ctx.Client.Ping}) ms.");
        }

        [Command("help"), Aliases("h", "?", "wtf"), Description("Displays information about commands.")]
        public async Task HelpAsync(CommandContext ctx, [Description("Command to provide information for")]params string[] command)
        {
            await ctx.CommandsNext.DefaultHelpAsync(ctx, command);
        }

        [Command("uptime"), Description("Check ModCore's uptime."), Aliases("u")]
        public async Task UptimeAsync(CommandContext ctx)
        {
            var st = this.StartTimes;
            var bup = DateTimeOffset.Now.Subtract(st.ProcessStartTime);
            var sup = DateTimeOffset.Now.Subtract(st.SocketStartTime);

            // Needs improvement
            await ctx.RespondAsync(
                $"Program uptime: {string.Format("{0} days, {1}", bup.ToString("dd"), bup.ToString(@"hh\:mm\:ss"))}\n" +
                $"Socket uptime: {string.Format("{0} days, {1}", sup.ToString("dd"), sup.ToString(@"hh\:mm\:ss"))}");
        }

        [Command("invite"), Description("Get an invite to this ModCore instance. Sharing is caring!"), Aliases("inv")]
        public async Task InviteAsync(CommandContext ctx)
        {
            //TODO replace with a link to a nice invite builder!
            // what the hell is an invite builder? - chris
            var app = ctx.Client.CurrentApplication;
            if (app.IsPublic != null && (bool)app.IsPublic)
                await ctx.RespondAsync(
                    $"Add ModCore to your server!\n<https://discordapp.com/oauth2/authorize?client_id={app.Id}&scope=bot>");
            else
                await ctx.RespondAsync("I'm sorry Mario, but this instance of ModCore has been set to private!");
        }

        [Command("purgeuser"), Description("Delete an amount of messages by an user."), Aliases("pu"),
         RequirePermissions(Permissions.ManageMessages)]
        public async Task PurgeUserAsync(CommandContext ctx, [Description("User to delete messages from")]DiscordUser user,
            [Description("Message limit.")]int limit, [Description("Amount of messages to skip")]int skip = 0)
        {
            var i = 0;
            var ms = await ctx.Channel.GetMessagesBeforeAsync(ctx.Message, limit);
            var deletThis = new List<DiscordMessage>();
            foreach (var m in ms)
            {
                if (user != null && m.Author.Id != user.Id) continue;
                if (i < skip)
                    i++;
                else
                    deletThis.Add(m);
            }
            if (deletThis.Any())
                await ctx.Channel.DeleteMessagesAsync(deletThis,
                    $"Purged messages by {user?.Username}#{user?.Discriminator} (ID:{user?.Id})");
            var resp = await ctx.RespondAsync($"Latest messages by {user?.Mention} (ID:{user?.Id}) deleted.");
            await Task.Delay(2000);
            await resp.DeleteAsync("Purge command executed.");
            await ctx.Message.DeleteAsync("Purge command executed.");

            await ctx.LogActionAsync(
                $"Purged messages.\nUser: {user?.Username}#{user?.Discriminator} (ID:{user?.Id})\nChannel: #{ctx.Channel.Name} ({ctx.Channel.Id})");
        }

        private static List<string> Tokenize(string value, char sep, char block)
        {
            var result = new List<string>();
            var sb = new StringBuilder();
            var insideBlock = false;
            foreach (var c in value)
            {
                if (insideBlock && c == '\\')
                {
                    continue;
                }
                if (c == block)
                {
                    insideBlock = !insideBlock;
                }
                else if (c == sep && !insideBlock)
                {
                    if (sb.IsNullOrWhitespace()) continue;
                    result.Add(sb.ToString().Trim());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }
            if (sb.ToString().Trim().Length > 0)
            {
                result.Add(sb.ToString().Trim());
            }

            return result;
        }

        [Command("purgeregexp"), Description(
             "For power users! Delete messages from the current channel by regular expression match. " +
             "Pass a Regexp in ECMAScript ( /expression/flags ) format, or simply a regex string " +
             "in quotes."), Aliases("purgeregex", "pr"), RequirePermissions(Permissions.ManageMessages)]
        public async Task PurgeRegexpAsync(CommandContext ctx, [RemainingText, Description("Your regex")] string regexp, 
            [Description("Message limit")]int limit = 50, [Description("Amount of messages to skip")]int skip = 0)
        {
            // TODO add a flag to disable CultureInvariant.
            var regexOptions = RegexOptions.CultureInvariant;
            // kept here for displaying in the result
            var flags = "";

            if (string.IsNullOrEmpty(regexp))
            {
                await ctx.RespondAsync("RegExp is empty");
                return;
            }
            var blockType = regexp[0];
            if (blockType == '"' || blockType == '/')
            {
                // token structure
                // "regexp" limit? skip?
                // /regexp/ limit? skip?
                // /regexp/ flags limit? skip? 
                var tokens = Tokenize(SpaceReplacer.Replace(regexp, " ").Trim(), ' ', blockType);
                regexp = tokens[0];
                if (tokens.Count > 1)
                {
                    // parse flags only in ECMAScript regexp literal
                    if (blockType == '/')
                    {
                        // if tokens[1] is a valid integer then it's `limit`. otherwise it's `flags`, and we remove it
                        // for the other bits.
                        flags = tokens[1];
                        if (!int.TryParse(flags, out var _))
                        {
                            // remove the flags element
                            tokens.RemoveAt(1);

                            if (flags.Contains('m'))
                            {
                                regexOptions |= RegexOptions.Multiline;
                            }
                            if (flags.Contains('i'))
                            {
                                regexOptions |= RegexOptions.IgnoreCase;
                            }
                            if (flags.Contains('s'))
                            {
                                regexOptions |= RegexOptions.Singleline;
                            }
                            if (flags.Contains('x'))
                            {
                                regexOptions |= RegexOptions.ExplicitCapture;
                            }
                            if (flags.Contains('r'))
                            {
                                regexOptions |= RegexOptions.RightToLeft;
                            }
                            // for debugging only
                            if (flags.Contains('c'))
                            {
                                regexOptions |= RegexOptions.Compiled;
                            }
                        }
                    }

                    if (int.TryParse(tokens[1], out var result))
                    {
                        limit = result;
                    }
                    else
                    {
                        await ctx.RespondAsync(tokens[1] + " is not a valid int");
                        return;
                    }
                    if (tokens.Count > 2)
                    {
                        if (int.TryParse(tokens[2], out var res2))
                        {
                            skip = res2;
                        }
                        else
                        {
                            await ctx.RespondAsync(tokens[2] + " is not a valid int");
                            return;
                        }
                    }
                }
            }
            var regexCompiled = new Regex(regexp, regexOptions);

            var i = 0;
            var ms = await ctx.Channel.GetMessagesBeforeAsync(ctx.Message, limit);
            var deletThis = new List<DiscordMessage>();
            foreach (var m in ms)
            {
                if (!regexCompiled.IsMatch(m.Content)) continue;

                if (i < skip)
                    i++;
                else
                    deletThis.Add(m);
            }
            var resultString =
                $"Purged {deletThis.Count} messages by /{regexp.Replace("/", @"\/").Replace(@"\", @"\\")}/{flags}";
            if (deletThis.Any())
                await ctx.Channel.DeleteMessagesAsync(deletThis, resultString);
            var resp = await ctx.RespondAsync(resultString);
            await Task.Delay(2000);
            await resp.DeleteAsync("Purge command executed.");
            await ctx.Message.DeleteAsync("Purge command executed.");

            await ctx.LogActionAsync(
                $"Purged {deletThis.Count} messages.\nRegex: ```\n{regexp}```\nFlags: {flags}\nChannel: #{ctx.Channel.Name} ({ctx.Channel.Id})");
        }

        [Command("purge"), Description("Delete an amount of messages from the current channel."), Aliases("p"),
         RequirePermissions(Permissions.ManageMessages)]
        public async Task PurgeAsync(CommandContext ctx, [Description("Amount of messages to remove")]int limit, 
            [Description("Amount of messages to skip")]int skip = 0)
        {
            var i = 0;
            var ms = await ctx.Channel.GetMessagesBeforeAsync(ctx.Message, limit);
            var deletThis = new List<DiscordMessage>();
            foreach (var m in ms)
            {
                if (i < skip)
                    i++;
                else
                    deletThis.Add(m);
            }
            if (deletThis.Any())
                await ctx.Channel.DeleteMessagesAsync(deletThis, "Purged messages.");
            var resp = await ctx.RespondAsync("Latest messages deleted.");
            await Task.Delay(2000);
            await resp.DeleteAsync("Purge command executed.");
            await ctx.Message.DeleteAsync("Purge command executed.");

            await ctx.LogActionAsync($"Purged messages.\nChannel: #{ctx.Channel.Name} ({ctx.Channel.Id})");
        }

        [Command("clean"), Description("Purge ModCore's messages."), Aliases("c"),
         RequirePermissions(Permissions.ManageMessages)]
        public async Task CleanAsync(CommandContext ctx)
        {
            var gs = ctx.GetGuildSettings() ?? new GuildSettings();
            var prefix = gs?.Prefix ?? "?>";
            var ms = await ctx.Channel.GetMessagesBeforeAsync(ctx.Message, 100);
            var deletThis = ms.Where(m => m.Author.Id == ctx.Client.CurrentUser.Id || m.Content.StartsWith(prefix))
                .ToList();
            if (deletThis.Any())
                await ctx.Channel.DeleteMessagesAsync(deletThis, "Cleaned up commands");
            var resp = await ctx.RespondAsync("Latest messages deleted.");
            await Task.Delay(2000);
            await resp.DeleteAsync("Clean command executed.");
            await ctx.Message.DeleteAsync("Clean command executed.");

            await ctx.LogActionAsync();
        }

        [Command("ban"), Description("Bans a member."), Aliases("b"), RequirePermissions(Permissions.BanMembers)]
        public async Task BanAsync(CommandContext ctx, [Description("Member to ban")] DiscordMember m,
            [RemainingText, Description("Reason to ban this member")] string reason = "")
        {
            if (ctx.Member.Id == m.Id)
            {
                await ctx.RespondAsync("You can't do that to yourself! You have so much to live for!");
                return;
            }

            var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
            var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
            await m.SendMessageAsync($"You've been banned from {ctx.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the follwing reason:\n```\n{reason}\n```")}");
            await ctx.Guild.BanMemberAsync(m, 7, $"{ustr}{rstr}");
            await ctx.RespondAsync($"Banned user {m.DisplayName} (ID:{m.Id})");

            await ctx.LogActionAsync($"Banned user {m.DisplayName} (ID:{m.Id})\n{rstr}");
        }

        [Command("hackban"), Description("Ban an user by their ID. The user does not need to be in the guild."),
         Aliases("hb"), RequirePermissions(Permissions.BanMembers)]
        public async Task HackBanAsync(CommandContext ctx, [Description("ID of user to ban")]ulong id, 
            [RemainingText, Description("Reason to ban this member")] string reason = "")
        {
            if (ctx.Member.Id == id)
            {
                await ctx.RespondAsync("You can't do that to yourself! You have so much to live for!");
                return;
            }

            var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
            var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
            await ctx.Guild.BanMemberAsync(id, 7, $"{ustr}{rstr}");
            await ctx.RespondAsync("User hackbanned successfully.");

            await ctx.LogActionAsync($"Hackbanned ID: {id}\n{rstr}");
        }

        [Command("kick"), Description("Kicks a member from the guild. Can optionally provide a reason for kick."),
         Aliases("k"), RequirePermissions(Permissions.KickMembers)]
        public async Task KickAsync(CommandContext ctx, [Description("Member to kick")]DiscordMember m, 
            [RemainingText, Description("Reason to kick this member")] string reason = "")
        {
            if (ctx.Member.Id == m.Id)
            {
                await ctx.RespondAsync("You can't do that to yourself! You have so much to live for!");
                return;
            }

            var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
            var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
            await m.SendMessageAsync($"You've been kicked from {ctx.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the follwing reason:\n```\n{reason}\n```")}");
            await m.RemoveAsync($"{ustr}{rstr}");
            await ctx.RespondAsync($"Kicked user {m.DisplayName} (ID:{m.Id})");

            await ctx.LogActionAsync($"Kicked user {m.DisplayName} (ID:{m.Id})\n{rstr}");
        }

        [Command("softban"),
         Description("Bans then unbans an user from the guild. " +
                     "This will delete their recent messages, but they can join back."), Aliases("sb"),
         RequireUserPermissions(Permissions.KickMembers), RequireBotPermissions(Permissions.BanMembers)]
        public async Task SoftbanAsync(CommandContext ctx, [Description("Member to softban")]DiscordMember m, 
            [RemainingText, Description("Reason to softban this member")] string reason = "")
        {
            if (ctx.Member.Id == m.Id)
            {
                await ctx.RespondAsync("You can't do that to yourself! You have so much to live for!");
                return;
            }

            var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
            var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
            await m.SendMessageAsync($"You've been kicked from {ctx.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the follwing reason:\n```\n{reason}\n```")}");
            await m.BanAsync(7, $"{ustr}{rstr} (softban)");
            await m.UnbanAsync(ctx.Guild, $"{ustr}{rstr}");
            await ctx.RespondAsync($"Softbanned user {m.DisplayName} (ID:{m.Id})");

            await ctx.LogActionAsync($"Softbanned user {m.DisplayName} (ID:{m.Id})\n{rstr}");
        }

       

        [Group("globalwarn"), Aliases("gw", "gwarn", "globalw"), Description("Commands to add or remove globalwarns."), 
            RequireUserPermissions(Permissions.Administrator), RequireBotPermissions(Permissions.BanMembers)]
        public class GlobalWarn
        {
            private DatabaseContextBuilder Database { get; }

            public GlobalWarn(DatabaseContextBuilder db)
            {
                this.Database = db;
            }

            [Command("add"), Description("Adds the specified user to a global watchlist.")]
            public async Task AddAsync(CommandContext ctx, [Description("Member to warn about")]DiscordMember m,
           [RemainingText, Description("Reason to warn about this member")] string reason = "")
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                if (cfg.GlobalWarn.WarnLevel == GlobalWarnLevel.None || cfg.GlobalWarn.Enable)
                    await ctx.RespondAsync("You do not have globalwarn enabled on this server.");

                bool issuedBefore = false;
                using (var db = this.Database.CreateContext())
                    issuedBefore = db.Bans.Any(x => x.GuildId == (long)ctx.Guild.Id && x.UserId == (long)m.Id);
                if (issuedBefore)
                {
                    await ctx.RespondAsync("You have already warned about this user! Stop picking on them...");
                    return;
                }
                if (ctx.Member.Id == m.Id)
                {
                    await ctx.RespondAsync("You can't do that to yourself! You have so much to live for!");
                    return;
                }

                var ban = new DatabaseBan
                {
                    GuildId = (long)ctx.Guild.Id,
                    UserId = (long)m.Id,
                    IssuedAt = DateTime.Now,
                    BanReason = reason
                };
                using (var db = this.Database.CreateContext())
                {
                    db.Bans.Add(ban);
                    await db.SaveChangesAsync();
                }

                var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
                var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
                await m.SendMessageAsync($"You've been banned from {ctx.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the following reason:\n```\n{reason}\n```")}");
                await ctx.Guild.BanMemberAsync(m, 7, $"{ustr}{rstr}");
                await ctx.RespondAsync($"Banned and issued global warn about user {m.DisplayName} (ID:{m.Id})");

                await ctx.LogActionAsync($"Banned and issued global warn about user {m.DisplayName} (ID:{m.Id})\n{rstr}\n");
                await GlobalWarnUpdateAsync(ctx, m, true);
            }

            [Command("remove"), Description("Removes the specified user from the global watchlist.")]
            public async Task RemoveAsync(CommandContext ctx, [Description("Member to warn about")]DiscordMember m)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                if (cfg.GlobalWarn.WarnLevel == GlobalWarnLevel.None || cfg.GlobalWarn.Enable)
                    await ctx.RespondAsync("You do not have globalwarn enabled on this server.");

                bool issuedBefore = false;
                using (var db = this.Database.CreateContext())
                    issuedBefore = db.Bans.Any(x => x.GuildId == (long)ctx.Guild.Id && x.UserId == (long)m.Id);
                if (issuedBefore)
                {
                    await ctx.RespondAsync("You have already warned about this user! Stop picking on them...");
                    return;
                }
                using (var db = this.Database.CreateContext())
                {
                    db.Bans.Remove(db.Bans.First(x => x.GuildId == (long)ctx.Guild.Id && x.UserId == (long)m.Id));
                    await db.SaveChangesAsync();
                }

                var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
                await m.SendMessageAsync($"You've been unbanned from {ctx.Guild.Name}.");
                await ctx.Guild.UnbanMemberAsync(m, $"{ustr}");
                await ctx.RespondAsync($"Unbanned and retracted global warn about user {m.DisplayName} (ID:{m.Id})");

                await ctx.LogActionAsync($"Unbanned and retracted global warn about user {m.DisplayName} (ID:{m.Id})\n");
                await GlobalWarnUpdateAsync(ctx, m, false);
            }

            private async Task GlobalWarnUpdateAsync(CommandContext ctx, DiscordMember m, bool banNotify)
            {
                DatabaseBan[] bans;
                using (var db = this.Database.CreateContext())
                {
                    bans = db.Bans.Where(x => x.UserId == (long)m.Id).ToArray();

                    var prevowns = new List<ulong>();
                    int count = 0;
                    var guilds = ModCore.Shards.SelectMany(x => x.Client.Guilds.Values);
                    foreach (var b in bans)
                    {
                        var g = guilds.First(x => x.Id == (ulong)b.GuildId);
                        if (prevowns.Contains(g.Owner.Id))
                            continue;
                        count++;
                        prevowns.Add(g.Owner.Id);
                    }
                    if (banNotify)
                    {
                        if (count > 2)
                        {
                            foreach (DiscordGuild g in guilds)
                            {
                                try
                                {
                                    var settings = g.GetGuildSettings(db) ?? new GuildSettings();
                                    DiscordMember guildmember = await g.GetMemberAsync(m.Id);

                                    if (guildmember != null && g.Id != ctx.Guild.Id && settings.GlobalWarn.Enable)
                                    {
                                        var embed = new DiscordEmbedBuilder()
                                            .WithColor(DiscordColor.MidnightBlue)
                                            .WithTitle($"WARNING: @{m.Username}#{m.Discriminator} - ID: {m.Id}");

                                        var banString = new StringBuilder();
                                        foreach (DatabaseBan ban in bans) banString.Append($"[{ban.GuildId} - {ban.BanReason}] ");
                                        embed.AddField("Bans", banString.ToString());

                                        if (settings.GlobalWarn.WarnLevel == GlobalWarnLevel.Owner)
                                        {
                                            await g.Owner.SendMessageAsync("", embed: embed);
                                        }
                                        else if (settings.GlobalWarn.WarnLevel == GlobalWarnLevel.JoinLog)
                                        {
                                            await g.Channels.First(x => x.Id == (ulong)settings.JoinLog.ChannelId).SendMessageAsync(embed: embed);
                                        }
                                    }
                                }
                                catch
                                {
                                    // TODO: Make SSG Proud
                                }
                            }
                        }
                    }
                    else
                    {
                        if (count >= 0)
                        {
                            foreach (DiscordGuild g in guilds)
                            {
                                try
                                {
                                    var settings = g.GetGuildSettings(db) ?? new GuildSettings();
                                    DiscordUser user = await ctx.Client.GetUserAsync(m.Id);

                                    if (user != null && g.Id != ctx.Guild.Id && settings.GlobalWarn.Enable)
                                    {
                                        var embed = new DiscordEmbedBuilder()
                                            .WithColor(DiscordColor.MidnightBlue)
                                            .WithTitle($"INFORMATION: @{m.Username}#{m.Discriminator} - ID: {m.Id}")
                                            .WithDescription($"User has been *unbanned*, with global warn removed, from {ctx.Guild.Name}.");

                                        if (count == 0)
                                        {
                                            embed.Description += "\nHe is now banned on no guilds.";
                                        }
                                        else
                                        {
                                            var banString = new StringBuilder();
                                            foreach (DatabaseBan ban in bans) banString.Append($"[{ban.GuildId} - {ban.BanReason}] ");
                                            embed.AddField("Bans", banString.ToString()); 
                                        }
                                        if (settings.GlobalWarn.WarnLevel == GlobalWarnLevel.Owner)
                                        {
                                            await g.Owner.SendMessageAsync("", embed: embed);
                                        }
                                        else if (settings.GlobalWarn.WarnLevel == GlobalWarnLevel.JoinLog)
                                        {
                                            await g.Channels.First(x => x.Id == (ulong)settings.JoinLog.ChannelId).SendMessageAsync(embed: embed);
                                        }
                                    }
                                }
                                catch
                                {
                                    // TODO: Make SSG Proud
                                }
                            }
                        }
                    }
                }
            }
        }

        [Command("mute"), Description("Mutes an user indefinitely. This will prevent them from speaking in chat. " +
                                      "You might need to set up a mute role, but most of the time ModCore can do it " +
                                      "for you."), Aliases("m"), RequirePermissions(Permissions.MuteMembers),
         RequireBotPermissions(Permissions.ManageRoles)]
        public async Task MuteAsync(CommandContext ctx, [Description("Member to mute")]DiscordMember m, 
            [RemainingText, Description("Reason to mute this member")] string reason = "")
        {
            if (ctx.Member.Id == m.Id)
            {
                await ctx.RespondAsync("You can't do that to yourself! You have so much to live for!");
                return;
            }

            var guildSettings = ctx.GetGuildSettings() ?? new GuildSettings();
            if (guildSettings == null)
            {
                await ctx.RespondAsync("Guild is not configured, please configure and rerun");
                return;
            }

            var b = guildSettings.MuteRoleId;
            var mute = ctx.Guild.GetRole(b);
            if (b == 0 || mute == null)
            {
                var setupStatus = await Utils.SetupMuteRole(ctx.Guild, ctx.Member, m);
                mute = setupStatus.Role;
                guildSettings.MuteRoleId = setupStatus.Role.Id;
                await ctx.RespondAsync("Mute role is not configured or missing, " + setupStatus.Message);
                await ctx.SetGuildSettingsAsync(guildSettings);
            }
            await Utils.GuaranteeMuteRoleDeniedEverywhere(ctx.Guild, mute);

            var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
            var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
            await m.SendMessageAsync($"You've been muted in {ctx.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the follwing reason:\n```\n{reason}\n```")}");
            await m.GrantRoleAsync(mute, $"{ustr}{rstr} (mute)");
            await ctx.RespondAsync(
                $"Muted user {m.DisplayName} (ID:{m.Id}) {(reason != "" ? "With reason: " + reason : "")}");

            await ctx.LogActionAsync(
                $"Muted user {m.DisplayName} (ID:{m.Id}) {(reason != "" ? "With reason: " + reason : "")}");
        }

        [Command("unmute"), Description("Unmutes an user previously muted with the mute command. Let them speak!"),
         Aliases("um"), RequirePermissions(Permissions.MuteMembers),
         RequireBotPermissions(Permissions.ManageRoles)]
        public async Task UnmuteAsync(CommandContext ctx, [Description("Member to unmute")]DiscordMember m, 
            [RemainingText, Description("Reason to unmute this member")] string reason = "")
        {
            if (ctx.Member.Id == m.Id)
            {
                await ctx.RespondAsync("You can't do that to yourself! You have so much to live for!");
                return;
            }

            var gcfg = ctx.GetGuildSettings() ?? new GuildSettings();
            if (gcfg == null)
            {
                await ctx.RespondAsync(
                    "Guild is not configured. Adjust this guild's configuration and re-run this command.");
                return;
            }

            var b = gcfg.MuteRoleId;
            var mute = ctx.Guild.GetRole(b);
            if (b == 0 || mute == null)
            {
                await ctx.RespondAsync(
                    "Mute role is not configured or missing. Set a correct role and re-run this command.");
                return;
            }

            var t = Timers.FindNearestTimer(TimerActionType.Unmute, m.Id, 0, ctx.Guild.Id, this.Database);
            if (t != null)
                await Timers.UnscheduleTimerAsync(t, ctx.Client, this.Database, this.Shared);

            var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
            var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
            await m.SendMessageAsync($"You've been unmuted in {ctx.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the follwing reason:\n```\n{reason}\n```")}");
            await m.RevokeRoleAsync(mute, $"{ustr}{rstr} (unmute)");
            await ctx.RespondAsync(
                $"Unmuted user {m.DisplayName} (ID:{m.Id}) {(reason != "" ? "With reason: " + reason : "")}");

            await ctx.LogActionAsync(
                $"Unmuted user {m.DisplayName} (ID:{m.Id}) {(reason != "" ? "With reason: " + reason : "")}");
        }

        [Command("leave"), Description("Makes this bot leave the current server. Goodbye moonmen."),
         RequireUserPermissions(Permissions.Administrator)]
        public async Task LeaveAsync(CommandContext ctx)
        {
            var interactivity = this.Interactivity;
            await ctx.RespondAsync("Are you sure you want to remove modcore from your guild?");
            var m = await interactivity.WaitForMessageAsync(
                x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Member.Id, TimeSpan.FromSeconds(30));

            if (m == null)
                await ctx.RespondAsync("Timed out.");
            else if (m.Message.Content == "yes")
            {
                await ctx.RespondAsync("Thanks for using ModCore. Leaving this guild.");
                await ctx.LogActionAsync("Left your server. Thanks for using ModCore.");
                await ctx.Guild.LeaveAsync();
            }
            else
                await ctx.RespondAsync("Operation canceled by user.");
        }

        [Command("tempban"), Aliases("tb"), Description(
             "Temporarily bans a member. They will be automatically unbanned " +
             "after a set amount of time."),
         RequirePermissions(Permissions.BanMembers)]
        public async Task TempBanAsync(CommandContext ctx, [Description("Member to ban temporarily")]DiscordMember m, 
            [Description("How long this member will be banned")]TimeSpan ts, [Description("Why this member got banned")]string reason = "")
        {
            if (ctx.Member.Id == m.Id)
            {
                await ctx.RespondAsync("You can't do that to yourself! You have so much to live for!");
                return;
            }

            var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
            var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
            await m.SendMessageAsync($"You've been temporarily banned from {ctx.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the following reason:\n```\n{reason}\n```")}" +
                $"\nYou can rejoin after {ts.Humanize(4, minUnit: TimeUnit.Second)}");
            await m.BanAsync(7, $"{ustr}{rstr}");
            // Add timer
            var now = DateTimeOffset.UtcNow;
            var dispatchAt = now + ts;

            var reminder = new DatabaseTimer
            {
                GuildId = (long)ctx.Guild.Id,
                ChannelId = 0,
                UserId = (long)m.Id,
                DispatchAt = dispatchAt.LocalDateTime,
                ActionType = TimerActionType.Unban
            };
            reminder.SetData(new TimerUnbanData
            {
                Discriminator = m.Discriminator,
                DisplayName = m.Username,
                UserId = (long)m.Id
            });
            using (var db = this.Database.CreateContext())
            {
                db.Timers.Add(reminder);
                await db.SaveChangesAsync();
            }

            Timers.RescheduleTimers(ctx.Client, this.Database, this.Shared);

            // End of Timer adding
            await ctx.RespondAsync(
                $"Tempbanned user {m.DisplayName} (ID:{m.Id}) to be unbanned in {ts.Humanize(4, minUnit: TimeUnit.Second)}");

            await ctx.LogActionAsync(
                $"Tempbanned user {m.DisplayName} (ID:{m.Id}) to be unbanned in {ts.Humanize(4, minUnit: TimeUnit.Second)}");
        }

        [Command("tempmute"), Aliases("tm"), Description("Temporarily mutes a member. They will be automatically " +
                                                         "unmuted after a set amount of time. This will prevent them " +
                                                         "from speaking in chat. You might need to set up a mute role, " +
                                                         "but most of the time ModCore can do it for you."),
         RequirePermissions(Permissions.MuteMembers)]
        public async Task TempMuteAsync(CommandContext ctx, [Description("Member to temporarily mute")]DiscordMember m, 
            [Description("How long this member will be muted")]TimeSpan ts, [Description("Reason to temp mute this member")]string reason = "")
        {
            if (ctx.Member.Id == m.Id)
            {
                await ctx.RespondAsync("You can't do that to yourself! You have so much to live for!");
                return;
            }

            var guildSettings = ctx.GetGuildSettings() ?? new GuildSettings();
            if (guildSettings == null)
            {
                await ctx.RespondAsync(
                    "Guild is not configured. Adjust this guild's configuration and re-run this command.");
                return;
            }

            var b = guildSettings.MuteRoleId;
            var mute = ctx.Guild.GetRole(b);
            if (b == 0 || mute == null)
            {
                var setupStatus = await Utils.SetupMuteRole(ctx.Guild, ctx.Member, m);
                mute = setupStatus.Role;
                guildSettings.MuteRoleId = setupStatus.Role.Id;
                await ctx.RespondAsync("Mute role is not configured or missing, " + setupStatus.Message);
                await ctx.SetGuildSettingsAsync(guildSettings);
            }
            await Utils.GuaranteeMuteRoleDeniedEverywhere(ctx.Guild, mute);

            var timer = Timers.FindNearestTimer(TimerActionType.Unmute, m.Id, 0, ctx.Guild.Id, this.Database);
            if (timer != null)
            {
                await ctx.RespondAsync("This member was already muted! Please try to unmute them first!");
                return;
            }

            var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
            var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
            await m.SendMessageAsync($"You've been temporarily muted in {ctx.Guild.Name}{(string.IsNullOrEmpty(reason) ? "." : $" with the following reason:\n```\n{reason}\n```")}" +
                $"\nYou can talk again after {ts.Humanize(4, minUnit: TimeUnit.Second)}");
            await m.GrantRoleAsync(mute, $"{ustr}{rstr} (mute)");
            // Add timer
            var now = DateTimeOffset.UtcNow;
            var dispatchAt = now + ts;

            var reminder = new DatabaseTimer
            {
                GuildId = (long)ctx.Guild.Id,
                ChannelId = 0,
                UserId = (long)m.Id,
                DispatchAt = dispatchAt.LocalDateTime,
                ActionType = TimerActionType.Unmute
            };
            reminder.SetData(new TimerUnmuteData
            {
                Discriminator = m.Discriminator,
                DisplayName = m.Username,
                UserId = (long)m.Id,
                MuteRoleId = (long)(ctx.GetGuildSettings() ?? new GuildSettings()).MuteRoleId
            });
            using (var db = this.Database.CreateContext())
            {
                db.Timers.Add(reminder);
                await db.SaveChangesAsync();
            }

            Timers.RescheduleTimers(ctx.Client, this.Database, this.Shared);

            // End of Timer adding
            await ctx.RespondAsync(
                $"Tempmuted user {m.DisplayName} (ID:{m.Id}) to be unmuted in {ts.Humanize(4, minUnit: TimeUnit.Second)}");

            await ctx.LogActionAsync(
                $"Tempmuted user {m.DisplayName} (ID:{m.Id}) to be unmuted in {ts.Humanize(4, minUnit: TimeUnit.Second)}");
        }

        [Command("schedulepin"), Aliases("sp"), Description("Schedules a pinned message. _I really don't know why " +
                                                            "you'd want to do this._"),
         RequirePermissions(Permissions.ManageMessages)]
        public async Task SchedulePinAsync(CommandContext ctx, [Description("Message to schedule a pin for")]DiscordMessage message,
            [Description("How long it will take for this message to get pinned")]TimeSpan pinfrom)
        {
            // Add timer
            var now = DateTimeOffset.UtcNow;
            var dispatchAt = now + pinfrom;

            var reminder = new DatabaseTimer
            {
                GuildId = (long)ctx.Guild.Id,
                ChannelId = (long)ctx.Channel.Id,
                UserId = (long)ctx.User.Id,
                DispatchAt = dispatchAt.LocalDateTime,
                ActionType = TimerActionType.Pin
            };
            reminder.SetData(new TimerPinData { MessageId = (long)message.Id, ChannelId = (long)ctx.Channel.Id });
            using (var db = this.Database.CreateContext())
            {
                db.Timers.Add(reminder);
                await db.SaveChangesAsync();
            }

            Timers.RescheduleTimers(ctx.Client, this.Database, this.Shared);

            // End of Timer adding
            await ctx.RespondAsync(
                $"After {pinfrom.Humanize(4, minUnit: TimeUnit.Second)} this message will be pinned");
        }

        [Command("scheduleunpin"), Aliases("sup"), Description("Schedules unpinning a pinned message. This command " +
                                                               "really is useless, isn't it?"),
         RequirePermissions(Permissions.ManageMessages)]
        public async Task ScheduleUnpinAsync(CommandContext ctx, [Description("Message to schedule unpinning for")]DiscordMessage message,
            [Description("Time it will take before this message gets unpinned")]TimeSpan pinuntil)
        {
            if (!message.Pinned)
                await message.PinAsync();
            // Add timer
            var now = DateTimeOffset.UtcNow;
            var dispatchAt = now + pinuntil;

            var reminder = new DatabaseTimer
            {
                GuildId = (long)ctx.Guild.Id,
                ChannelId = (long)ctx.Channel.Id,
                UserId = (long)ctx.User.Id,
                DispatchAt = dispatchAt.LocalDateTime,
                ActionType = TimerActionType.Unpin
            };
            reminder.SetData(new TimerUnpinData { MessageId = (long)message.Id, ChannelId = (long)ctx.Channel.Id });
            using (var db = this.Database.CreateContext())
            {
                db.Timers.Add(reminder);
                await db.SaveChangesAsync();
            }

            Timers.RescheduleTimers(ctx.Client, this.Database, this.Shared);

            // End of Timer adding
            await ctx.RespondAsync(
                $"In {pinuntil.Humanize(4, minUnit: TimeUnit.Second)} this message will be unpinned.");
        }

        [Command("listbans"), Aliases("lb"), Description("Lists banned users. Real complex stuff.")]
        public async Task ListBansAsync(CommandContext ctx)
        {
            var bans = await ctx.Guild.GetBansAsync();
            if (bans.Count == 0)
            {
                await ctx.RespondAsync("No user is banned.");
                return;
            }
            var interactivity = this.Interactivity;

            string banString = string.Join("\n", bans.Select((ban, idx) => FormatDiscordBan(ban, idx+1)));

            var p = this.Interactivity.GeneratePagesInEmbeds(banString);
            await this.Interactivity.SendPaginatedMessage(ctx.Channel, ctx.Member, p);
        }
        [Group("selfrole"), Description("Commands to give or take selfroles."), RequireBotPermissions(Permissions.ManageRoles)]
        public class SelfRole
        {
            private DatabaseContextBuilder Database { get; }

            public SelfRole(DatabaseContextBuilder db)
            {
                this.Database = db;
            }

            [Command("give"), Aliases("g"), Description("Gives the command callee a specified role, if " +
                                                                     "ModCore has been configured to allow so.")]
            public async Task GiveAsync(CommandContext ctx, [RemainingText, Description("Role you want to give to yourself")] DiscordRole role)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings(); ;
                if (cfg.SelfRoles.Contains(role.Id))
                {
                    if (ctx.Member.Roles.Any(x => x.Id == role.Id))
                    {
                        await ctx.RespondAsync("You already have that role!");
                        return;
                    }
                    if (ctx.Guild.CurrentMember.Roles.Any(x => x.Position >= role.Position))
                    {
                        await ctx.Member.GrantRoleAsync(role, "AutoRole granted.");
                        await ctx.RespondAsync($"Granted you the role `{role.Name}`.");
                    }
                    else
                        await ctx.RespondAsync("Can't grant you this role because that role is above my highest role!");
                }
                else
                {
                    await ctx.RespondAsync("You can't grant yourself that role!");
                }
            }

            [Command("take"), Aliases("t"), Description("Removes a specified role from the command callee, if " +
                                                                     "ModCore has been configured to allow so.")]
            public async Task TakeAsync(CommandContext ctx, [RemainingText, Description("Role you want to take from yourself")] DiscordRole role)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings(); ;

                if (cfg.SelfRoles.Contains(role.Id))
                {
                    if (ctx.Member.Roles.All(x => x.Id != role.Id))
                    {
                        await ctx.RespondAsync("You don't have that role!");
                        return;
                    }
                    if (ctx.Guild.CurrentMember.Roles.Any(x => x.Position >= role.Position))
                    {
                        await ctx.Member.RevokeRoleAsync(role, "AutoRole revoke.");
                        await ctx.RespondAsync($"Revoked your role: `{role.Name}`.");
                    }
                    else
                        await ctx.RespondAsync("Can't take this role because that role is above my highest role!");
                }
                else
                {
                    await ctx.RespondAsync("You can't revoke that role!");
                }
            }

            [Command("list"), Aliases("l"), Description("Lists all available selfroles, if any.")]
            public async Task ListAsync(CommandContext ctx)
            {
                GuildSettings cfg;
                cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                if (cfg.SelfRoles.Any())
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = ctx.Guild.Name,
                        ThumbnailUrl = ctx.Guild.IconUrl
                    };
                    var roles = cfg.SelfRoles
                        .Select(ctx.Guild.GetRole)
                        .Where(x => x != null)
                        .Select(x => x.Mention);

                    embed.AddField("Available SelfRoles", string.Join(", ", roles), true);
                    await ctx.RespondAsync(embed: embed);
                }
                else
                {
                    await ctx.RespondAsync("No available selfroles.");
                }
            }
        }

        [Command("announce"), Description("Announces a message to a channel, additionally mentioning a role.")]
        [RequireBotPermissions(Permissions.ManageRoles), RequireUserPermissions(Permissions.MentionEveryone)]
        public async Task AnnounceAsync(CommandContext ctx, [Description("Role to announce for")]DiscordRole role, 
            [Description("Channel to announce to")]DiscordChannel channel,[RemainingText, Description("Announcement text")] string message)
        {
            if (!role.IsMentionable)
            {
                await role.UpdateAsync(mentionable: true);
                await channel.SendMessageAsync($"{role.Mention} {message}");
                await role.UpdateAsync(mentionable: false);
                await ctx.Message.DeleteAsync();
                await ctx.LogActionAsync($"Announced {message}\nTo channel: #{channel.Name}\nTo role: {role.Name}");
            }
            else
            {
                await ctx.Channel.SendMessageAsync("You can't announce to that role because it is mentionable!");
                await ctx.LogActionAsync(
                    $"Failed announcement\nMessage: {message}\nTo channel: #{channel.Name}\nTo role: {role.Name}");
            }
        }

        private static string FormatDiscordBan(DiscordBan ban, int number)
        {
            return $"{number}. **{ban.User.ToDiscordTag()}**, Reason: {ban.Reason}";
        }
    }
}
