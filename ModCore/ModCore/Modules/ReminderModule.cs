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
        public async Task Execute(CommandContext ctx, TimeSpan dispatch, string message)
        {
            await ctx.RespondAsync($"Done! Reminding you about {message} <t:{DateTimeOffset.Now.Add(dispatch).ToUnixTimeSeconds()}:R>");
            timers.Enqueue(new Entities.TimerEvent()
            {
                ChannelId = (long)ctx.Channel.Id,
                UserId = (long)ctx.User.Id,
                Type = Entities.TimerType.Reminder,
                Dispatch = DateTimeOffset.Now.Add(dispatch),
                Message = message
            });
        }
    }
}
