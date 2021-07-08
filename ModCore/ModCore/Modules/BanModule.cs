using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ModCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Modules
{
    [Group("ban")]
    [RequirePermissions(DSharpPlus.Permissions.BanMembers)]
    public class BanModule : BaseCommandModule
    {
        private TimerService timers;

        public BanModule(TimerService timers)
        {
            this.timers = timers;
        }

        [GroupCommand]
        public async Task ExecuteAsync(CommandContext ctx, DiscordMember member, [RemainingText] string reason = "")
        {
            // DM user if possible, else just silently catch the error.
            try
            {
                await member.SendMessageAsync(
                    $"You have been banned from {ctx.Guild.Name}."
                    + (string.IsNullOrEmpty(reason) ? "" : $"\nReason: `{reason.Unmention()}`"));
            }
            catch (Exception ex) { }
            await member.BanAsync(reason: string.IsNullOrEmpty(reason) ? null : reason);

            await ctx.RespondAsync($"Banned member {member.DisplayName} ({member.Id}).");
        }

        [GroupCommand]
        public async Task ExecuteAsync(CommandContext ctx, ulong user_id, [RemainingText] string reason = "")
        {
            await ctx.Guild.BanMemberAsync(user_id, reason: string.IsNullOrEmpty(reason) ? null : reason);
            await ctx.RespondAsync($"Banned non-member {user_id}.");
        }

        [Command("temp")]
        public async Task TempAsync(CommandContext ctx, DiscordMember member, TimeSpan timespan, [RemainingText] string reason = "")
        {
            var ban_end = DateTimeOffset.Now.Add(timespan);
            // DM user if possible, else just silently catch the error.
            try
            {
                await member.SendMessageAsync(
                    $"You have been temporarily banned from {ctx.Guild.Name}."
                    + (string.IsNullOrEmpty(reason) ? "" : $"\nReason: `{reason.Unmention()}`")
                    + $"\nYour ban expires <t:{ban_end.ToUnixTimeSeconds()}:R>");
            }
            catch (Exception ex) { }

            await member.BanAsync(reason: string.IsNullOrEmpty(reason) ? null : reason);

            await ctx.RespondAsync($"Temporarily banned member {member.DisplayName} ({member.Id}).");

            timers.Enqueue(new Entities.TimerEvent()
            {
                GuildId = (long)ctx.Guild.Id,
                UserId = (long)ctx.User.Id,
                Type = Entities.TimerType.Unban,
                Message = reason,
                Dispatch = ban_end
            });
        }
    }
}
