using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ModCore.Database;
using ModCore.Entities;
using System.Text;

namespace ModCore.Commands
{
    public class Main
    {
        private DatabaseContextBuilder Database { get; }

        public Main(DatabaseContextBuilder db)
        {
            this.Database = db;
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

        [Command("purgeuser"), Aliases("pu"), RequirePermissions(Permissions.ManageMessages)]
        public async Task PurgeUserAsync(CommandContext ctx, DiscordUser User, int skip = 0)
        {
            int i = 0;
            var ms = await ctx.Channel.GetMessagesAsync(100, ctx.Message.Id);
            foreach (var m in ms)
            {
                if (User != null && m.Author.Id != User.Id) continue;
                if (i < skip)
                    i++;
                else
                    await m.DeleteAsync();
            }
            var resp = await ctx.RespondAsync($"Latest messages by {User.Mention} (ID:{User.Id}) deleted.");
            await Task.Delay(2000);
            await resp.DeleteAsync("Purge command executed.");
            await ctx.Message.DeleteAsync("Purge command executed.");
        }

        [Command("purge"), Aliases("p"), RequirePermissions(Permissions.ManageMessages)]
        public async Task PurgeUserAsync(CommandContext ctx, int skip = 0)
        {
            int i = 0;
            var ms = await ctx.Channel.GetMessagesAsync(100, ctx.Message.Id);
            foreach (var m in ms)
            {
                if (i < skip)
                    i++;
                else
                    await m.DeleteAsync();
            }
            var resp = await ctx.RespondAsync($"Latest messages deleted.");
            await Task.Delay(2000);
            await resp.DeleteAsync("Purge command executed.");
            await ctx.Message.DeleteAsync("Purge command executed.");
        }

        [Command("ban"), Aliases("b"), RequirePermissions(Permissions.BanMembers)]
        public async Task BanAsync(CommandContext ctx, DiscordMember m, string reason = "")
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
        }

        [Command("hackban"), Aliases("hb"), RequirePermissions(Permissions.BanMembers)]
        public async Task HackBanAsync(CommandContext ctx, ulong id, string reason = "")
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
        }

        [Command("kick"), Aliases("k"), RequirePermissions(Permissions.KickMembers)]
        public async Task KickAsync(CommandContext ctx, DiscordMember m, string reason = "")
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
        }

        [Command("softban"), Aliases("s"), RequireUserPermissions(Permissions.KickMembers)]
        public async Task SoftbanAsync(CommandContext ctx, DiscordMember m, string reason = "")
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
        }

        [Command("mute"), Aliases("m"), RequirePermissions(Permissions.MuteMembers)]
        public async Task MuteAsync(CommandContext ctx, DiscordMember m, string reason = "")
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
        }

        [Command("unmute"), Aliases("um"), RequirePermissions(Permissions.MuteMembers)]
        public async Task UnmuteAsync(CommandContext ctx, DiscordMember m, string reason = "")
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
        }

        [Command("userinfo"), Aliases("ui")]
        public async Task UserInfoAsync(CommandContext ctx, DiscordMember usr)
        {
            var embed = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.MidnightBlue)
                .WithTitle($"@{usr.Username}#{usr.Discriminator} - ID: {usr.Id}");

            if (usr.IsBot) embed.Title += " __[BOT]__ ";
            if (usr.IsOwner) embed.Title += " __[OWNER]__ ";

            embed.Description =
                $"Registered on     : {usr.CreationTimestamp.DateTime.ToString()}\n" +
                $"Joined Guild on  : {usr.JoinedAt.DateTime.ToString()}";

            var roles = new StringBuilder();
            foreach (var r in usr.Roles) roles.Append($"[{r.Name}] ");
            if (roles.Length == 0) roles.Append("*None*");
            embed.AddField("Roles", roles.ToString());

            var permsobj = usr.PermissionsIn(ctx.Channel);
            var perms = permsobj.ToPermissionString();
            if (((permsobj & Permissions.Administrator) | (permsobj & Permissions.AccessChannels)) == 0)
                perms = "**[!] User can't see this channel!**\n" + perms;
            if (perms == String.Empty) perms = "*None*";
            embed.AddField("Permissions", perms);

            embed.WithFooter($"{ctx.Guild.Name} / #{ctx.Channel.Name} / {DateTime.Now}");

            await ctx.RespondAsync("", false, embed: embed);
        }
    }
}
