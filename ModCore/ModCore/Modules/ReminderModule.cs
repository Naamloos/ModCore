using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using ModCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Modules
{
    [Group("remind")]
    [Aliases("reminder", "remindme")]
    public class ReminderModule : BaseCommandModule
    {
        TimerService timers;

        public ReminderModule(TimerService timers)
        {
            this.timers = timers;
        }

        [GroupCommand]
        public async Task ExecuteAsync(CommandContext ctx, TimeSpan dispatch, [RemainingText] string message)
        {
            await ctx.RespondAsync($"\u23f0 Reminding you <t:{DateTimeOffset.Now.Add(dispatch).ToUnixTimeSeconds()}:R>.");
            timers.Enqueue(new Entities.TimerEvent()
            {
                ChannelId = (long)ctx.Channel.Id,
                UserId = (long)ctx.User.Id,
                Type = Entities.TimerType.Reminder,
                Dispatch = DateTimeOffset.Now.Add(dispatch),
                Message = message,
                Creation = DateTimeOffset.Now
            });
        }

        [Command("in")]
        public async Task InAsync(CommandContext ctx, TimeSpan dispatch, [RemainingText] string message) => await ExecuteAsync(ctx, dispatch, message);
    }
}
