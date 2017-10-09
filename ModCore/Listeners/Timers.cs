﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Entities;
using ModCore.Logic;

namespace ModCore.Listeners
{
    public static class Timers
    {
        [AsyncListener(EventTypes.Ready)]
        public static async Task OnReady(ModCoreShard shard, ReadyEventArgs ea)
        {
            var db = shard.Database.CreateContext();
            if (!db.Timers.Any())
                return;

            var ids = ea.Client.Guilds.Select(xg => (long)xg.Key).ToArray();
            var timers = db.Timers.Where(xt => ids.Contains(xt.GuildId)).ToArray();
            if (!timers.Any())
                return;

            // lock timers
            await shard.ShardData.TimerSempahore.WaitAsync();

            var now = DateTimeOffset.UtcNow;
            var past = timers.Where(xt => xt.DispatchAt <= now).ToArray();
            if (past.Any())
            {
                foreach (var xt in past)
                {
                    // dispatch past timers
                    await DispatchTimer(new TimerData(null, xt, shard.Client, shard.Database, shard.ShardData, null));
                }

                db.Timers.RemoveRange(past);
                await db.SaveChangesAsync();
            }

            var future = timers.Except(past).OrderBy(xt => xt.DispatchAt).ToArray();
            if (!future.Any())
            {
                shard.ShardData.TimerSempahore.Release();
                return;
            }

            var nearest = future.FirstOrDefault();
            var cts = new CancellationTokenSource();
            var t = Task.Delay(nearest.DispatchAt - DateTimeOffset.UtcNow, cts.Token);
            var tdata = new TimerData(t, nearest, shard.Client, shard.Database, shard.ShardData, cts);
            _ = t.ContinueWith(TimerCallback, tdata, TaskContinuationOptions.OnlyOnRanToCompletion);
            shard.ShardData.TimerData = tdata;

            // release the lock
            shard.ShardData.TimerSempahore.Release();
        }

        public static void TimerCallback(Task t, object state)
        {
            var tdata = state as TimerData;
            var shard = tdata.Context;
            var db = tdata.Database.CreateContext();

            // lock the timers
            tdata.Shared.TimerSempahore.Wait();

            // remove the timer
            db.Timers.Remove(tdata.DbTimer);
            db.SaveChanges();
            tdata.Shared.TimerData = null;

            // dispatch it
            _ = Task.Run(LocalDispatch);

            // set a new timer
            var ids = shard.Guilds.Select(xg => (long)xg.Key).ToArray();
            var timers = db.Timers.Where(xt => ids.Contains(xt.GuildId))
                .OrderBy(xt => xt.DispatchAt)
                .ToArray();
            var nearest = timers.FirstOrDefault();
            if (nearest == null)
            {
                tdata.Shared.TimerSempahore.Release();
                return;
            }

            var cts = new CancellationTokenSource();
            var newt = Task.Delay(nearest.DispatchAt - DateTimeOffset.UtcNow, cts.Token);
            var newtdata = new TimerData(newt, nearest, shard, tdata.Database, tdata.Shared, cts);
            _ = newt.ContinueWith(TimerCallback, newtdata, TaskContinuationOptions.OnlyOnRanToCompletion);
            tdata.Shared.TimerData = newtdata;

            // release the lock
            tdata.Shared.TimerSempahore.Release();

            Task LocalDispatch() =>
                DispatchTimer(tdata);
        }

        public static async Task DispatchTimer(TimerData tdata)
        {
            var timer = tdata.DbTimer;
            var client = tdata.Context;
            if (timer.ActionType == TimerActionType.Reminder)
            {
                DiscordChannel chn = null;
                try
                {
                    chn = await client.GetChannelAsync((ulong)timer.ChannelId);
                }
                catch { }
                if (chn == null)
                    return;

                var data = timer.GetData<TimerReminderData>();
                var emoji = DiscordEmoji.FromName(client, ":alarm_clock:");
                var msg = $"{emoji} <@!{(ulong)timer.UserId}>, you wanted to be reminded of the following:\n\n{data.ReminderText}";
                await chn.SendMessageAsync(msg);
            }
            else if (timer.ActionType == TimerActionType.Ban)
            {
                // TODO
            }
            else if (timer.ActionType == TimerActionType.Mute)
            {
                // TODO
            }
        }
    }
}
