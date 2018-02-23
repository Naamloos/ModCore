﻿using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;

namespace ModCore.Commands
{
    [Group("config", CanInvokeWithoutSubcommand = true)]
    [Aliases("cfg")]
    [Description("Guild configuration options. Invoking without a subcommand will list current guild's settings.")]
    [RequirePermissions(Permissions.ManageGuild)]
    public class Config
    {
        public static DiscordEmoji CheckMark { get; } = DiscordEmoji.FromUnicode("✅");

        public DatabaseContextBuilder Database { get; }
        public InteractivityExtension Interactivity { get; }

        public Config(DatabaseContextBuilder db, InteractivityExtension interactive)
        {
            this.Database = db;
            this.Interactivity = interactive;
        }

        [Description("Starts the ModCore configuration wizard. You probably want to do this first!")]
        public async Task ExecuteGroupAsync(CommandContext ctx)
        {
            await ctx.IfGuildSettingsAsync(async () =>
            {
                var t0 = await ctx.SafeRespondAsync(
                    "Welcome to ModCore! Looks like you haven't configured your guild yet." +
                    "Would you like to go through a quick setup? (Y/N)");

                var message = await Interactivity.WaitForMessageAsync(e => e.Author.Id == ctx.Message.Author.Id,
                    TimeSpan.FromSeconds(40));
                if (!message.Message.Content.EqualsIgnoreCase("y") &&
                    !message.Message.Content.EqualsIgnoreCase("yes") &&
                    !message.Message.Content.EqualsIgnoreCase("ya") &&
                    !message.Message.Content.EqualsIgnoreCase("ja") &&
                    !message.Message.Content.EqualsIgnoreCase("da"))
                {
                    await ctx.SafeRespondAsync(
                        "OK, I won't bother you anymore. Just execute this command again if you need help configuring.");
                    await t0.DeleteAsync("modcore cleanup after itself: welcome message");
                    await message.Message.DeleteAsync(
                        "modcore cleanup after itself: user response to welcome message");
                    return;
                }

                DiscordChannel channel;
                try
                {
                    channel =
                        ctx.Guild.Channels.FirstOrDefault(e => e.Name == "modcore-setup") ??
                        await ctx.Guild.CreateChannelAsync("modcore-setup", ChannelType.Text, null, null, null,
                            null, null, "modcore setup channel creation");
                }
                catch
                {
                    await ctx.SafeRespondAsync("Unfortunately, I wasn't able to create the modcore setup channel.\n" +
                                           "Could you kindly create a channel called `modcore-setup` and re-run the command?\n" +
                                           "I'll set up the rest for you. This will help keep the setup process away from prying eyes.");
                    return;
                }
                await channel.AddOverwriteAsync(ctx.Guild.EveryoneRole, Permissions.None,
                    Permissions.AccessChannels, "modcore overwrites for setup channel");
                await channel.AddOverwriteAsync(ctx.Member, Permissions.AccessChannels, Permissions.None,
                    "modcore overwrites for setup channel");

                await channel.ElevatedMessageAsync(
                    "OK, now, can you create a webhook for ModCore and give me its URL?\n" +
                    "If you don't know what that is, simply say no and I'll make one for you.");

                var message2 = await Interactivity.WaitForMessageAsync(e => e.Author.Id == ctx.Message.Author.Id,
                    TimeSpan.FromSeconds(40));
                var mContent = message2.Message.Content;
                if (!mContent.Contains("discordapp.com/api/webhooks/"))
                {
                    await channel.ElevatedMessageAsync("Alright, I'll make a webhook for you then. Sit tight...");
                    DiscordChannel logChannel;
                    try
                    {
                        logChannel =
                            ctx.Guild.Channels.FirstOrDefault(e => e.Name == "modlog") ??
                            await ctx.Guild.CreateChannelAsync("modlog", ChannelType.Text, null, null, null,
                                null, null, "ModCore Logging channel.");
                    }
                    catch
                    {
                        await ctx.SafeRespondAsync("Unfortunately, I wasn't able to create the modcore logging channel.\n" +
                                               "Could you kindly create a channel called `modlog` and re-run the command?\n" +
                                               "I'll set up the rest for you. This is to setup a channel for the actionlog to post into.");
                        return;
                    }

                    DiscordWebhook webhook = await logChannel.CreateWebhookAsync("ModCore Logging", null, "Created webhook to post log messages");

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
                await ctx.SafeRespondAsync(
                        "Webhook configured. Looks like you're all set! ModCore has been set up.");

            },
                async gcfg =>
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = ctx.Guild.Name,
                        ThumbnailUrl = ctx.Guild.IconUrl,
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

                    var globalwarn = gcfg.GlobalWarn;
                    embed.AddField("GlobalWarn",
                        globalwarn.Enable
                            ? "Enabled\nMode: " + globalwarn.WarnLevel
                            : "Disabled", true);

                    var messageLog = gcfg.MessageLog;
                    embed.AddField(@"Message Deletion Logging",
                        messageLog.Enable
                            ? "Enabled\nMode: \n" + messageLog.LogLevel + "\n" + (messageLog.ChannelId == 0 ? ", but no channel configured!" : 
                            "\nChannel: " + ctx.Guild.GetChannel((ulong)messageLog.ChannelId).Name)
                            : "Disabled"
                            , true);

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
                    await ctx.SafeRespondAsync("This guild is not configured.");
                    return;
                }

                var nums = new byte[8];
                using (var rng = RandomNumberGenerator.Create())
                    rng.GetBytes(nums);
                var numss = string.Join(", ", nums);
                var numst = string.Join(" ", nums.Reverse());

                await ctx.SafeRespondAsync(
                    $"You are about to reset the configuration for this guild. To confirm, type these numbers in reverse order, using single space as separator: {numss}. You have 45 seconds.");
                var iv = ctx.Client.GetInteractivity();
                var msg = await iv.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id && xm.Content == numst,
                    TimeSpan.FromSeconds(45));
                if (msg == null)
                {
                    await ctx.SafeRespondAsync("Operation aborted.");
                    return;
                }

                db.GuildConfig.Remove(cfg);
                await db.SaveChangesAsync();
            }

