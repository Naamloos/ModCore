using System;
using System.Collections.Generic;
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

        public async Task ExecuteGroupAsync(CommandContext ctx)
        {
            await ctx.IfGuildSettingsAsync(async () =>
                {
                    var t0 = await ctx.RespondAsync(
                        "Welcome to ModCore! Looks like you haven't configured your guild yet." +
                        "Would you like to go through a quick setup? (Y/N)");

                    var message = await Interactivity.WaitForMessageAsync(e => e.Author.Id == ctx.Message.Author.Id,
                        TimeSpan.FromSeconds(40));
                    if (!message.Message.Content.EqualsIgnoreCase("y") &&
                        !message.Message.Content.EqualsIgnoreCase("yes"))
                    {
                        await ctx.RespondAsync(
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
                        await ctx.RespondAsync("Unfortunately, I wasn't able to create the modcore setup channel.\n" +
                                               "Could you kindly create a channel called `modcore-setup` and re-run the command?\n" +
                                               "I'll set up the rest for you. This will help keep the setup process away from prying eyes.");
                        return;
                    }
                    await channel.AddOverwriteAsync(ctx.Guild.EveryoneRole, Permissions.None,
                        Permissions.AccessChannels, "modcore overwrites for setup channel");
                    await channel.AddOverwriteAsync(ctx.Member, Permissions.AccessChannels, Permissions.None,
                        "modcore overwrites for setup channel");

                    await channel.SendMessageAsync(
                        "OK, now, can you create a webhook for ModCore and give me its URL?\n" +
                        "If you don't know what that is, simply say no and I'll make one for you.");

                    var message2 = await Interactivity.WaitForMessageAsync(e => e.Author.Id == ctx.Message.Author.Id,
                        TimeSpan.FromSeconds(40));
                    var mContent = message2.Message.Content;
                    if (!mContent.Contains("discordapp.com/api/webhooks/"))
                    {
                        await channel.SendMessageAsync("Alright, I'll make a webhook for you then. Sit tight...");
                        await channel.SendMessageAsync(
                            "Gee, it looks like the developer hasn't implemented this part yet.\n" +
                            "What a silly boy!");
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
                        await ctx.RespondAsync(
                            "Webhook configured. Looks like you're all set! ModCore has been set up.");
                    }
                    //https://canary.discordapp.com/api/webhooks/id/token
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
                        embed.AddField("Linkfilter modules",
                            $"{(linkfilterCfg.BlockInviteLinks ? "✔" : "✖")}anti-invites, " +
                            $"{(linkfilterCfg.BlockBooters ? "✔" : "✖")}anti-ddos, " +
                            $"{(linkfilterCfg.BlockIpLoggers ? "✔" : "✖")}anti-ip loggers, " +
                            $"{(linkfilterCfg.BlockShockSites ? "✔" : "✖")}anti-shock sites, " +
                            $"{(linkfilterCfg.BlockUrlShorteners ? "✔" : "✖")}anti-url shorteners", true);

                        if (linkfilterCfg.ExemptRoleIds.Any())
                        {
                            var roles = linkfilterCfg.ExemptRoleIds
                                .Select(ctx.Guild.GetRole)
                                .Where(xr => xr != null)
                                .Select(xr => xr.Mention);

                            embed.AddField("Linkfilter-exempt roles", string.Join(", ", roles), true);
                        }

                        if (linkfilterCfg.ExemptUserIds.Any())
                        {
                            var users = linkfilterCfg.ExemptUserIds
                                .Select(xid => $"<@!{xid}>");

                            embed.AddField("Linkfilter-exempt users", string.Join(", ", users), true);
                        }

                        if (linkfilterCfg.ExemptInviteGuildIds.Any())
                        {
                            var guilds = string.Join(", ", linkfilterCfg.ExemptInviteGuildIds);

                            embed.AddField("Linkfilter-exempt invite targets", guilds, true);
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

                            embed.AddField("Role State-ignored roles", string.Join(", ", roles), true);
                        }

                        if (roleCfg.IgnoredChannelIds.Any())
                        {
                            var channels = roleCfg.IgnoredChannelIds
                                .Select(ctx.Guild.GetChannel)
                                .Where(xc => xc != null)
                                .Select(xc => xc.Mention);

                            embed.AddField("Role State-ignored channels", string.Join(", ", channels), true);
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

                            embed.AddField("InvisiCop-exempt roles", string.Join(", ", roles), true);
                        }

                        if (inviscopCfg.ExemptUserIds.Any())
                        {
                            var users = inviscopCfg.ExemptUserIds
                                .Select(xid => $"<@!{xid}>");

                            embed.AddField("InvisiCop-exempt users", string.Join(", ", users), true);
                        }
                    }

                    await ctx.RespondAsync(embed: embed.Build());
                });
        }

        [Command("reset"), Description("Resets this guild's configuration to initial state. This cannot be reversed.")]
        public async Task ResetAsync(CommandContext ctx)
        {
            using (var db = this.Database.CreateContext())
            {
                var cfg = db.GuildConfig.SingleOrDefault(xc => (ulong) xc.GuildId == ctx.Guild.Id);
                if (cfg == null)
                {
                    await ctx.RespondAsync("This guild is not configured.");
                    return;
                }

                var nums = new byte[8];
                using (var rng = RandomNumberGenerator.Create())
                    rng.GetBytes(nums);
                var numss = string.Join(", ", nums);
                var numst = string.Join(" ", nums.Reverse());

                await ctx.RespondAsync(
                    $"You are about to reset the configuration for this guild. To confirm, type these numbers in reverse order, using single space as separator: {numss}. You have 45 seconds.");
                var iv = ctx.Client.GetInteractivity();
                var msg = await iv.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id && xm.Content == numst,
                    TimeSpan.FromSeconds(45));
                if (msg == null)
                {
                    await ctx.RespondAsync("Operation aborted.");
                    return;
                }

                db.GuildConfig.Remove(cfg);
                await db.SaveChangesAsync();
            }

            await ctx.RespondAsync("Configuration reset.");
        }

        [Command("prefix"), Aliases("pfix"),
         Description("Sets the command prefix for this guild. Prefixes longer than 10 characters will be truncated.")]
        public async Task PrefixAsync(CommandContext ctx,
            [Description("New command prefix for this guild.")] string prefix = null)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                prefix = null;

            prefix = prefix?.TrimStart();
            if (prefix?.Length > 20)
                prefix = prefix.Substring(0, 21);

            await ctx.WithGuildSettings(cfg => { cfg.Prefix = prefix; });

            await ctx.RespondAsync(prefix == null
                ? "Prefix restored to default."
                : $"Prefix changed to: \"{prefix}\".");
        }

        [Command("muterole"), Aliases("mr"),
         Description("Sets the role used to mute users. Invoking with no arguments will reset this setting.")]
        public async Task MuteRole(CommandContext ctx,
            [Description("New mute role for this guild.")] DiscordRole role = null)
        {
            await ctx.WithGuildSettings(cfg => cfg.MuteRoleId = role == null ? 0 : role.Id);

            await ctx.RespondAsync(role == null ? "Muting disabled." : $"Mute role set to {role.Mention}.");
        }

        [Group("linkfilter"), Aliases("inviteblocker", "invite", "ib", "filter", "lf"),
         Description("Linkfilter configuration commands.")]
        public class Linkfilter
        {
            [Command("enable"), Aliases("on"), Description("Enables linkfilter for this guild.")]
            public async Task EnableAsync(CommandContext ctx)
            {
                await ctx.WithGuildSettings(cfg => cfg.Linkfilter.Enable = true);
                await ctx.Message.CreateReactionAsync(CheckMark);
            }

            [Command("disable"), Aliases("off"), Description("Disables linkfilter for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                await ctx.WithGuildSettings(cfg => cfg.Linkfilter.Enable = false);
                await ctx.Message.CreateReactionAsync(CheckMark);
            }

            [Group("modules"), Aliases("mod", "m", "s"),
             Description("Commands to toggle linkfilter modules for this guild.")]
            public class Modules
            {
                private delegate ref bool WithLinkfilter(GuildLinkfilterSettings lf);

                private static async Task Toggle(CommandContext ctx, string r, WithLinkfilter func)
                {
                    await ctx.WithGuildSettings(cfg =>
                    {
                        ref var cv = ref func(cfg.Linkfilter);
                        switch (r)
                        {
                            case "on":
                            case "enable":
                            case "enabled":
                            case "1":
                            case "true":
                            case "yes":
                            case "y":
                                cv = true;
                                break;
                            case "off":
                            case "disable":
                            case "disabled":
                            case "0":
                            case "false":
                            case "no":
                            case "n":
                                cv = false;
                                break;
                            default:
                                cv = !cv;
                                break;
                        }

                        return ctx.Message.RespondAsync(
                            $"{(cv ? "enabled" : "disabled")} this module");
                    });
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }

                [Group("all"), Aliases("a", "0"), Description("Commands to manage all linkfilter modules at once.")]
                public class AllModules
                {
                    [Command("off"), Aliases("disable", "disabled", "0", "false", "no", "n"),
                     Description("Disables all linkfilter modules for this guild.")]
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
                     Description("Enables all linkfilter modules for this guild.")]
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
                     Description("Sets all linkfilter modules to default for this guild.")]
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
                [Command("exempt"), Aliases("x"), Description("Exempts user from linkfilter checks.")]
                public async Task ExemptAsync(CommandContext ctx,
                    [RemainingText, Description("Member to exempt from linkfilter checks.")] DiscordMember mbr)
                {
                    await ctx.WithGuildSettings(cfg =>
                    {
                        var ibx = cfg.Linkfilter.ExemptUserIds;
                        if (!ibx.Contains(mbr.Id))
                            ibx.Add(mbr.Id);
                    });
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }

                [Command("unexempt"), Aliases("ux"), Description("Unexempts user from linkfilter checks.")]
                public async Task UnexemptAsync(CommandContext ctx,
                    [RemainingText, Description("Member to unexempt from linkfilter checks.")] DiscordMember mbr)
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
                [Command("exempt"), Aliases("x"), Description("Exempts role from linkfilter checks.")]
                public async Task ExemptAsync(CommandContext ctx,
                    [RemainingText, Description("Role to exempt from linkfilter checks.")] DiscordRole rl)
                {
                    await ctx.WithGuildSettings(cfg =>
                    {
                        var ibx = cfg.Linkfilter.ExemptRoleIds;
                        if (!ibx.Contains(rl.Id))
                            ibx.Add(rl.Id);
                    });
                }

                [Command("unexempt"), Aliases("ux"), Description("Unexempts role from linkfilter checks.")]
                public async Task UnexemptAsync(CommandContext ctx,
                    [RemainingText, Description("Role to unexempt from linkfilter checks.")] DiscordRole rl)
                {
                    await ctx.WithGuildSettings(cfg =>
                    {
                        var ibx = cfg.Linkfilter.ExemptRoleIds;
                        if (ibx.Contains(rl.Id))
                            ibx.Remove(rl.Id);
                    });
                }
            }

            [Group("guild"), Aliases("invite", "i"), Description("Invite target exemption management commands,")]
            public class Guild
            {
                [Command("exempt"), Aliases("x"), Description("Exempts code from invite checks.")]
                public async Task ExemptAsync(CommandContext ctx,
                    [RemainingText, Description("Invite code to exempt from invite checks.")] string invite)
                {
                    var inv = await ctx.Client.GetInviteByCodeAsync(invite);
                    if (inv == null)
                    {
                        await ctx.RespondAsync("Invite seems to be invalid. Maybe the bot is banned.");
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
                    [RemainingText, Description("Invite code to unexempt from invite checks.")] string invite)
                {
                    var inv = await ctx.Client.GetInviteByCodeAsync(invite);
                    if (inv == null)
                    {
                        await ctx.RespondAsync("Invite seems to be invalid. Maybe the bot is banned.");
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
            [Command("enable"), Aliases("on"), Description("Enables role state for this guild.")]
            public async Task EnableAsync(CommandContext ctx)
            {
                await ctx.WithGuildSettings(cfg => cfg.RoleState.Enable = true);
                await ctx.RespondAsync("Role State enabled.");
            }

            [Command("disable"), Aliases("off"), Description("Disables role state for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                await ctx.WithGuildSettings(cfg => cfg.RoleState.Enable = false);
                await ctx.RespondAsync("Role State disabled.");
            }

            [Group("role"), Aliases("r"), Description("Role exemption management commands.")]
            public class Role
            {
                [Command("ignore"), Aliases("x"), Description("Exempts role from being saved by Role State.")]
                public async Task ExemptAsync(CommandContext ctx,
                    [RemainingText, Description("Role to exempt from being saved.")] DiscordRole rl)
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
                    [RemainingText, Description("Role to unexempt from being saved.")] DiscordRole rl)
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
                    [RemainingText, Description("Channel to exempt from having its invites saved.")] DiscordChannel chn)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.RoleState.IgnoredChannelIds;
                    if (!ibx.Contains(chn.Id))
                        ibx.Add(chn.Id);
                    await ctx.SetGuildSettingsAsync(cfg);

                    using (var db = this.Database.CreateContext())
                    {
                        var chperms = db.RolestateOverrides.Where(xs =>
                            xs.ChannelId == (long) chn.Id && xs.GuildId == (long) chn.Guild.Id);
                        if (chperms.Any())
                        {
                            db.RolestateOverrides.RemoveRange(chperms);
                            await db.SaveChangesAsync();
                        }
                    }

                    await ctx.Message.CreateReactionAsync(CheckMark);
                }

                [Command("unignore"), Aliases("ux"),
                 Description("Unexempts rchannel from having its overrides saved by Role State.")]
                public async Task UnexemptAsync(CommandContext ctx,
                    [RemainingText, Description("Channel to unexempt from having its invites  saved.")]
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
                                ChannelId = (long) chn.Id,
                                GuildId = (long) chn.Guild.Id,
                                MemberId = (long) xo.Id,
                                PermsAllow = (long) xo.Allow,
                                PermsDeny = (long) xo.Deny
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
            [Command("enable"), Aliases("on"), Description("Enables invisicop for this guild.")]
            public async Task EnableAsync(CommandContext ctx)
            {
                await ctx.WithGuildSettings(cfg => cfg.InvisiCop.Enable = true);
                await ctx.RespondAsync("InvisiCop enabled.");
            }

            [Command("disable"), Aliases("off"), Description("Disables invisicop for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                await ctx.WithGuildSettings(cfg => cfg.InvisiCop.Enable = false);
                await ctx.RespondAsync("InvisiCop disabled.");
            }
        }

        [Group("actionlog"), Aliases("al"), Description("ActionLog configuration commands.")]
        public class ActionLog
        {
            [Command("enable"), Aliases("on"), Description("Enables actionlog for this guild.")]
            public async Task EnableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.ActionLog.Enable = true;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.RespondAsync(
                    $"ActionLog enabled.\nIf you haven't done this yet, Please execute `{cfg.Prefix}config actionlog setwebhook`");
            }

            [Command("disable"), Aliases("off"), Description("Disables actionlog for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.ActionLog.Enable = false;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.RespondAsync("ActionLog disabled.");
            }

            [Command("setwebhook"), Aliases("swh"),
             Description("Sets the webhook ID and token for this guild's action log")]
            public async Task SetWebhookAsync(CommandContext ctx, ulong ID, string token)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.ActionLog.WebhookId = ID;
                cfg.ActionLog.WebhookToken = token;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.RespondAsync("ActionLog webhook configured.");
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
                await ctx.RespondAsync(
                    $"AutoRole enabled.\nIf you haven't done this yet, Please execute `{cfg.Prefix}config autorole setrole`");
            }

            [Command("disable"), Aliases("off"), Description("Disables AutoRole for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.AutoRole.Enable = false;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.RespondAsync("AutoRole disabled.");
            }

            [Command("setrole"), Aliases("sr"),
             Description("Sets the webhook ID and token for this guild's action log")]
            public async Task SetRoleAsync(CommandContext ctx, DiscordRole Role)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.AutoRole.RoleId = (long) Role.Id;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.RespondAsync("AutoRole role configured.");
            }
        }

        [Group("error"), Aliases("er"), Description("Error verbosity configuration commands.")]
        public class ErrorVerbosity
        {
            [Command("chat"), Aliases("c"), Description("Sets command error reporting for this guild (in chat).")]
            public async Task ChatAsync(CommandContext ctx, string verbosity)
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
                        await ctx.RespondAsync(
                            "Unsupported verbosity level.\nSupported levels: `none`, `name`, `namedesc` or `exception`");
                        return;
                }
                cfg.CommandError.Chat = vb;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.RespondAsync($"Error reporting verbosity in chat set to `{verbosity}`.");
            }

            [Command("log"), Aliases("a"), Description("Sets command error reporting for this guild (in action log).")]
            public async Task ActionLogAsync(CommandContext ctx, string verbosity)
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
                        await ctx.RespondAsync(
                            "Unsupported verbosity level.\nSupported levels: `none`, `name`, `namedesc` or `exception`");
                        return;
                }
                cfg.CommandError.ActionLog = vb;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.RespondAsync($"Error reporting verbosity in action log set to `{verbosity}`.");
            }
        }

        [Group("joinlog"), Aliases("j"), Description("Join log configuration commands.")]
        public class JoinLog
        {
            [Command("enable"), Aliases("on"), Description("Enables JoinLog for this guild.")]
            public async Task EnableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.JoinLog.Enable = true;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.RespondAsync(
                    $"Joinlog enabled.\nIf you haven't done this yet, Please execute `{cfg.Prefix}config joinlog setchannel`");
            }

            [Command("disable"), Aliases("off"), Description("Disables JoinLog for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.JoinLog.Enable = false;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.RespondAsync("JoinLog disabled.");
            }

            [Command("setchannel"), Aliases("sc"), Description("Sets the channel ID for this guild's join log.")]
            public async Task SetRoleAsync(CommandContext ctx, DiscordChannel Channel)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.JoinLog.ChannelId = (long) Channel.Id;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.RespondAsync("JoinLog channel configured.");
            }
        }

        [Group("selfrole"), Aliases("sr"), Description("SelfRole configuration commands.")]
        public class SelfRole
        {
            [Command("add"), Aliases("a"), Description("Adds roles to selfrole list")]
            public async Task AddSelfRoleAsync(CommandContext ctx, DiscordRole Role)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                if (!cfg.SelfRoles.Contains(Role.Id))
                {
                    cfg.SelfRoles.Add(Role.Id);
                }
                else
                {
                    await ctx.RespondAsync("This role has already been added!");
                    return;
                }
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.RespondAsync($"Added role `{Role.Name}` with ID `{Role.Id}` to SelfRoles.");
            }

            [Command("remove"), Aliases("r"), Description("Removes roles from selfrole list")]
            public async Task RemoveSelfRoleAsync(CommandContext ctx, DiscordRole Role)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                if (cfg.SelfRoles.Contains(Role.Id))
                {
                    cfg.SelfRoles.Remove(Role.Id);
                }
                else
                {
                    await ctx.RespondAsync("This role isn't in SelfRoles!");
                    return;
                }
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.RespondAsync($"Removed role `{Role.Name}` with ID `{Role.Id}` from SelfRoles.");
            }
        }
    }
}