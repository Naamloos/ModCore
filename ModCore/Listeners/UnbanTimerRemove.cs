using DSharpPlus.EventArgs;
using ModCore.Entities;
using ModCore.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Listeners
{
    public class UnbanTimerRemove
    {
        [AsyncListener(EventTypes.GuildBanRemoved)]
        public static async Task CommandError(ModCoreShard bot, GuildBanRemoveEventArgs e)
        {
            var t = Timers.FindNearestTimer(TimerActionType.Unban, e.Member.Id, 0, e.Guild.Id, bot.Database);
            if (t != null)
                await Timers.UnscheduleTimerAsync(t, bot.Client, bot.Database, bot.ShardData);
        }
    }
}
