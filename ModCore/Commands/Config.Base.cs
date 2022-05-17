using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using ModCore.Entities;
using ModCore.Logic;
using ModCore.Logic.Extensions;

namespace ModCore.Commands
{
    public partial class Config
    {
        [GroupCommand, Description("Starts the ModCore configuration wizard. You probably want to do this first!")]
        public async Task ExecuteGroupAsync(CommandContext ctx)
        {
            await ctx.IfGuildSettingsAsync(async () =>
                {
                    await ctx.RespondAsync($"Your server has not been set up with ModCore yet.\n" +
                        $"Execute `{Shared.DefaultPrefix}config setup` to set it up."); // this is the default prefix.
                },
                async gcfg =>
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = ctx.Guild.Name,
                        Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail() { Url = ctx.Guild.IconUrl },
                        Description = "ModCore configuration for this guild:"
                    };

                    embed.AddField("Command Prefix", gcfg.Prefix != null ? $"\"{gcfg.Prefix}\"" : "Not configured",
                        true);

                    var muted = gcfg.MuteRoleId != 0 ? ctx.Guild.GetRole(gcfg.MuteRoleId) : null;
                    embed.AddField("Muted Role", muted != null ? muted.Mention : "Not configured or missing", true);

                    var logs = gcfg.Logging;
                    embed.AddField("Logging",
                        $"Member join/leave: " + (logs.JoinLog_Enable ? "Enabled" : "Disabled") +
                        $"\nMessage edits: " + (logs.EditLog_Enable ? "Enabled" : "Disabled") +
                        $"\nAvatar edits: " + (logs.AvatarLog_Enable ? "Enabled" : "Disabled") +
                        $"\nNickname edits: " + (logs.NickameLog_Enable ? "Enabled" : "Disabled")
                        );

                    var autorole = gcfg.AutoRole;
                    embed.AddField("Auto Role",
                        autorole.Enable ? $"Enabled with Role ID {autorole.RoleId}." : "Disabled");

                    var commanderror = gcfg.CommandError;
                    embed.AddField("Command Error logging",
                        $"Chat: {commanderror.Chat}, ActionLog: {commanderror.ActionLog}");

                    var linkfilterCfg = gcfg.Linkfilter;
                    embed.AddField("Linkfilter", linkfilterCfg.Enable ? "Enabled" : "Disabled", true);
                    if (linkfilterCfg.Enable)
                    {
                        embed.AddField("Linkfilter Modules",
                            $"{(linkfilterCfg.BlockInviteLinks ? "✔" : "✖")}Anti-Invites, " +
                            $"{(linkfilterCfg.BlockBooters ? "✔" : "✖")}Anti-DDoS, " +
                            $"{(linkfilterCfg.BlockIpLoggers ? "✔" : "✖")}Anti-IP Loggers, " +
                            $"{(linkfilterCfg.BlockShockSites ? "✔" : "✖")}Anti-Shock Sites, " +
                            $"{(linkfilterCfg.BlockUrlShorteners ? "✔" : "✖")}Anti-URL Shorteners", true);

                        if (linkfilterCfg.ExemptRoleIds.Any())
                        {
                            var roles = linkfilterCfg.ExemptRoleIds
                                .Select(ctx.Guild.GetRole)
                                .Where(xr => xr != null)
                                .Select(xr => xr.Mention);

                            embed.AddField("Linkfilter-Exempt Roles", string.Join(", ", roles), true);
                        }

                        if (linkfilterCfg.ExemptUserIds.Any())
                        {
                            var users = linkfilterCfg.ExemptUserIds
                                .Select(xid => $"<@!{xid}>");

                            embed.AddField("Linkfilter-Exempt Users", string.Join(", ", users), true);
                        }

                        if (linkfilterCfg.ExemptInviteGuildIds.Any())
                        {
                            var guilds = string.Join(", ", linkfilterCfg.ExemptInviteGuildIds);

                            embed.AddField("Linkfilter-Exempt Invite Targets", guilds, true);
                        }
                    }

                    var roleCfg = gcfg.RoleState;
                    embed.AddField("Role State", roleCfg.Enable ? "Enabled" : "Disabled", true);
                    if (roleCfg.Enable)
                    {
                        if (roleCfg.IgnoredRoleIds.Any())
                        {
                            var roles = roleCfg.IgnoredRoleIds
                                .Select(ctx.Guild.GetRole)
                                .Where(xr => xr != null)
                                .Select(xr => xr.Mention);

                            embed.AddField("Role State-Ignored Roles", string.Join(", ", roles), true);
                        }

                        if (roleCfg.IgnoredChannelIds.Any())
                        {
                            var channels = roleCfg.IgnoredChannelIds
                                .Select(ctx.Guild.GetChannel)
                                .Where(xc => xc != null)
                                .Select(xc => xc.Mention);

                            embed.AddField("Role State-Ignored Channels", string.Join(", ", channels), true);
                        }
                    }

                    var inviscopCfg = gcfg.InvisiCop;
                    embed.AddField("InvisiCop", inviscopCfg.Enable ? "Enabled" : "Disabled", true);
                    if (inviscopCfg.Enable)
                    {
                        if (inviscopCfg.ExemptRoleIds.Any())
                        {
                            var roles = inviscopCfg.ExemptRoleIds
                                .Select(ctx.Guild.GetRole)
                                .Where(xr => xr != null)
                                .Select(xr => xr.Mention);

                            embed.AddField("InvisiCop-Exempt Roles", string.Join(", ", roles), true);
                        }

                        if (inviscopCfg.ExemptUserIds.Any())
                        {
                            var users = inviscopCfg.ExemptUserIds
                                .Select(xid => $"<@!{xid}>");

                            embed.AddField("InvisiCop-Exempt Users", string.Join(", ", users), true);
                        }
                    }
                    embed.AddField("Starboard",
                        gcfg.Starboard.Enable ? $"Enabled\nChannel: <#{gcfg.Starboard.ChannelId}>\nEmoji: {gcfg.Starboard.Emoji.EmojiName}" : "Disabled", true);

                    var suggestions = gcfg.SpellingHelperEnabled;
                    embed.AddField("Command Suggestions",
                        suggestions ? "Enabled" : "Disabled", true);

                    if (gcfg.SelfRoles.Any())
                    {
                        var roles = gcfg.SelfRoles
                            .Select(ctx.Guild.GetRole)
                            .Where(xr => xr != null)
                            .Select(xr => xr.Mention);

                        embed.AddField("Available Selfroles", string.Join(", ", roles), true);
                    }
                    await ctx.ElevatedRespondAsync(embed: embed.Build());
                });
        }

        [Command("reset"), Description("Resets this guild's configuration to initial state. This cannot be reversed.")]
        public async Task ResetAsync(CommandContext ctx)
        {
            using (var db = this.Database.CreateContext())
            {
                var cfg = db.GuildConfig.SingleOrDefault(xc => (ulong)xc.GuildId == ctx.Guild.Id);
                if (cfg == null)
                {
                    await ctx.SafeRespondUnformattedAsync("This guild is not configured.");
                    return;
                }

                var captcha = GetUniqueKey(32);

                /*await ctx.RespondWithFileAsync(
				    $"{GetUniqueKey(64)}.png",
				    new MemoryStream(CaptchaProvider.DrawCaptcha(captcha, "#99AAB5", "#23272A", 24, "sans-serif")),
				    "You are about to reset the configuration for this guild. **This change is irreversible.** " +
				    "Type the characters in the image to continue. You have 45 seconds.");*/

                await ctx.RespondAsync(captcha);

                var msg = await this.Interactivity
                    .WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id && xm.Content.ToUpperInvariant() == captcha, TimeSpan.FromSeconds(45));
                if (msg.Result == null)
                {
                    await ctx.SafeRespondUnformattedAsync("Operation aborted.");
                    return;
                }

                db.GuildConfig.Remove(cfg);
                await db.SaveChangesAsync();
            }

            await ctx.SafeRespondUnformattedAsync("Configuration reset.");
        }

        private string GetUniqueKey(int maxSize)
        {
            var chars = "ABCDEFGHMPRSTUVWXYZ23456789".ToCharArray();
            var data = new byte[1];

            RandomNumberProvider.GetNonZeroBytes(data);
            data = new byte[maxSize];
            RandomNumberProvider.GetNonZeroBytes(data);

            var result = new StringBuilder(maxSize);
            foreach (var b in data)
            {
                result.Append(chars[b % chars.Length]);
            }
            return result.ToString();
        }
    }
}