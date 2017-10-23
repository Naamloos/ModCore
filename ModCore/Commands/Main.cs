using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ModCore.Database;
using ModCore.Entities;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DSharpPlus.Interactivity;
using System.Threading;
using ModCore.Listeners;
using Humanizer.Localisation;
using Humanizer;

namespace ModCore.Commands
{
    public class Main
    {
        private static readonly Regex SpaceReplacer = new Regex(" {2,}");
        
        public SharedData Shared { get; }
        public DatabaseContextBuilder Database { get; }
        public InteractivityExtension Interactivity { get; }

        public Main(SharedData shared, DatabaseContextBuilder db, InteractivityExtension interactive)
        {
            this.Database = db;
            this.Shared = shared;
            this.Interactivity = interactive;
        }

        [Command("ping")]
        public async Task PingAsync(CommandContext ctx)
        {
            await ctx.RespondAsync($"Pong: ({ctx.Client.Ping}) ms.");
        }

        [Command("uptime"), Aliases("u")]
        public async Task UptimeAsync(CommandContext ctx)
        {
            var st = ctx.Dependencies.GetDependency<StartTimes>();
            var bup = DateTimeOffset.Now.Subtract(st.ProcessStartTime);
            var sup = DateTimeOffset.Now.Subtract(st.SocketStartTime);

            // Needs improvement
            await ctx.RespondAsync($"Program uptime: {string.Format("{0} days, {1}", bup.ToString("dd"), bup.ToString(@"hh\:mm\:ss"))}\n" +
                $"Socket uptime: {string.Format("{0} days, {1}", sup.ToString("dd"), sup.ToString(@"hh\:mm\:ss"))}");
        }

        [Command("invite"), Aliases("inv")]
        public async Task InviteAsync(CommandContext ctx)
        {
            //TODO replace with a link to a nice invite builder!
            var app = ctx.Client.CurrentApplication;
            if (app.IsPublic != null && (bool)app.IsPublic)
                await ctx.RespondAsync($"Add ModCore to your server!\n<https://discordapp.com/oauth2/authorize?client_id={app.Id}&scope=bot>");
            else
                await ctx.RespondAsync("I'm sorry Mario, but this instance of ModCore has been set to private!");
        }

