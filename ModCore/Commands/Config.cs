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

        public Config(DatabaseContextBuilder db)
        {
            this.Database = db;
        }

        public async Task ExecuteGroupAsync(CommandContext ctx)
        {
            await ctx.IfGuildSettings(async () => await ctx.RespondAsync("This guild is not configured."),
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
            var db = this.Database.CreateContext();
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
            var iv = ctx.Client.GetInteractivityModule();
            var msg = await iv.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id && xm.Content == numst,
                TimeSpan.FromSeconds(45));
            if (msg == null)
            {
                await ctx.RespondAsync("Operation aborted.");
                return;
            }

            db.GuildConfig.Remove(cfg);
            await db.SaveChangesAsync();

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
            if (prefix?.Length > 10)
                prefix = prefix.Substring(0, 10);

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
        public class InviteBlocker
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

                private static async Task Toggle(CommandContext ctx, WithLinkfilter func)
                {
                    await ctx.WithGuildSettings(cfg =>
                    {
                        ref var cv = ref func(cfg.Linkfilter);
                        cv = !cv;

                        return ctx.Message.RespondAsync(
                            $"{(cv ? "enabled" : "disabled")} this module");
                    });
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }

                [Group("all"), Aliases("a", "0"), Description("Commands to manage all linkfilter modules at once.")]
                public class AllModules
                {
                    [Command("off"), Aliases("false", "f", "0"),
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

                    [Command("on"), Aliases("true", "t", "1"),
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

                [Command("booters"), Aliases("booter", "boot", "ddos", "b", "1"),
                 Description("Toggle blocking booter/DDoS sites for this guild.")]
                public async Task ToggleBlockBootersAsync(CommandContext ctx)
                    => await Toggle(ctx, lf => ref lf.BlockBooters);

                [Command("invites"), Aliases("invitelinks", "invite", "inv", "i", "2"),
                 Description("Toggle blocking invite links for this guild.")]
                public async Task ToggleBlockInviteLinksAsync(CommandContext ctx)
                    => await Toggle(ctx, lf => ref lf.BlockInviteLinks);

                [Command("iploggers"), Aliases("iplogs", "ips", "ip", "3"),
                 Description("Toggle blocking IP logger sites for this guild.")]
                public async Task ToggleBlockIpLoggersAsync(CommandContext ctx)
                    => await Toggle(ctx, lf => ref lf.BlockIpLoggers);

                [Command("shocksites"), Aliases("shock", "shocks", "gore", "g", "4"),
                 Description("Toggle blocking shock/gore sites for this guild.")]
                public async Task ToggleBlockShockSitesAsync(CommandContext ctx)
                    => await Toggle(ctx, lf => ref lf.BlockShockSites);

                [Command("urlshortener"), Aliases("urlshorteners", "urlshort", "urls", "url", "u", "5"),
                 Description("Toggle blocking URL shortener links for this guild.")]
                public async Task ToggleBlockUrlShortenersAsync(CommandContext ctx)
                    => await Toggle(ctx, lf => ref lf.BlockUrlShorteners);
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
    }
}