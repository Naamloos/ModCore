using System;
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
            var gcfg = ctx.GetGuildSettings();
            if (gcfg == null)
            {
                await ctx.RespondAsync("This guild is not configured.");
                return;
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = ctx.Guild.Name,
                ThumbnailUrl = ctx.Guild.IconUrl,
                Description = "ModCore configuration for this guild:"
            };

            embed.AddField("Command Prefix", gcfg.Prefix != null ? $"\"{gcfg.Prefix}\"" : "Not configured", true);

            var muted = gcfg.MuteRoleId != 0 ? ctx.Guild.GetRole(gcfg.MuteRoleId) : null;
            embed.AddField("Muted Role", muted != null ? muted.Mention : "Not configured or missing", true);

            var cfgib = gcfg.Linkfilter;
            embed.AddField("Linkfilter", cfgib.Enable ? "Enabled" : "Disabled", true);
            if (cfgib.Enable)
            {
                embed.AddField("Linkfilter modules",
                    $"{(cfgib.BlockInviteLinks ? "✔" : "✖")}anti-invites, " +
                    $"{(cfgib.BlockBooters ? "✔" : "✖")}anti-ddos, " +
                    $"{(cfgib.BlockIpLoggers ? "✔" : "✖")}anti-ip loggers, " +
                    $"{(cfgib.BlockShockSites ? "✔" : "✖")}anti-shock sites, " +
                    $"{(cfgib.BlockUrlShorteners ? "✔" : "✖")}anti-url shorteners", true);
                
                if (cfgib.ExemptRoleIds.Any())
                {
                    var roles = cfgib.ExemptRoleIds
                        .Select(ctx.Guild.GetRole)
                        .Where(xr => xr != null)
                        .Select(xr => xr.Mention);

                    embed.AddField("Linkfilter-exempt roles", string.Join(", ", roles), true);
                }

                if (cfgib.ExemptUserIds.Any())
                {
                    var users = cfgib.ExemptUserIds
                        .Select(xid => $"<@!{xid}>");

                    embed.AddField("Linkfilter-exempt users", string.Join(", ", users), true);
                }

                if (cfgib.ExemptInviteGuildIds.Any())
                {
                    var guilds = string.Join(", ", cfgib.ExemptInviteGuildIds);

                    embed.AddField("Linkfilter-exempt invite targets", guilds, true);
                }
            }

            var cfgrs = gcfg.RoleState;
            embed.AddField("Role State", cfgrs.Enable ? "Enabled" : "Disabled", true);
            if (cfgrs.Enable)
            {
                if (cfgrs.IgnoredRoleIds.Any())
                {
                    var roles = cfgrs.IgnoredRoleIds
                        .Select(ctx.Guild.GetRole)
                        .Where(xr => xr != null)
                        .Select(xr => xr.Mention);

                    embed.AddField("Role State-ignored roles", string.Join(", ", roles), true);
                }

                if (cfgrs.IgnoredChannelIds.Any())
                {
                    var channels = cfgrs.IgnoredChannelIds
                        .Select(ctx.Guild.GetChannel)
                        .Where(xc => xc != null)
                        .Select(xc => xc.Mention);

                    embed.AddField("Role State-ignored channels", string.Join(", ", channels), true);
                }
            }

            var cfgic = gcfg.InvisiCop;
            embed.AddField("InvisiCop", cfgic.Enable ? "Enabled" : "Disabled", true);
            if (cfgic.Enable)
            {
                if (cfgic.ExemptRoleIds.Any())
                {
                    var roles = cfgic.ExemptRoleIds
                        .Select(ctx.Guild.GetRole)
                        .Where(xr => xr != null)
                        .Select(xr => xr.Mention);

                    embed.AddField("InvisiCop-exempt roles", string.Join(", ", roles), true);
                }

                if (cfgic.ExemptUserIds.Any())
                {
                    var users = cfgic.ExemptUserIds
                        .Select(xid => $"<@!{xid}>");

                    embed.AddField("InvisiCop-exempt users", string.Join(", ", users), true);
                }
            }

            await ctx.RespondAsync(embed: embed.Build());
        }

        [Command("reset"), Description("Resets this guild's configuration to initial state. This cannot be reversed.")]
        public async Task ResetAsync(CommandContext ctx)
        {
            var db = this.Database.CreateContext();
            var cfg = db.GuildConfig.SingleOrDefault(xc => (ulong)xc.GuildId == ctx.Guild.Id);
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

            await ctx.RespondAsync($"You are about to reset the configuration for this guild. To confirm, type these numbers in reverse order, using single space as separator: {numss}. You have 45 seconds.");
            var iv = ctx.Client.GetInteractivityModule();
            var msg = await iv.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id && xm.Content == numst, TimeSpan.FromSeconds(45));
            if (msg == null)
            {
                await ctx.RespondAsync("Operation aborted.");
                return;
            }

            db.GuildConfig.Remove(cfg);
            await db.SaveChangesAsync();

            await ctx.RespondAsync("Configuration reset.");
        }

        [Command("prefix"), Aliases("pfix"), Description("Sets the command prefix for this guild. Prefixes longer than 10 characters will be truncated.")]
        public async Task PrefixAsync(CommandContext ctx, [Description("New command prefix for this guild.")] string prefix = null)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                prefix = null;

            prefix = prefix?.TrimStart();
            if (prefix?.Length > 10)
                prefix = prefix?.Substring(0, 10);

            var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
            cfg.Prefix = prefix;
            await ctx.SetGuildSettingsAsync(cfg);

            await ctx.RespondAsync(prefix == null ? "Prefix restored to default." : $"Prefix changed to: \"{prefix}\".");
        }

        [Command("muterole"), Aliases("mr"), Description("Sets the role used to mute users. Invoking with no arguments will reset this setting.")]
        public async Task MuteRole(CommandContext ctx, [Description("New mute role for this guild.")] DiscordRole role = null)
        {
            var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
            cfg.MuteRoleId = role == null ? 0 : role.Id;
            await ctx.SetGuildSettingsAsync(cfg);

            await ctx.RespondAsync(role == null ? "Muting disabled." : $"Mute role set to {role.Mention}.");
        }

        [Group("linkfilter"), Aliases("inviteblocker", "invite", "ib", "filter", "lf"), Description("Linkfilter configuration commands.")]
        public class InviteBlocker
        {
            [Command("enable"), Aliases("on"), Description("Enables linkfilter for this guild.")]
            public async Task EnableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.Linkfilter.Enable = true;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.Message.CreateReactionAsync(CheckMark);
            }

            [Command("disable"), Aliases("off"), Description("Disables linkfilter for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.Linkfilter.Enable = false;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.Message.CreateReactionAsync(CheckMark);
            }

            [Group("modules"), Aliases("mod", "m", "s"), Description("Commands to toggle linkfilter modules for this guild.")]
            public class Modules
            {
                [Group("all"), Aliases("a", "0"), Description("Commands to manage all linkfilter modules at once.")]
                public class AllModules
                {
                    [Command("off"), Aliases("false", "f", "0"), Description("Disables all linkfilter modules for this guild.")]
                    public async Task DisableAllLinkfilterModulesAsync(CommandContext ctx)
                    {
                        var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                        cfg.Linkfilter.BlockBooters = false;
                        cfg.Linkfilter.BlockInviteLinks = false;
                        cfg.Linkfilter.BlockIpLoggers = false;
                        cfg.Linkfilter.BlockShockSites = false;
                        cfg.Linkfilter.BlockUrlShorteners = false;
                        await ctx.SetGuildSettingsAsync(cfg);
                        await ctx.Message.CreateReactionAsync(CheckMark);
                    }
                    
                    [Command("on"), Aliases("true", "t", "1"), Description("Enables all linkfilter modules for this guild.")]
                    public async Task EnableAllLinkfilterModulesAsync(CommandContext ctx)
                    {
                        var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                        cfg.Linkfilter.BlockBooters = true;
                        cfg.Linkfilter.BlockInviteLinks = true;
                        cfg.Linkfilter.BlockIpLoggers = true;
                        cfg.Linkfilter.BlockShockSites = true;
                        cfg.Linkfilter.BlockUrlShorteners = true;
                        await ctx.SetGuildSettingsAsync(cfg);
                        await ctx.Message.CreateReactionAsync(CheckMark);
                    }
                    
                    [Command("default"), Aliases("def", "d", "2"), Description("Sets all linkfilter modules to default for this guild.")]
                    public async Task RestoreDefaultAllLinkfilterModulesAsync(CommandContext ctx)
                    {
                        var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                        cfg.Linkfilter.BlockBooters = true;
                        cfg.Linkfilter.BlockInviteLinks = true;
                        cfg.Linkfilter.BlockIpLoggers = true;
                        cfg.Linkfilter.BlockShockSites = true;
                        cfg.Linkfilter.BlockUrlShorteners = false;
                        await ctx.SetGuildSettingsAsync(cfg);
                        await ctx.Message.CreateReactionAsync(CheckMark);
                    }
                }

                [Command("booters"), Aliases("booter", "boot", "ddos", "b", "1"), Description("Toggle blocking booter/DDoS sites for this guild.")]
                public async Task ToggleBlockBootersAsync(CommandContext ctx)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    cfg.Linkfilter.BlockBooters = !cfg.Linkfilter.BlockBooters;
                    await ctx.SetGuildSettingsAsync(cfg);
                    await ctx.Message.RespondAsync($"{(cfg.Linkfilter.BlockBooters ? "enabled" : "disabled")} this module");
                }

                [Command("invites"), Aliases("invitelinks", "invite", "inv", "i", "2"), Description("Toggle blocking invite links for this guild.")]
                public async Task ToggleBlockInviteLinksAsync(CommandContext ctx)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    cfg.Linkfilter.BlockInviteLinks = !cfg.Linkfilter.BlockInviteLinks;
                    await ctx.SetGuildSettingsAsync(cfg);
                    await ctx.Message.RespondAsync($"{(cfg.Linkfilter.BlockInviteLinks ? "enabled" : "disabled")} this module");
                }

                [Command("iploggers"), Aliases("iplogs", "ips", "ip", "3"), Description("Toggle blocking IP logger sites for this guild.")]
                public async Task ToggleBlockIpLoggersAsync(CommandContext ctx)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    cfg.Linkfilter.BlockIpLoggers = !cfg.Linkfilter.BlockIpLoggers;
                    await ctx.SetGuildSettingsAsync(cfg);
                    await ctx.Message.RespondAsync($"{(cfg.Linkfilter.BlockIpLoggers ? "enabled" : "disabled")} this module");
                }

                [Command("shocksites"), Aliases("shock", "shocks", "gore", "g", "4"), Description("Toggle blocking shock/gore sites for this guild.")]
                public async Task ToggleBlockShockSitesAsync(CommandContext ctx)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    cfg.Linkfilter.BlockShockSites = !cfg.Linkfilter.BlockShockSites;
                    await ctx.SetGuildSettingsAsync(cfg);
                    await ctx.Message.RespondAsync($"{(cfg.Linkfilter.BlockShockSites ? "enabled" : "disabled")} this module");
                }

                [Command("urlshortener"), Aliases("urlshorteners", "urlshort", "urls", "url", "u", "5"), Description("Toggle blocking URL shortener links for this guild.")]
                public async Task ToggleBlockUrlShortenersAsync(CommandContext ctx)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    cfg.Linkfilter.BlockUrlShorteners = !cfg.Linkfilter.BlockUrlShorteners;
                    await ctx.SetGuildSettingsAsync(cfg);
                    await ctx.Message.RespondAsync($"{(cfg.Linkfilter.BlockUrlShorteners ? "enabled" : "disabled")} this module");
                }
            }

            [Group("user"), Aliases("usr", "u"), Description("User exemption management commands.")]
            public class User
            {
                [Command("exempt"), Aliases("x"), Description("Exempts user from linkfilter checks.")]
                public async Task ExemptAsync(CommandContext ctx, [RemainingText, Description("Member to exempt from linkfilter checks.")] DiscordMember mbr)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.Linkfilter.ExemptUserIds;
                    if (!ibx.Contains(mbr.Id))
                        ibx.Add(mbr.Id);
                    await ctx.SetGuildSettingsAsync(cfg);
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }

                [Command("unexempt"), Aliases("ux"), Description("Unexempts user from linkfilter checks.")]
                public async Task UnexemptAsync(CommandContext ctx, [RemainingText, Description("Member to unexempt from linkfilter checks.")] DiscordMember mbr)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.Linkfilter.ExemptUserIds;
                    if (ibx.Contains(mbr.Id))
                        ibx.Remove(mbr.Id);
                    await ctx.SetGuildSettingsAsync(cfg);
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }
            }

            [Group("role"), Aliases("r"), Description("Role exemption management commands.")]
            public class Role
            {
                [Command("exempt"), Aliases("x"), Description("Exempts role from linkfilter checks.")]
                public async Task ExemptAsync(CommandContext ctx, [RemainingText, Description("Role to exempt from linkfilter checks.")] DiscordRole rl)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.Linkfilter.ExemptRoleIds;
                    if (!ibx.Contains(rl.Id))
                        ibx.Add(rl.Id);
                    await ctx.SetGuildSettingsAsync(cfg);
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }

                [Command("unexempt"), Aliases("ux"), Description("Unexempts role from linkfilter checks.")]
                public async Task UnexemptAsync(CommandContext ctx, [RemainingText, Description("Role to unexempt from linkfilter checks.")] DiscordRole rl)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.Linkfilter.ExemptRoleIds;
                    if (ibx.Contains(rl.Id))
                        ibx.Remove(rl.Id);
                    await ctx.SetGuildSettingsAsync(cfg);
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }
            }

            [Group("guild"), Aliases("invite", "i"), Description("Invite target exemption management commands,")]
            public class Guild
            {
                [Command("exempt"), Aliases("x"), Description("Exempts code from invite checks.")]
                public async Task ExemptAsync(CommandContext ctx, [RemainingText, Description("Invite code to exempt from invite checks.")] string invite)
                {
                    var inv = await ctx.Client.GetInviteByCodeAsync(invite);
                    if (inv == null)
                        return;

                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.Linkfilter.ExemptInviteGuildIds;
                    if (!ibx.Contains(inv.Guild.Id))
                        ibx.Add(inv.Guild.Id);
                    await ctx.SetGuildSettingsAsync(cfg);
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }

                [Command("unexempt"), Aliases("ux"), Description("Unexempts code from invite checks.")]
                public async Task UnexemptAsync(CommandContext ctx, [RemainingText, Description("Invite code to unexempt from invite checks.")] string invite)
                {
                    var inv = await ctx.Client.GetInviteByCodeAsync(invite);
                    if (inv == null)
                        return;

                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.Linkfilter.ExemptInviteGuildIds;
                    if (ibx.Contains(inv.Guild.Id))
                        ibx.Remove(inv.Guild.Id);
                    await ctx.SetGuildSettingsAsync(cfg);
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
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.RoleState.Enable = true;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.RespondAsync("Role State enabled.");
            }

            [Command("disable"), Aliases("off"), Description("Disables role state for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.RoleState.Enable = false;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.RespondAsync("Role State disabled.");
            }
        }

        [Group("invisicop"), Aliases("ic"), Description("InvisiCop configuration commands.")]
        public class InvisiCop
        {
            [Command("enable"), Aliases("on"), Description("Enables invisicop for this guild.")]
            public async Task EnableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.InvisiCop.Enable = true;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.RespondAsync("InvisiCop enabled.");
            }

            [Command("disable"), Aliases("off"), Description("Disables invisicop for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.InvisiCop.Enable = false;
                await ctx.SetGuildSettingsAsync(cfg);
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
                await ctx.RespondAsync($"ActionLog enabled.\nIf you haven't done this yet, Please execute `{cfg.Prefix}config actionlog setwebhook`");
            }

            [Command("disable"), Aliases("off"), Description("Disables actionlog for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.ActionLog.Enable = false;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.RespondAsync("ActionLog disabled.");
            }

            [Command("setwebhook"), Aliases("sethook"), Description("Sets the webhook ID and token for this guild's action log")]
            public async Task SetWebhookAsync(CommandContext ctx, ulong ID, string token)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.ActionLog.WebhookId = ID;
                cfg.ActionLog.WebhookToken = token;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.RespondAsync("ActionLog webhook configured.");
            }
        }
    }
}
