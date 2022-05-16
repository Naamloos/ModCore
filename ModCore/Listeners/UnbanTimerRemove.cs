using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using ModCore.Entities;
using ModCore.Logic;

namespace ModCore.Listeners
{
    public class UnbanTimerRemove
    {
        [AsyncListener(EventTypes.GuildBanRemoved)]
        public static async Task CommandError(ModCoreShard bot, GuildBanRemoveEventArgs eventargs)
        {
            var timer = Timers.FindNearestTimer(TimerActionType.Unban, eventargs.Member.Id, 0, eventargs.Guild.Id, bot.Database);
            if (timer != null)
                await Timers.UnscheduleTimerAsync(timer, bot.Client, bot.Database, bot.SharedData);
        }
    }
}
