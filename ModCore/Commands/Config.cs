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

            var cfgib = gcfg.InviteBlocker;
            embed.AddField("Invite Blocker", cfgib.Enable ? "Enabled" : "Disabled", true);
            if (cfgib.Enable)
            {
                if (cfgib.ExemptRoleIds.Any())
                {
                    var roles = cfgib.ExemptRoleIds
                        .Select(ctx.Guild.GetRole)
                        .Where(xr => xr != null)
                        .Select(xr => xr.Mention);

                    embed.AddField("Invite Blocker-exempt roles", string.Join(", ", roles), true);
                }

                if (cfgib.ExemptUserIds.Any())
                {
                    var users = cfgib.ExemptUserIds
                        .Select(xid => $"<@!{xid}>");

                    embed.AddField("Invite Blocker-exempt users", string.Join(", ", users), true);
                }

                if (cfgib.ExemptInviteGuildIds.Any())
                {
                    var guilds = string.Join(", ", cfgib.ExemptInviteGuildIds);

                    embed.AddField("Invite Blocker-exempt invite targets", guilds, true);
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

        [Group("inviteblocker"), Aliases("invite", "ib"), Description("Invite Blocker configuration commands.")]
        public class InviteBlocker
        {
            [Command("enable"), Aliases("on"), Description("Enables invite blocker for this guild.")]
            public async Task EnableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.InviteBlocker.Enable = true;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.Message.CreateReactionAsync(CheckMark);
            }

            [Command("disable"), Aliases("off"), Description("Disables invite blocker for this guild.")]
            public async Task DisableAsync(CommandContext ctx)
            {
                var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                cfg.InviteBlocker.Enable = false;
                await ctx.SetGuildSettingsAsync(cfg);
                await ctx.Message.CreateReactionAsync(CheckMark);
            }

            [Group("user"), Aliases("usr", "u"), Description("User exemption management commands.")]
            public class User
            {
                [Command("exempt"), Aliases("x"), Description("Exempts user from invite checks.")]
                public async Task ExemptAsync(CommandContext ctx, [RemainingText, Description("Member to exempt from invite checks.")] DiscordMember mbr)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.InviteBlocker.ExemptUserIds;
                    if (!ibx.Contains(mbr.Id))
                        ibx.Add(mbr.Id);
                    await ctx.SetGuildSettingsAsync(cfg);
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }

                [Command("unexempt"), Aliases("ux"), Description("Unexempts user from invite checks.")]
                public async Task UnexemptAsync(CommandContext ctx, [RemainingText, Description("Member to unexempt from invite checks.")] DiscordMember mbr)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.InviteBlocker.ExemptUserIds;
                    if (ibx.Contains(mbr.Id))
                        ibx.Remove(mbr.Id);
                    await ctx.SetGuildSettingsAsync(cfg);
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }
            }

            [Group("role"), Aliases("r"), Description("Role exemption management commands.")]
            public class Role
            {
                [Command("exempt"), Aliases("x"), Description("Exempts user from invite checks.")]
                public async Task ExemptAsync(CommandContext ctx, [RemainingText, Description("Role to exempt from invite checks.")] DiscordRole rl)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.InviteBlocker.ExemptRoleIds;
                    if (!ibx.Contains(rl.Id))
                        ibx.Add(rl.Id);
                    await ctx.SetGuildSettingsAsync(cfg);
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }

                [Command("unexempt"), Aliases("ux"), Description("Unexempts user from invite checks.")]
                public async Task UnexemptAsync(CommandContext ctx, [RemainingText, Description("Role to unexempt from invite checks.")] DiscordRole rl)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.InviteBlocker.ExemptRoleIds;
                    if (ibx.Contains(rl.Id))
                        ibx.Remove(rl.Id);
                    await ctx.SetGuildSettingsAsync(cfg);
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }
            }

            [Group("guild"), Aliases("i"), Description("Invite target exemption management commands,")]
            public class Guild
            {
                [Command("exempt"), Aliases("x"), Description("Exempts user from invite checks.")]
                public async Task ExemptAsync(CommandContext ctx, [RemainingText, Description("Invite code to exempt from invite checks.")] string invite)
                {
                    var inv = await ctx.Client.GetInviteByCodeAsync(invite);
                    if (inv == null)
                        return;

                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.InviteBlocker.ExemptInviteGuildIds;
                    if (!ibx.Contains(inv.Guild.Id))
                        ibx.Add(inv.Guild.Id);
                    await ctx.SetGuildSettingsAsync(cfg);
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }

                [Command("unexempt"), Aliases("ux"), Description("Unexempts user from invite checks.")]
                public async Task UnexemptAsync(CommandContext ctx, [RemainingText, Description("Invite code to unexempt from invite checks.")] string invite)
                {
                    var inv = await ctx.Client.GetInviteByCodeAsync(invite);
                    if (inv == null)
                        return;

                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.InviteBlocker.ExemptInviteGuildIds;
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

            [Group("role"), Aliases("r"), Description("Role exemption management commands.")]
            public class Role
            {
                [Command("ignore"), Aliases("x"), Description("Exempts role from being saved by Role State.")]
                public async Task ExemptAsync(CommandContext ctx, [RemainingText, Description("Role to exempt from being saved.")] DiscordRole rl)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.RoleState.IgnoredRoleIds;
                    if (!ibx.Contains(rl.Id))
                        ibx.Add(rl.Id);
                    await ctx.SetGuildSettingsAsync(cfg);
                    await ctx.Message.CreateReactionAsync(CheckMark);
                }

                [Command("unignore"), Aliases("ux"), Description("Unexempts role from being saved by Role State.")]
                public async Task UnexemptAsync(CommandContext ctx, [RemainingText, Description("Role to unexempt from being saved.")] DiscordRole rl)
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

                [Command("ignore"), Aliases("x"), Description("Exempts channel from having its overrides saved by Role State.")]
                public async Task ExemptAsync(CommandContext ctx, [RemainingText, Description("Channel to exempt from having its invites saved.")] DiscordChannel chn)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.RoleState.IgnoredChannelIds;
                    if (!ibx.Contains(chn.Id))
                        ibx.Add(chn.Id);
                    await ctx.SetGuildSettingsAsync(cfg);

                    var db = this.Database.CreateContext();
                    var chperms = db.RolestateOverrides.Where(xs => xs.ChannelId == (long)chn.Id && xs.GuildId == (long)chn.Guild.Id);
                    if (chperms.Any())
                    {
                        db.RolestateOverrides.RemoveRange(chperms);
                        await db.SaveChangesAsync();
                    }

                    await ctx.Message.CreateReactionAsync(CheckMark);
                }

                [Command("unignore"), Aliases("ux"), Description("Unexempts rchannel from having its overrides saved by Role State.")]
                public async Task UnexemptAsync(CommandContext ctx, [RemainingText, Description("Channel to unexempt from having its invites  saved.")] DiscordChannel chn)
                {
                    var cfg = ctx.GetGuildSettings() ?? new GuildSettings();
                    var ibx = cfg.RoleState.IgnoredChannelIds;
                    if (ibx.Contains(chn.Id))
                        ibx.Remove(chn.Id);
                    await ctx.SetGuildSettingsAsync(cfg);

                    var os = chn.PermissionOverwrites.Where(xo => xo.Type == "member");
                    var db = this.Database.CreateContext();
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
    }
}