        [Command("purgeuser"), Aliases("pu"), RequirePermissions(Permissions.ManageMessages)]
        public async Task PurgeUserAsync(CommandContext ctx, DiscordUser user, int limit, int skip = 0)
        {
            var i = 0;
            var ms = await ctx.Channel.GetMessagesAsync(limit, ctx.Message.Id);
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
                await ctx.Channel.DeleteMessagesAsync(deletThis, $"Purged messages by {user.Username}#{user.Discriminator} (ID:{user.Id})");
            var resp = await ctx.RespondAsync($"Latest messages by {user.Mention} (ID:{user.Id}) deleted.");
            await Task.Delay(2000);
            await resp.DeleteAsync("Purge command executed.");
            await ctx.Message.DeleteAsync("Purge command executed.");

            await ctx.LogActionAsync($"Purged messages.\nUser: {user.Username}#{user.Discriminator} (ID:{user.Id})\nChannel: #{ctx.Channel.Name} ({ctx.Channel.Id})");
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

        [Command("purgeregexp"), Aliases("purgeregex", "pr"), RequirePermissions(Permissions.ManageMessages)]
        public async Task PurgeRegexpAsync(CommandContext ctx, [RemainingText] string regexp, int limit = 50, int skip = 0)
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
                            
                            if (flags.Contains('m')) {
                                regexOptions |= RegexOptions.Multiline;
                            }
                            if (flags.Contains('i')) {
                                regexOptions |= RegexOptions.IgnoreCase;
                            }
                            if (flags.Contains('s')) {
                                regexOptions |= RegexOptions.Singleline;
                            }
                            if (flags.Contains('x')) {
                                regexOptions |= RegexOptions.ExplicitCapture;
                            }
                            if (flags.Contains('r')) {
                                regexOptions |= RegexOptions.RightToLeft;
                            }
                            // for debugging only
                            if (flags.Contains('c')) {
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
            var ms = await ctx.Channel.GetMessagesAsync(limit, ctx.Message.Id);
            var deletThis = new List<DiscordMessage>();
            foreach (var m in ms)
            {
                if (!regexCompiled.IsMatch(m.Content)) continue;
                
                if (i < skip)
                    i++;
                else
                    deletThis.Add(m);
            }
            var resultString = $"Purged {deletThis.Count} messages by /{regexp.Replace("/", @"\/").Replace(@"\",@"\\")}/{flags}";
            if (deletThis.Any())
                await ctx.Channel.DeleteMessagesAsync(deletThis, resultString);
            var resp = await ctx.RespondAsync(resultString);
            await Task.Delay(2000);
            await resp.DeleteAsync("Purge command executed.");
            await ctx.Message.DeleteAsync("Purge command executed.");

            await ctx.LogActionAsync($"Purged {deletThis.Count} messages.\nRegex: ```\n{regexp}```\nFlags: {flags}\nChannel: #{ctx.Channel.Name} ({ctx.Channel.Id})");
        }

        [Command("purge"), Aliases("p"), RequirePermissions(Permissions.ManageMessages)]
        public async Task PurgeAsync(CommandContext ctx, int limit, int skip = 0)
        {
            var i = 0;
            var ms = await ctx.Channel.GetMessagesAsync(limit, ctx.Message.Id);
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
            var resp = await ctx.RespondAsync($"Latest messages deleted.");
            await Task.Delay(2000);
            await resp.DeleteAsync("Purge command executed.");
            await ctx.Message.DeleteAsync("Purge command executed.");

            await ctx.LogActionAsync($"Purged messages.\nChannel: #{ctx.Channel.Name} ({ctx.Channel.Id})");
        }

        [Command("clean"), Aliases("c"), RequirePermissions(Permissions.ManageMessages)]
        public async Task CleanAsync(CommandContext ctx)
        {
            var gs = ctx.GetGuildSettings();
            var prefix = gs != null? gs.Prefix : "?>";
            var ms = await ctx.Channel.GetMessagesAsync(100, ctx.Message.Id);
            var delet_this = new List<DiscordMessage>();
            foreach (var m in ms)
            {
                if (m.Author.Id == ctx.Client.CurrentUser.Id || m.Content.StartsWith(prefix))
                    delet_this.Add(m);
            }
            if (delet_this.Any())
                await ctx.Channel.DeleteMessagesAsync(delet_this, "Cleaned up commands");
            var resp = await ctx.RespondAsync($"Latest messages deleted.");
            await Task.Delay(2000);
            await resp.DeleteAsync("Clean command executed.");
            await ctx.Message.DeleteAsync("Clean command executed.");

            await ctx.LogActionAsync();
        }

        [Command("ban"), Aliases("b"), RequirePermissions(Permissions.BanMembers)]
        public async Task BanAsync(CommandContext ctx, DiscordMember m, [RemainingText]string reason = "")
        {
            if (ctx.Member.Id == m.Id)
            {
                await ctx.RespondAsync("You can't do that to yourself! You have so much to live for!");
                return;
            }

            var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
            var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
            await ctx.Guild.BanMemberAsync(m, 7, $"{ustr}{rstr}");
            await ctx.RespondAsync($"Banned user {m.DisplayName} (ID:{m.Id})");

            await ctx.LogActionAsync($"Banned user {m.DisplayName} (ID:{m.Id})\n{rstr}");
        }

        [Command("hackban"), Aliases("hb"), RequirePermissions(Permissions.BanMembers)]
        public async Task HackBanAsync(CommandContext ctx, ulong id, [RemainingText]string reason = "")
        {
            if (ctx.Member.Id == id)
            {
                await ctx.RespondAsync("You can't do that to yourself! You have so much to live for!");
                return;
            }

            var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
            var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
            await ctx.Guild.BanMemberAsync(id, 7, $"{ustr}{rstr}");
            await ctx.RespondAsync($"User hackbanned successfully.");

            await ctx.LogActionAsync($"Hackbanned ID: {id}\n{rstr}");
        }

        [Command("kick"), Aliases("k"), RequirePermissions(Permissions.KickMembers)]
        public async Task KickAsync(CommandContext ctx, DiscordMember m, [RemainingText]string reason = "")
        {
            if (ctx.Member.Id == m.Id)
            {
                await ctx.RespondAsync("You can't do that to yourself! You have so much to live for!");
                return;
            }

            var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
            var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
            await m.RemoveAsync($"{ustr}{rstr}");
            await ctx.RespondAsync($"Kicked user {m.DisplayName} (ID:{m.Id})");

            await ctx.LogActionAsync($"Kicked user {m.DisplayName} (ID:{m.Id})\n{rstr}");
        }

        [Command("softban"), Aliases("s"), RequireUserPermissions(Permissions.KickMembers), RequireBotPermissions(Permissions.BanMembers)]
        public async Task SoftbanAsync(CommandContext ctx, DiscordMember m, [RemainingText]string reason = "")
        {
            if (ctx.Member.Id == m.Id)
            {
                await ctx.RespondAsync("You can't do that to yourself! You have so much to live for!");
                return;
            }

            var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
            var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
            await m.BanAsync(7, $"{ustr}{rstr} (softban)");
            await m.UnbanAsync(ctx.Guild, $"{ustr}{rstr}");
            await ctx.RespondAsync($"Softbanned user {m.DisplayName} (ID:{m.Id})");

            await ctx.LogActionAsync($"Softbanned user {m.DisplayName} (ID:{m.Id})\n{rstr}");
        }

        [Command("mute"), Aliases("m"), RequirePermissions(Permissions.MuteMembers), RequireBotPermissions(Permissions.ManageRoles)]
        public async Task MuteAsync(CommandContext ctx, DiscordMember m, [RemainingText]string reason = "")
        {
            if (ctx.Member.Id == m.Id)
            {
                await ctx.RespondAsync("You can't do that to yourself! You have so much to live for!");
                return;
            }

            var gcfg = ctx.GetGuildSettings();
            if (gcfg == null)
            {
                await ctx.RespondAsync("Guild is not configured. Adjust this guild's configuration and re-run this command.");
                return;
            }

            var b = gcfg.MuteRoleId;
            var mute = ctx.Guild.GetRole(b);
            if (b == 0 || mute == null)
            {
                await ctx.RespondAsync("Mute role is not configured or missing. Set a correct role and re-run this command.");
                return;
            }

            var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
            var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
            await m.GrantRoleAsync(mute, $"{ustr}{rstr} (mute)");
            await ctx.RespondAsync($"Muted user {m.DisplayName} (ID:{m.Id}) { (reason != "" ? "With reason: " + reason : "")}");

            await ctx.LogActionAsync($"Muted user {m.DisplayName} (ID:{m.Id}) { (reason != "" ? "With reason: " + reason : "")}");
        }

        [Command("unmute"), Aliases("um"), RequirePermissions(Permissions.MuteMembers), RequireBotPermissions(Permissions.ManageRoles)]
        public async Task UnmuteAsync(CommandContext ctx, DiscordMember m, [RemainingText]string reason = "")
        {
            if (ctx.Member.Id == m.Id)
            {
                await ctx.RespondAsync("You can't do that to yourself! You have so much to live for!");
                return;
            }

            var gcfg = ctx.GetGuildSettings();
            if (gcfg == null)
            {
                await ctx.RespondAsync("Guild is not configured. Adjust this guild's configuration and re-run this command.");
                return;
            }

            var b = gcfg.MuteRoleId;
            var mute = ctx.Guild.GetRole(b);
            if (b == 0 || mute == null)
            {
                await ctx.RespondAsync("Mute role is not configured or missing. Set a correct role and re-run this command.");
                return;
            }

            var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
            var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
            await m.RevokeRoleAsync(mute, $"{ustr}{rstr} (unmute)");
            await ctx.RespondAsync($"Unmuted user {m.DisplayName} (ID:{m.Id}) { (reason != "" ? "With reason: " + reason : "")}");

            await ctx.LogActionAsync($"Unmuted user {m.DisplayName} (ID:{m.Id}) { (reason != "" ? "With reason: " + reason : "")}");
        }

        [Command("leave"), Description("Makes this bot leave the current server."), RequireUserPermissions(Permissions.Administrator)]
        public async Task LeaveAsync(CommandContext ctx)
        {
            var interactivity = ctx.Dependencies.GetDependency<InteractivityExtension>();
            await ctx.RespondAsync("Are you sure you want to remove modcore from your guild?");
            var m = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Member.Id, TimeSpan.FromSeconds(30));

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

        [Command("tempban"), Aliases("tb"), Description("Temporarily bans a member."), RequirePermissions(Permissions.BanMembers)]
        public async Task TempBanAsync(CommandContext ctx, DiscordMember m, TimeSpan ts, string reason = "")
        {
            if (ctx.Member.Id == m.Id)
            {
                await ctx.RespondAsync("You can't do that to yourself! You have so much to live for!");
                return;
            }

            var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
            var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
            await m.BanAsync(7, $"{ustr}{rstr}");
            // Add timer
            var now = DateTimeOffset.UtcNow;
            var dispatch_at = now + ts;

            // lock the timers
            await this.Shared.TimerSempahore.WaitAsync();

            var reminder = new DatabaseTimer
            {
                GuildId = (long)ctx.Guild.Id,
                ChannelId = (long)ctx.Channel.Id,
                UserId = (long)ctx.User.Id,
                DispatchAt = dispatch_at.LocalDateTime,
                ActionType = TimerActionType.Unban
            };
            reminder.SetData(new TimerUnbanData { Discriminator = m.Discriminator, DisplayName = m.Username, UserId = (long)m.Id });
            var db = this.Database.CreateContext();
            db.Timers.Add(reminder);
            await db.SaveChangesAsync();

            if (this.Shared.TimerData == null || this.Shared.TimerData.DispatchTime >= dispatch_at)
            {
                var tdata = this.Shared.TimerData;
                tdata?.Cancel?.Cancel();

                var cts = new CancellationTokenSource();
                var t = Task.Delay(reminder.DispatchAt - DateTimeOffset.UtcNow, cts.Token);
                tdata = new TimerData(t, reminder, ctx.Client, this.Database, this.Shared, cts);
                _ = t.ContinueWith(Timers.TimerCallback, tdata, TaskContinuationOptions.OnlyOnRanToCompletion);
                this.Shared.TimerData = tdata;
            }
            this.Shared.TimerSempahore.Release();
            // End of Timer adding
            await ctx.RespondAsync($"Tempbanned user {m.DisplayName} (ID:{m.Id}) to be unbanned in {ts.Humanize(4, minUnit: TimeUnit.Second)}");

            await ctx.LogActionAsync($"Tempbanned user {m.DisplayName} (ID:{m.Id}) to be unbanned in {ts.Humanize(4, minUnit: TimeUnit.Second)}");
        }

        [Command("tempmute"), Aliases("tm"), Description("Temporarily mutes a member."), RequirePermissions(Permissions.MuteMembers)]
        public async Task TempMuteAsync(CommandContext ctx, DiscordMember m, TimeSpan ts, string reason = "")
        {
            if (ctx.Member.Id == m.Id)
            {
                await ctx.RespondAsync("You can't do that to yourself! You have so much to live for!");
                return;
            }

            var gcfg = ctx.GetGuildSettings();
            if (gcfg == null)
            {
                await ctx.RespondAsync("Guild is not configured. Adjust this guild's configuration and re-run this command.");
                return;
            }

            var b = gcfg.MuteRoleId;
            var mute = ctx.Guild.GetRole(b);
            if (b == 0 || mute == null)
            {
                await ctx.RespondAsync("Mute role is not configured or missing. Set a correct role and re-run this command.");
                return;
            }

            var ustr = $"{ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})";
            var rstr = string.IsNullOrWhiteSpace(reason) ? "" : $": {reason}";
            await m.GrantRoleAsync(mute, $"{ustr}{rstr} (mute)");
            // Add timer
            var now = DateTimeOffset.UtcNow;
            var dispatch_at = now + ts;

            // lock the timers
            await this.Shared.TimerSempahore.WaitAsync();

            var reminder = new DatabaseTimer
            {
                GuildId = (long)ctx.Guild.Id,
                ChannelId = (long)ctx.Channel.Id,
                UserId = (long)ctx.User.Id,
                DispatchAt = dispatch_at.LocalDateTime,
                ActionType = TimerActionType.Unmute
            };
            reminder.SetData(new TimerUnmuteData { Discriminator = m.Discriminator, DisplayName = m.Username, UserId = (long)m.Id, MuteRoleId = (long)ctx.GetGuildSettings().MuteRoleId });
            var db = this.Database.CreateContext();
            db.Timers.Add(reminder);
            await db.SaveChangesAsync();

            if (this.Shared.TimerData == null || this.Shared.TimerData.DispatchTime >= dispatch_at)
            {
                var tdata = this.Shared.TimerData;
                tdata?.Cancel?.Cancel();

                var cts = new CancellationTokenSource();
                var t = Task.Delay(reminder.DispatchAt - DateTimeOffset.UtcNow, cts.Token);
                tdata = new TimerData(t, reminder, ctx.Client, this.Database, this.Shared, cts);
                _ = t.ContinueWith(Timers.TimerCallback, tdata, TaskContinuationOptions.OnlyOnRanToCompletion);
                this.Shared.TimerData = tdata;
            }
            this.Shared.TimerSempahore.Release();
            // End of Timer adding
            await ctx.RespondAsync($"Tempmuted user {m.DisplayName} (ID:{m.Id}) to be unmuted in {ts.Humanize(4, minUnit: TimeUnit.Second)}");

            await ctx.LogActionAsync($"Tempmuted user {m.DisplayName} (ID:{m.Id}) to be unmuted in {ts.Humanize(4, minUnit: TimeUnit.Second)}");
        }

        [Command("schedulepin"), Aliases("sp"), Description("Schedules a pinned message."), RequirePermissions(Permissions.ManageMessages)]
        public async Task SchedulePinAsync(CommandContext ctx, DiscordMessage message, TimeSpan pinfrom)
        {
            // Add timer
            var now = DateTimeOffset.UtcNow;
            var dispatch_at = now + pinfrom;

            // lock the timers
            await this.Shared.TimerSempahore.WaitAsync();

            var reminder = new DatabaseTimer
            {
                GuildId = (long)ctx.Guild.Id,
                ChannelId = (long)ctx.Channel.Id,
                UserId = (long)ctx.User.Id,
                DispatchAt = dispatch_at.LocalDateTime,
                ActionType = TimerActionType.Pin
            };
            reminder.SetData(new TimerPinData { MessageId = (long)message.Id, ChannelId = (long)ctx.Channel.Id });
            var db = this.Database.CreateContext();
            db.Timers.Add(reminder);
            await db.SaveChangesAsync();

            if (this.Shared.TimerData == null || this.Shared.TimerData.DispatchTime >= dispatch_at)
            {
                var tdata = this.Shared.TimerData;
                tdata?.Cancel?.Cancel();

                var cts = new CancellationTokenSource();
                var t = Task.Delay(reminder.DispatchAt - DateTimeOffset.UtcNow, cts.Token);
                tdata = new TimerData(t, reminder, ctx.Client, this.Database, this.Shared, cts);
                _ = t.ContinueWith(Timers.TimerCallback, tdata, TaskContinuationOptions.OnlyOnRanToCompletion);
                this.Shared.TimerData = tdata;
            }
            this.Shared.TimerSempahore.Release();
            // End of Timer adding
            await ctx.RespondAsync($"After {pinfrom.Humanize(4, minUnit: TimeUnit.Second)} this message will be pinned");
        }

        [Command("scheduleunpin"), Aliases("sup"), Description("Schedules unpinning a pinned message."), RequirePermissions(Permissions.ManageMessages)]
        public async Task ScheduleUnpinAsync(CommandContext ctx, DiscordMessage message, TimeSpan pinuntil)
        {
            if (!message.Pinned)
                await message.PinAsync();
            // Add timer
            var now = DateTimeOffset.UtcNow;
            var dispatch_at = now + pinuntil;

            // lock the timers
            await this.Shared.TimerSempahore.WaitAsync();

            var reminder = new DatabaseTimer
            {
                GuildId = (long)ctx.Guild.Id,
                ChannelId = (long)ctx.Channel.Id,
                UserId = (long)ctx.User.Id,
                DispatchAt = dispatch_at.LocalDateTime,
                ActionType = TimerActionType.Unpin
            };
            reminder.SetData(new TimerUnpinData { MessageId = (long)message.Id, ChannelId = (long)ctx.Channel.Id });
            var db = this.Database.CreateContext();
            db.Timers.Add(reminder);
            await db.SaveChangesAsync();

            if (this.Shared.TimerData == null || this.Shared.TimerData.DispatchTime >= dispatch_at)
            {
                var tdata = this.Shared.TimerData;
                tdata?.Cancel?.Cancel();

                var cts = new CancellationTokenSource();
                var t = Task.Delay(reminder.DispatchAt - DateTimeOffset.UtcNow, cts.Token);
                tdata = new TimerData(t, reminder, ctx.Client, this.Database, this.Shared, cts);
                _ = t.ContinueWith(Timers.TimerCallback, tdata, TaskContinuationOptions.OnlyOnRanToCompletion);
                this.Shared.TimerData = tdata;
            }
            this.Shared.TimerSempahore.Release();
            // End of Timer adding
            await ctx.RespondAsync($"In {pinuntil.Humanize(4, minUnit: TimeUnit.Second)} this message will be unpinned.");
        }

        [Command("listbans")]
        [Aliases("lb")]
        public async Task ListBansAsync(CommandContext ctx, int limit, int skip = 0)
        {
            var bans = await ctx.Guild.GetBansAsync();

            if (bans.Count == 0)
            {
                await ctx.RespondAsync("No user is banned.");
                return;
            }

            var embed = new DiscordEmbedBuilder();
            embed.WithTitle("Banned Users");

            var pagedBans = bans.Skip(skip).Take(limit);
            var formattedBans = pagedBans.Select((ban, idx) => FormatDiscordBan(ban, idx + skip + 1));
            embed.WithDescription(string.Join("\n", formattedBans));

            embed.WithFooter(
                $"Total {bans.Count} banned users. Showing {skip + 1} - {Math.Min(skip + limit, bans.Count)}.");

            await ctx.RespondAsync(embed: embed.Build());
        }

        [Command("gimme"), Aliases("giveme", "gm"), RequireBotPermissions(Permissions.ManageRoles)]
        public async Task GimmeAsync(CommandContext ctx, [RemainingText]DiscordRole Role)
        {
            var cfg = ctx.Guild.GetGuildSettings(Database.CreateContext());
            if (cfg.SelfRoles.Contains(Role.Id))
            {
                if(ctx.Member.Roles.Any(x => x.Id == Role.Id))
                {
                    await ctx.RespondAsync("You already have that role!");
                    return;
                }
                if (ctx.Guild.CurrentMember.Roles.Any(x => x.Position >= Role.Position))
                {
                    await ctx.Member.GrantRoleAsync(Role, "AutoRole granted.");
                    await ctx.RespondAsync($"Granted you the role `{Role.Name}`.");
                }
                else
                    await ctx.RespondAsync("Can't grant you this role because that role is above my highest role!");
            }
            else
            {
                await ctx.RespondAsync("You can't grant yourself that role!");
            }
        }

        [Command("revokeme"), Aliases("rm"), RequireBotPermissions(Permissions.ManageRoles)]
        public async Task RevokemeAsync(CommandContext ctx, [RemainingText]DiscordRole Role)
        {
            var cfg = ctx.Guild.GetGuildSettings(Database.CreateContext());
            if (cfg.SelfRoles.Contains(Role.Id))
            {
                if (!ctx.Member.Roles.Any(x => x.Id == Role.Id))
                {
                    await ctx.RespondAsync("You don't have that role!");
                    return;
                }
                if (ctx.Guild.CurrentMember.Roles.Any(x => x.Position >= Role.Position))
                {
                    await ctx.Member.RevokeRoleAsync(Role, "AutoRole revoke.");
                    await ctx.RespondAsync($"Revoked your role: `{Role.Name}`.");
                }
                else
                    await ctx.RespondAsync("Can't take this role because that role is above my highest role!");
            }
            else
            {
                await ctx.RespondAsync("You can't revoke that role!");
            }
        }

        private static string FormatDiscordBan(DiscordBan ban, int number)
        {
            return $"{number}. **{ban.User.ToDiscordTag()}**, Reason: {ban.Reason}";
        }
    }
}
