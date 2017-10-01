using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ModCore.Entities;
using System;
using System.Threading.Tasks;

namespace ModCore.Commands
{
    public class Main
    {
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
            await m.BanAsync(7, $"Banned by: {ctx.Member.DisplayName} {(reason != "" ? "With reason: " + reason : "") }");
            await ctx.RespondAsync($"Banned user {m.DisplayName} (ID:{m.Id})");
        }

        [Command("kick"), Aliases("k"), RequirePermissions(Permissions.KickMembers)]
        public async Task KickAsync(CommandContext ctx, DiscordMember m, string reason = "")
        {
            if (ctx.Member.Id == m.Id)
            {
                await ctx.RespondAsync("You can't do that to yourself! You have so much to live for!");
                return;
            }
            await m.RemoveAsync($"Kicked by: {ctx.Member.DisplayName} {(reason != "" ? "With reason: " + reason : "") }");
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
            await m.BanAsync(7, $"Softbanned by: {ctx.Member.DisplayName} {(reason != "" ? "With reason: " + reason : "") }");
            await m.UnbanAsync(ctx.Guild, $"Softbanned by: {ctx.Member.DisplayName} {(reason != "" ? "With reason: " + reason : "") }");
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
            var b = ctx.Dependencies.GetDependency<ModCoreShard>().Settings.MuteRoleId;
            var mute = ctx.Guild.GetRole(b);
            await m.GrantRoleAsync(mute);
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
            var b = ctx.Dependencies.GetDependency<ModCoreShard>().Settings.MuteRoleId;
            var mute = ctx.Guild.GetRole(b);
            await m.RevokeRoleAsync(mute);
            await ctx.RespondAsync($"Unmuted user {m.DisplayName} (ID:{m.Id}) { (reason != "" ? "With reason: " + reason : "")}");
        }
    }
}