            await ctx.SafeRespondAsync("Configuration reset.");
        }

        [Command("prefix"), Aliases("pfix"),
         Description("Sets the command prefix for this guild. Prefixes longer than 10 characters will be truncated.")]
        public async Task PrefixAsync(CommandContext ctx,
            [Description("New command prefix for this guild")] string prefix = null)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                prefix = null;

            prefix = prefix?.TrimStart();
            if (prefix?.Length > 20)
                prefix = prefix.Substring(0, 21);

            await ctx.WithGuildSettings(cfg => { cfg.Prefix = prefix; });

            await ctx.SafeRespondAsync(prefix == null
                ? "Prefix restored to default."
                : $"Prefix changed to: \"{prefix}\".");
        }

        [Group("suggestions"), Aliases("suggestion", "sugg", "sug", "s"), Description("Suggestions configuration commands.")]
        public class Suggestions
        {
            [Command("enable"), Aliases("on"), Description("Enables command suggestions for this guild.")]
            public async Task EnableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.SpellingHelperEnabled = true;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync("Enabled command suggestions.");
            }

            [Command("disable"), Aliases("off"), Description("Disables command suggestions for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.SpellingHelperEnabled = true;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync("Disabled command suggestions.");
            }
        }

        [Command("muterole"), Aliases("mr"),
         Description("Sets the role used to mute users. Invoking with no arguments will reset this setting.")]
        public async Task MuteRole(CommandContext ctx,
            [Description("New mute role for this guild")] DiscordRole role = null)
        {
            await ctx.WithGuildSettings(cfg => cfg.MuteRoleId = role == null ? 0 : role.Id);

            await ctx.SafeRespondAsync(role == null ? "Muting disabled." : $"Mute role set to {role.Mention}.");
        }


        [Group("linkfilter"), Aliases("inviteblocker", "invite", "ib", "filter", "lf"),
         Description("Linkfilter configuration commands.")]
        public class Linkfilter
        {
            [Command("enable"), Aliases("on"), Description("Enables Linkfilter for this guild.")]
            public async Task EnableAsync(CommandContext ctx)
            {
                await ctx.WithGuildSettings(cfg => cfg.Linkfilter.Enable = true);
                await ctx.Message.CreateReactionAsync(CheckMark);
            }

            [Command("disable"), Aliases("off"), Description("Disables Linkfilter for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                await ctx.WithGuildSettings(cfg => cfg.Linkfilter.Enable = false);
                await ctx.Message.CreateReactionAsync(CheckMark);
            }

            [Group("modules"), Aliases("mod", "m", "s"),
             Description("Commands to toggle Linkfilter modules for this guild.")]
            public class Modules
            {
                private delegate ref bool WithLinkfilter(GuildLinkfilterSettings lf);

                private static async Task Toggle(CommandContext ctx, string r, WithLinkfilter func)
                {
                    await ctx.WithGuildSettings(cfg =>
                    {
                        ref var cv = ref func(cfg.Linkfilter);
                        
                        if (AugmentedBoolConverter.TryConvert(r, ctx, out var b)) cv = b;
                        else cv = !cv;
                        
                        return ctx.ElevatedRespondAsync($"{(cv ? "Enabled" : "Disabled")} this module.");
                    });
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }

                [Group("all"), Aliases("a", "0"), Description("Commands to manage all Linkfilter modules at once.")]
                public class AllModules
                {
                    [Command("off"), Aliases("disable", "disabled", "0", "false", "no", "n"),
                     Description("Disables all Linkfilter modules for this guild.")]
                    public async Task DisableAllLinkfilterModulesAsync(CommandContext ctx)
                    {
                        await ctx.WithGuildSettings(cfg =>
                        {
                            cfg.Linkfilter.BlockBooters = false;
                            cfg.Linkfilter.BlockInviteLinks = false;
                            cfg.Linkfilter.BlockIpLoggers = false;
                            cfg.Linkfilter.BlockShockSites = false;
                            cfg.Linkfilter.BlockUrlShorteners = false;
                        });
                        await ctx.Message.CreateReactionAsync(CheckMark);
                    }

                    [Command("on"), Aliases("enable", "enabled", "1", "true", "yes", "y"),
                     Description("Enables all Linkfilter modules for this guild.")]
                    public async Task EnableAllLinkfilterModulesAsync(CommandContext ctx)
                    {
                        await ctx.WithGuildSettings(cfg =>
                        {
                            cfg.Linkfilter.BlockBooters = true;
                            cfg.Linkfilter.BlockInviteLinks = true;
                            cfg.Linkfilter.BlockIpLoggers = true;
                            cfg.Linkfilter.BlockShockSites = true;
                            cfg.Linkfilter.BlockUrlShorteners = true;
                        });
                        await ctx.Message.CreateReactionAsync(CheckMark);
                    }

                    [Command("default"), Aliases("def", "d", "2"),
                     Description("Sets all Linkfilter modules to default for this guild.")]
                    public async Task RestoreDefaultAllLinkfilterModulesAsync(CommandContext ctx)
                    {
                        await ctx.WithGuildSettings(cfg =>
                        {
                            cfg.Linkfilter.BlockBooters = true;
                            cfg.Linkfilter.BlockInviteLinks = true;
                            cfg.Linkfilter.BlockIpLoggers = true;
                            cfg.Linkfilter.BlockShockSites = true;
                            cfg.Linkfilter.BlockUrlShorteners = false;
                        });
                        await ctx.Message.CreateReactionAsync(CheckMark);
                    }
                }

                private const string ToggleDesc = "Leave empty to toggle, " +
                                                  "set to one of `on`, `enable`, `enabled`, `1`, `true`, `yes` or `y` to enable, " +
                                                  "or set to one of `off`, `disable`, `disabled`, `0`, `false`, `no` or `n` to disable. "
                    ;

                [Command("booters"), Aliases("booter", "boot", "ddos", "b", "1"),
                 Description("Toggle blocking booter/DDoS sites for this guild.")]
                public async Task ToggleBlockBootersAsync(CommandContext ctx, [Description(ToggleDesc)] string r = "")
                    => await Toggle(ctx, r, lf => ref lf.BlockBooters);

                [Command("invites"), Aliases("invitelinks", "invite", "inv", "i", "2"),
                 Description("Toggle blocking invite links for this guild.")]
                public async Task ToggleBlockInviteLinksAsync(CommandContext ctx,
                    [Description(ToggleDesc)] string r = "")
                    => await Toggle(ctx, r, lf => ref lf.BlockInviteLinks);

                [Command("iploggers"), Aliases("iplogs", "ips", "ip", "3"),
                 Description("Toggle blocking IP logger sites for this guild.")]
                public async Task ToggleBlockIpLoggersAsync(CommandContext ctx, [Description(ToggleDesc)] string r = "")
                    => await Toggle(ctx, r, lf => ref lf.BlockIpLoggers);

                [Command("shocksites"), Aliases("shock", "shocks", "gore", "g", "4"),
                 Description("Toggle blocking shock/gore sites for this guild.")]
                public async Task ToggleBlockShockSitesAsync(CommandContext ctx,
                    [Description(ToggleDesc)] string r = "")
                    => await Toggle(ctx, r, lf => ref lf.BlockShockSites);

                [Command("urlshortener"), Aliases("urlshorteners", "urlshort", "urls", "url", "u", "5"),
                 Description("Toggle blocking URL shortener links for this guild.")]
                public async Task ToggleBlockUrlShortenersAsync(CommandContext ctx,
                    [Description(ToggleDesc)] string r = "")
                    => await Toggle(ctx, r, lf => ref lf.BlockUrlShorteners);
            }

            [Group("user"), Aliases("usr", "u"), Description("User exemption management commands.")]
            public class User
            {
                [Command("exempt"), Aliases("x"), Description("Exempts user from Linkfilter checks.")]
                public async Task ExemptAsync(CommandContext ctx,
                    [RemainingText, Description("Member to exempt from Linkfilter checks")] DiscordMember mbr)
                {
                    await ctx.WithGuildSettings(cfg =>
                    {
                        var ibx = cfg.Linkfilter.ExemptUserIds;
                        if (!ibx.Contains(mbr.Id))
                            ibx.Add(mbr.Id);
                    });
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }

                [Command("unexempt"), Aliases("ux"), Description("Unexempts user from Linkfilter checks.")]
                public async Task UnexemptAsync(CommandContext ctx,
                    [RemainingText, Description("Member to unexempt from Linkfilter checks")] DiscordMember mbr)
                {
                    await ctx.WithGuildSettings(cfg =>
                    {
                        var ibx = cfg.Linkfilter.ExemptUserIds;
                        if (ibx.Contains(mbr.Id))
                            ibx.Remove(mbr.Id);
                    });
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }
            }

            [Group("role"), Aliases("r"), Description("Role exemption management commands.")]
            public class Role
            {
                [Command("exempt"), Aliases("x"), Description("Exempts role from Linkfilter checks.")]
                public async Task ExemptAsync(CommandContext ctx,
                    [RemainingText, Description("Role to exempt from Linkfilter checks")] DiscordRole rl)
                {
                    await ctx.WithGuildSettings(cfg =>
                    {
                        var ibx = cfg.Linkfilter.ExemptRoleIds;
                        if (!ibx.Contains(rl.Id))
                            ibx.Add(rl.Id);
                    });
                }

                [Command("unexempt"), Aliases("ux"), Description("Unexempts role from Linkfilter checks.")]
                public async Task UnexemptAsync(CommandContext ctx,
                    [RemainingText, Description("Role to unexempt from Linkfilter checks")] DiscordRole rl)
                {
                    await ctx.WithGuildSettings(cfg =>
                    {
                        var ibx = cfg.Linkfilter.ExemptRoleIds;
                        if (ibx.Contains(rl.Id))
                            ibx.Remove(rl.Id);
                    });
                }
            }

            [Group("guild"), Aliases("invite", "i"), Description("Invite target exemption management commands.")]
            public class Guild
            {
                [Command("exempt"), Aliases("x"), Description("Exempts code from invite checks.")]
                public async Task ExemptAsync(CommandContext ctx,
                    [RemainingText, Description("Invite code to exempt from invite checks")] string invite)
                {
                    var inv = await ctx.Client.GetInviteByCodeAsync(invite);
                    if (inv == null)
                    {
                        await ctx.SafeRespondAsync("Invite seems to be invalid. Maybe the bot is banned.");
                        return;
                    }

                    await ctx.WithGuildSettings(cfg =>
                    {
                        var ibx = cfg.Linkfilter.ExemptInviteGuildIds;
                        if (!ibx.Contains(inv.Guild.Id))
                            ibx.Add(inv.Guild.Id);
                    });
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }

                [Command("unexempt"), Aliases("ux"), Description("Unexempts code from invite checks.")]
                public async Task UnexemptAsync(CommandContext ctx,
                    [RemainingText, Description("Invite code to unexempt from invite checks")] string invite)
                {
                    var inv = await ctx.Client.GetInviteByCodeAsync(invite);
                    if (inv == null)
                    {
                        await ctx.SafeRespondAsync("Invite seems to be invalid. Maybe the bot is banned.");
                        return;
                    }

                    await ctx.WithGuildSettings(cfg =>
                    {
                        var ibx = cfg.Linkfilter.ExemptInviteGuildIds;
                        if (ibx.Contains(inv.Guild.Id))
                            ibx.Remove(inv.Guild.Id);
                    });
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }
            }
        }

        [Group("rolestate"), Aliases("rs"), Description("Role State configuration commands.")]
        public class RoleState
        {
            [Command("enable"), Aliases("on"), Description("Enables Role State for this guild.")]
            public async Task EnableAsync(CommandContext ctx)
            {
                await ctx.WithGuildSettings(cfg => cfg.RoleState.Enable = true);
                await ctx.SafeRespondAsync("Role State enabled.");
            }

            [Command("disable"), Aliases("off"), Description("Disables Role State for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                await ctx.WithGuildSettings(cfg => cfg.RoleState.Enable = false);
                await ctx.SafeRespondAsync("Role State disabled.");
            }

            [Group("role"), Aliases("r"), Description("Role exemption management commands.")]
            public class Role
            {
                [Command("ignore"), Aliases("x"), Description("Exempts role from being saved by Role State.")]
                public async Task ExemptAsync(CommandContext ctx,
                    [RemainingText, Description("Role to exempt from being saved")] DiscordRole rl)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.RoleState.IgnoredRoleIds;
                    if (!ibx.Contains(rl.Id))
                        ibx.Add(rl.Id);
                    await ctx.SetGuildSettingsAsync(cfg);
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }

                [Command("unignore"), Aliases("ux"), Description("Unexempts role from being saved by Role State.")]
                public async Task UnexemptAsync(CommandContext ctx,
                    [RemainingText, Description("Role to unexempt from being saved")] DiscordRole rl)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.RoleState.IgnoredRoleIds;
                    if (ibx.Contains(rl.Id))
                        ibx.Remove(rl.Id);
                    await ctx.SetGuildSettingsAsync(cfg);
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }
            }

            [Group("channel"), Aliases("c"), Description("Channel exemption management commands.")]
            public class Channel
            {
                private DatabaseContextBuilder Database { get; }

                public Channel(DatabaseContextBuilder db)
                {
                    this.Database = db;
                }

                [Command("ignore"), Aliases("x"),
                 Description("Exempts channel from having its overrides saved by Role State.")]
                public async Task ExemptAsync(CommandContext ctx,
                    [RemainingText, Description("Channel to exempt from having its invites saved")] DiscordChannel chn)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.RoleState.IgnoredChannelIds;
                    if (!ibx.Contains(chn.Id))
                        ibx.Add(chn.Id);
                    await ctx.SetGuildSettingsAsync(cfg);

                    using (var db = this.Database.CreateContext())
                    {
                        var chperms = db.RolestateOverrides.Where(xs =>
                            xs.ChannelId == (long)chn.Id && xs.GuildId == (long)chn.Guild.Id);
                        if (chperms.Any())
                        {
                            db.RolestateOverrides.RemoveRange(chperms);
                            await db.SaveChangesAsync();
                        }
                    }

                    await ctx.Message.CreateReactionAsync(CheckMark);
                }

                [Command("unignore"), Aliases("ux"),
                 Description("Unexempts channel from having its overrides saved by Role State.")]
                public async Task UnexemptAsync(CommandContext ctx,
                    [RemainingText, Description("Channel to unexempt from having its invites saved")]
                    DiscordChannel chn)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.RoleState.IgnoredChannelIds;
                    if (ibx.Contains(chn.Id))
                        ibx.Remove(chn.Id);
                    await ctx.SetGuildSettingsAsync(cfg);

                    var os = chn.PermissionOverwrites.Where(xo => xo.Type.ToString().ToLower() == "member").ToArray();
                    using (var db = this.Database.CreateContext())
                    {
                        if (os.Any())
                        {
                            await db.RolestateOverrides.AddRangeAsync(os.Select(xo => new DatabaseRolestateOverride
                            {
                                ChannelId = (long)chn.Id,
                                GuildId = (long)chn.Guild.Id,
                                MemberId = (long)xo.Id,
                                PermsAllow = (long)xo.Allow,
                                PermsDeny = (long)xo.Deny
                            }));
                            await db.SaveChangesAsync();
                        }
                    }

                    await ctx.Message.CreateReactionAsync(CheckMark);
                }
            }
        }

        [Group("invisicop"), Aliases("ic"), Description("InvisiCop configuration commands.")]
        public class InvisiCop
        {
            [Command("enable"), Aliases("on"), Description("Enables InvisiCop for this guild.")]
            public async Task EnableAsync(CommandContext ctx)
            {
                await ctx.WithGuildSettings(cfg => cfg.InvisiCop.Enable = true);
                await ctx.SafeRespondAsync("InvisiCop enabled.");
            }

            [Command("disable"), Aliases("off"), Description("Disables InvisiCop for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                await ctx.WithGuildSettings(cfg => cfg.InvisiCop.Enable = false);
                await ctx.SafeRespondAsync("InvisiCop disabled.");
            }
        }

        [Group("actionlog"), Aliases("al"), Description("ActionLog configuration commands.")]
        public class ActionLog
        {
            [Command("enable"), Aliases("on"), Description("Enables ActionLog for this guild.")]
            public async Task EnableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.ActionLog.Enable = true;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync(
                    $"ActionLog enabled.\nIf you haven't done this yet, Please execute `{cfg.Prefix}config actionlog setwebhook`");
            }

            [Command("disable"), Aliases("off"), Description("Disables ActionLog for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.ActionLog.Enable = false;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync("ActionLog disabled.");
            }

            [Command("setwebhook"), Aliases("swh"),
             Description("Sets the webhook ID and token for this guild's action log.")]
            public async Task SetWebhookAsync(CommandContext ctx, [Description("Webhook ID")]ulong id, [Description("Webhook token")]string token)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.ActionLog.WebhookId = id;
                cfg.ActionLog.WebhookToken = token;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync("ActionLog webhook configured.");
            }
        }

        [Group("autorole"), Aliases("ar"), Description("AutoRole configuration commands.")]
        public class AutoRole
        {
            [Command("enable"), Aliases("on"), Description("Enables AutoRole for this guild.")]
            public async Task EnableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.AutoRole.Enable = true;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync(
                    $"AutoRole enabled.\nIf you haven't done this yet, Please execute `{cfg.Prefix}config autorole setrole`");
            }

            [Command("disable"), Aliases("off"), Description("Disables AutoRole for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.AutoRole.Enable = false;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync("AutoRole disabled.");
            }

            [Command("setrole"), Aliases("sr"),
             Description("Sets a role to grant to new members.")]
            public async Task SetRoleAsync(CommandContext ctx, [Description("Role to grant to new members")]DiscordRole role)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.AutoRole.RoleId = (long)role.Id;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync("AutoRole role configured.");
            }
        }

        [Group("error"), Aliases("er"), Description("Error verbosity configuration commands.")]
        public class ErrorVerbosity
        {
            [Command("chat"), Aliases("c"), Description("Sets command error reporting for this guild (in chat).")]
            public async Task ChatAsync(CommandContext ctx, [Description("New error verbosity")]string verbosity)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                var vb = CommandErrorVerbosity.None;
                switch (verbosity)
                {
                    case "none":
                        vb = CommandErrorVerbosity.None;
                        break;
                    case "name":
                        vb = CommandErrorVerbosity.Name;
                        break;
                    case "namedesc":
                        vb = CommandErrorVerbosity.NameDesc;
                        break;
                    case "exception":
                        vb = CommandErrorVerbosity.Exception;
                        break;
                    default:
                        await ctx.SafeRespondAsync(
                            "Unsupported verbosity level.\nSupported levels: `none`, `name`, `namedesc` or `exception`");
                        return;
                }
                cfg.CommandError.Chat = vb;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync($"Error reporting verbosity in chat set to `{verbosity}`.");
            }

            [Command("log"), Aliases("a"), Description("Sets command error reporting for this guild (in ActionLog).")]
            public async Task ActionLogAsync(CommandContext ctx, [Description("New error verbosity")]string verbosity)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                var vb = CommandErrorVerbosity.None;
                switch (verbosity)
                {
                    case "none":
                        vb = CommandErrorVerbosity.None;
                        break;
                    case "name":
                        vb = CommandErrorVerbosity.Name;
                        break;
                    case "namedesc":
                        vb = CommandErrorVerbosity.NameDesc;
                        break;
                    case "exception":
                        if (ctx.Client.CurrentApplication.Owner.Id == ctx.Member.Id)
                            vb = CommandErrorVerbosity.Exception;
                        else
                            await ctx.SafeRespondAsync(
                                "Unsupported verbosity level.\nSupported levels: `none`, `name`, `namedesc` or `exception`");
                        break;
                    default:
                        await ctx.SafeRespondAsync(
                            "Unsupported verbosity level.\nSupported levels: `none`, `name`" 
                            + ((ctx.Client.CurrentApplication.Owner.Id == ctx.Member.Id)? ", `namedesc` or `exception`" : "or`namedesc`"));
                        return;
                }
                cfg.CommandError.ActionLog = vb;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync($"Error reporting verbosity in ActionLog set to `{verbosity}`.");
            }
        }

        [Group("joinlog"), Aliases("j"), Description("JoinLog configuration commands.")]
        public class JoinLog
        {
            [Command("enable"), Aliases("on"), Description("Enables JoinLog for this guild.")]
            public async Task EnableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.JoinLog.Enable = true;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync(
                    $"Joinlog enabled.\nIf you haven't done this yet, Please execute `{cfg.Prefix}config joinlog setchannel`");
            }

            [Command("disable"), Aliases("off"), Description("Disables JoinLog for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.JoinLog.Enable = false;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync("JoinLog disabled.");
            }

            [Command("setchannel"), Aliases("sc"), Description("Sets the channel ID for this guild's JoinLog.")]
            public async Task SetChannelAsync(CommandContext ctx, [Description("Channel to send the JoinLogs to")]DiscordChannel channel)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.JoinLog.ChannelId = (long)channel.Id;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync("JoinLog channel configured.");
            }
        }

        [Group("selfrole"), Aliases("sr"), Description("SelfRole configuration commands.")]
        public class SelfRole
        {
            [Command("add"), Aliases("a"), Description("Adds roles to SelfRole list")]
            public async Task AddSelfRoleAsync(CommandContext ctx, [Description("Role to allow for self-granting")]DiscordRole role)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                if (!cfg.SelfRoles.Contains(role.Id))
                {
                    cfg.SelfRoles.Add(role.Id);
                }
                else
                {
                    await ctx.SafeRespondAsync("This role has already been added!");
                    return;
                }
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync($"Added role `{role.Name}` with ID `{role.Id}` to SelfRoles.");
            }

            [Command("remove"), Aliases("r"), Description("Removes roles from SelfRole list")]
            public async Task RemoveSelfRoleAsync(CommandContext ctx, [Description("Role to disallow from self-granting")]DiscordRole role)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                if (cfg.SelfRoles.Contains(role.Id))
                {
                    cfg.SelfRoles.Remove(role.Id);
                }
                else
                {
                    await ctx.SafeRespondAsync("This role isn't in SelfRoles!");
                    return;
                }
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync($"Removed role `{role.Name}` with ID `{role.Id}` from SelfRoles.");
            }
        }

        [Group("starboard"), Aliases("star"), Description("Starboard configuration commands.")]
        public class Starboard
        {
            [Command("enable"), Aliases("on"), Description("Enables Starboard for this guild.")]
            public async Task EnableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.Starboard.Enable = true;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync(
                    $"Starboard enabled.\nIf you haven't done this yet, Please execute `{cfg.Prefix}config starboard setchannel`");
            }

            [Command("disable"), Aliases("off"), Description("Disables Starboard for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.Starboard.Enable = false;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync("Starboard disabled.");
            }

            [Command("allownsfw"), Aliases("nsfw"), Description("Sets whether or not to allow NSFW stars in this guild.")]
            public async Task AllowNsfwAsync(CommandContext ctx, [Description("Whether NSFW stars should be allowed")]bool allow)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.Starboard.AllowNSFW = allow;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync($"Set allow NSFW to: {allow}.");
            }

            [Command("setchannel"), Aliases("sc"), Description("Sets the channel ID for this guild's Starboard.")]
            public async Task SetChannelAsync(CommandContext ctx, [Description("Channel to log stars to")]DiscordChannel channel)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.Starboard.ChannelId = (long)channel.Id;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync("Starboard channel configured.");
            }

            [Command("setemoji"), Aliases("se"), Description("Sets the Starboard emoji for this guild.")]
            public async Task SetEmojiAsync(CommandContext ctx, [Description("Starboard emoji")]DiscordEmoji emoji)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.Starboard.Emoji = new GuildStarboardEmoji { EmojiId = (long)emoji.Id, EmojiName = emoji.Name };
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync($"Starboard emoji set to {emoji}.");
            }
        }

        [Group("globalwarn"), Aliases("gw"), Description("GlobalWarn configuration commands.")]
        public class GlobalWarn
        {
            [Command("enable"), Aliases("on"), Description("Enables GlobalWarn for this guild.")]
            public async Task EnableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.GlobalWarn.Enable = true;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync("GlobalWarn enabled.");
            }

            [Command("disable"), Aliases("off"), Description("Disables GlobalWarn for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.GlobalWarn.Enable = false;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync("GlobalWarn disabled.");
            }

            [Command("changemode"), Aliases("cm"),
             Description("Sets the GlobalWarn mode.")]
            public async Task SetRoleAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                await ctx.SafeRespondAsync("__**Available GlobalWarn modes:**__\n1. None\n2. Owner (Warns the Server Owner if someone on the GlobalWarn list joins the server)\n3. Joinlog (Sends the warning to the JoinLog channel)\n\nType an option.");
                var iv = ctx.Client.GetInteractivity();

                var msg = await iv.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id &&
                (xm.Content.ToLower() == "none" || xm.Content.ToLower() == "owner" || xm.Content.ToLower() == "joinlog" || xm.Content == "1" || xm.Content == "2" || xm.Content == "3"),
                TimeSpan.FromSeconds(45));
                if (msg == null)
                {
                    await ctx.SafeRespondAsync("Operation aborted.");
                    return;
                }
                switch (msg.Message.Content)
                {
                    case "none":
                    case "1":
                        cfg.GlobalWarn.WarnLevel = GlobalWarnLevel.None;
                        break;
                    case "owner":
                    case "2":
                        cfg.GlobalWarn.WarnLevel = GlobalWarnLevel.Owner;
                        break;
                    case "joinlog":
                    case "3":
                        cfg.GlobalWarn.WarnLevel = GlobalWarnLevel.JoinLog;
                        break;
                }
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync("GlobalWarn mode configured to " + cfg.GlobalWarn.WarnLevel);
            }
        }

        [Group("messagelog"), Aliases("ml"), Description("MessageLog configuration commands.")]
        public class MessageLog
        {
            [Command("enable"), Aliases("on"), Description("Enables MessageLog for this guild.")]
            public async Task EnableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.MessageLog.Enable = true;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync("MessageLog enabled.");
            }

            [Command("disable"), Aliases("off"), Description("Disables MessageLog for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.MessageLog.Enable = false;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync("MessageLog disabled.");
            }

            [Command("changemode"), Aliases("cm"),
             Description("Sets the MessageLog mode.")]
            public async Task SetRoleAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                await ctx.SafeRespondAsync("__**Available MessageLog modes:**__\n1. None\n2. Delete (Logs any message deletions in the specified channel)\n3. Edit (Logs both message deletions and message edits in the specified channel)\n\nType an option.");
                var iv = ctx.Client.GetInteractivity();

                var msg = await iv.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id &&
                (xm.Content.ToLower() == "none" || xm.Content.ToLower() == "delete" || xm.Content.ToLower() == "edit" || xm.Content == "1" || xm.Content == "2" || xm.Content == "3"),
                TimeSpan.FromSeconds(45));
                if (msg == null)
                {
                    await ctx.SafeRespondAsync("Operation aborted.");
                    return;
                }
                switch (msg.Message.Content)
                {
                    case "none":
                    case "1":
                        cfg.MessageLog.LogLevel = MessageLogLevel.None;
                        break;
                    case "delete":
                    case "2":
                        cfg.MessageLog.LogLevel = MessageLogLevel.Delete;
                        break;
                    case "edit":
                    case "3":
                        cfg.MessageLog.LogLevel = MessageLogLevel.Edit;
                        break;
                }
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync("MessageLog mode configured to " + cfg.MessageLog.LogLevel);
            }

            [Command("setchannel"), Aliases("sc"), Description("Sets the channel ID for the MessageLog.")]
            public async Task SetChannelAsync(CommandContext ctx, [Description("Channel to log Deleted/Edited messages to.")]DiscordChannel channel)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.MessageLog.ChannelId = (long)channel.Id;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.SafeRespondAsync("MessageLog channel configured.");
            }
        }
    }
}