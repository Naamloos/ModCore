using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using ModCore.Database;
using ModCore.Database.JsonEntities;
using ModCore.Entities;
using ModCore.Extensions.AsyncListeners.Attributes;
using ModCore.Extensions.AsyncListeners.Enums;
using ModCore.Utils;

namespace ModCore.Listeners
{
    public class UnbanTimerRemove
    {
        [AsyncListener(EventType.GuildBanRemoved)]
        public static async Task CommandError(GuildBanRemoveEventArgs eventargs, DatabaseContextBuilder database, DiscordClient client, SharedData sharedData)
        {
            var timer = Timers.FindNearestTimer(TimerActionType.Unban, eventargs.Member.Id, 0, eventargs.Guild.Id, database);
            if (timer != null)
                await Timers.UnscheduleTimerAsync(timer, client, database, sharedData);
        }
    }
}
