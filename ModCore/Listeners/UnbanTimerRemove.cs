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
            using (var db = bot.Database.CreateContext())
            {
                if (db.Timers.Any(x => x.ActionType == TimerActionType.Unmute && (ulong)x.UserId == e.Member.Id))
                {
                    db.Timers.Remove(db.Timers.First(x => (ulong)x.UserId == e.Member.Id && x.ActionType == TimerActionType.Unban));
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
