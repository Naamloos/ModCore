using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using ModCore.Entities;
using ModCore.Logic;

namespace ModCore.Listeners
{
    public class UnbanTimerRemove
    {
        [AsyncListener(EventTypes.GuildBanRemoved)]
        public static async Task CommandError(ModCoreShard bot, GuildBanRemoveEventArgs e)
        {
            var t = Timers.FindNearestTimer(TimerActionType.Unban, e.Member.Id, 0, e.Guild.Id, bot.Database);
            if (t != null)
                await Timers.UnscheduleTimerAsync(t, bot.Client, bot.Database, bot.SharedData);
        }
    }
}
