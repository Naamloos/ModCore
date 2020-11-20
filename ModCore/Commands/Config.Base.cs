﻿using System;
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

                    var actionlog = gcfg.ActionLog;
                    embed.AddField("Action Log",
                        actionlog.Enable
                            ? "Enabled" + (actionlog.WebhookId == 0 ? ", but not configured!" : "")
                            : "Disabled");

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

                    var joinlog = gcfg.JoinLog;
                    embed.AddField("Join Logging",
                        joinlog.Enable
                            ? "Enabled" + (joinlog.ChannelId == 0 ? ", but no channel configured!" : "")
                            : "Disabled", true);

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

        [Command("setup"), Description("Sets up this guild's config.")]
        [RequireBotPermissions(Permissions.ManageWebhooks | Permissions.ManageRoles | Permissions.ManageGuild)]
        public async Task SetupAsync(CommandContext ctx)
        {
            var hassettings = ctx.GetGuildSettings() != null;
            DiscordMessage t0;

            if (hassettings)
            {
                t0 = await ctx.SafeRespondUnformattedAsync(
                        "Welcome to ModCore! Looks like you already configured your guild." +
                        "Would you like to go through the setup again? (Y/N)");
            }
            else
            {
                t0 = await ctx.SafeRespondUnformattedAsync(
                        "Welcome to ModCore! This server has not been set up yet." +
                        "Would you like to go through the setup? (Y/N)");
            }

            var res = await Interactivity.WaitForMessageAsync(e => e.Author.Id == ctx.Message.Author.Id,
                    TimeSpan.FromSeconds(40));

            var message = res.TimedOut ? null : res.Result;

            if (!message.Content.EqualsIgnoreCase("y") &&
                !message.Content.EqualsIgnoreCase("yes") &&
                !message.Content.EqualsIgnoreCase("ya") &&
                !message.Content.EqualsIgnoreCase("ja") &&
                !message.Content.EqualsIgnoreCase("da"))
            {
                await ctx.SafeRespondUnformattedAsync(
                    "OK, I won't bother you anymore. Just execute this command again if you need help configuring.");
                await t0.DeleteAsync("modcore cleanup after itself: welcome message");
                await message.DeleteAsync(
                    "modcore cleanup after itself: user response to welcome message");
                return;
            }

            DiscordChannel channel;
            try
            {
                channel =
                    ctx.Guild.Channels.FirstOrDefault(e => e.Value.Name == "modcore-setup").Value ??
                    await ctx.Guild.CreateChannelAsync("modcore-setup", ChannelType.Text, reason: "modcore setup channel creation");
            }
            catch
            {
                await ctx.SafeRespondUnformattedAsync("Unfortunately, I wasn't able to create the modcore setup channel.\n" +
                                       "Could you kindly create a channel called `modcore-setup` and re-run the command?\n" +
                                       "I'll set up the rest for you. This will help keep the setup process away from prying eyes.");
                return;
            }
            await channel.AddOverwriteAsync(ctx.Guild.EveryoneRole, Permissions.None,
                Permissions.AccessChannels, "modcore overwrites for setup channel");
            await channel.AddOverwriteAsync(ctx.Member, Permissions.AccessChannels, Permissions.None,
                "modcore overwrites for setup channel");
            await channel.AddOverwriteAsync(ctx.Guild.CurrentMember, Permissions.AccessChannels, Permissions.None,
                "modcore overwrites for setup channel");

            await channel.ElevatedMessageAsync(
                "OK, now, can you create a webhook for ModCore and give me its URL?\n" +
                "If you don't know what that is, simply say no and I'll make one for you.");

            var res2 = await Interactivity.WaitForMessageAsync(e => e.Author.Id == ctx.Message.Author.Id,
                TimeSpan.FromSeconds(40));
            var message2 = res2.TimedOut ? null : res2.Result;

            var mContent = message2.Content;
            if (!mContent.Contains("discordapp.com/api/webhooks/"))
            {
                await channel.ElevatedMessageAsync("Alright, I'll make a webhook for you then. Sit tight...");
                DiscordChannel logChannel;
                try
                {
                    logChannel =
                        ctx.Guild.Channels.FirstOrDefault(e => e.Value.Name == "modlog").Value ??
                        await ctx.Guild.CreateChannelAsync("modlog", ChannelType.Text, reason: "ModCore Logging channel.");
                }
                catch
                {
                    await ctx.SafeRespondUnformattedAsync("Unfortunately, I wasn't able to create the modcore logging channel.\n" +
                                           "Could you kindly create a channel called `modlog` and re-run the command?\n" +
                                           "I'll set up the rest for you. This is to setup a channel for the actionlog to post into.");
                    return;
                }

                var webhook = await logChannel.CreateWebhookAsync("ModCore Logging", reason: "Created webhook to post log messages");

                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.ActionLog.WebhookId = webhook.Id;
                cfg.ActionLog.WebhookToken = webhook.Token;
                await ctx.SetGuildSettingsAsync(cfg);
            }
            else
            {
                var tokens = mContent
                    .Substring(mContent.IndexOfInvariant("/api/webhooks/") + "/api/webhooks/".Length)
                    .Split('/');

                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.ActionLog.WebhookId = ulong.Parse(tokens[0]);
                cfg.ActionLog.WebhookToken = tokens[1];
                await ctx.SetGuildSettingsAsync(cfg);
            }
            await channel.ElevatedMessageAsync(
                    "Webhook configured. Looks like you're all set! ModCore has been set up." +
                    "\nThis channel will be deleted in 30 seconds...");

            await Task.Delay(TimeSpan.FromSeconds(30));

            try
            {
                await channel.DeleteAsync();
            }
            catch (Exception)
            {
                await channel.ElevatedMessageAsync("Failed to delete the channel.\nPlease try to do so yourself.");
            }
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