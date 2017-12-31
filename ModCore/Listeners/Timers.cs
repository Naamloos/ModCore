using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;
using System.Collections.Generic;

namespace ModCore.Listeners
{
    public static class Timers
    {
        [AsyncListener(EventTypes.Ready)]
        public static async Task OnReady(ModCoreShard shard, ReadyEventArgs ea)
        {
            using (var db = shard.Database.CreateContext())
            {
                if (!db.Timers.Any())
                    return;

                var ids = ea.Client.Guilds.Select(xg => (long)xg.Key).ToArray();
                var timers = db.Timers.Where(xt => ids.Contains(xt.GuildId)).ToArray();
                if (!timers.Any())
                    return;

                // lock timers
                await shard.SharedData.TimerSempahore.WaitAsync();

                var now = DateTimeOffset.UtcNow;
                var past = timers.Where(xt => xt.DispatchAt <= now.AddSeconds(30)).ToArray();
                if (past.Any())
                {
                    foreach (var xt in past)
                    {
                        // dispatch past timers
                        _ = DispatchTimer(new TimerData(null, xt, shard.Client, shard.Database, shard.SharedData, null));
                    }

                    db.Timers.RemoveRange(past);
                    await db.SaveChangesAsync();
                }

                // unlock the timers
                shard.SharedData.TimerSempahore.Release();

                RescheduleTimers(shard.Client, shard.Database, shard.SharedData);
            }
        }

        public static void TimerCallback(Task t, object state)
        {
            var tdata = state as TimerData;
            var shard = tdata.Context;
            var shared = tdata.Shared;

            // lock the timers
            shared.TimerSempahore.Wait();

            // remove the timer
            using (var db = tdata.Database.CreateContext())
            {
                db.Timers.Remove(tdata.DbTimer);
                db.SaveChanges();
                
                tdata.Shared.TimerData = null;
            }

            // release the lock
            shared.TimerSempahore.Release();

            // dispatch the timer
            _ = Task.Run(LocalDispatch);

            // schedule new one
            RescheduleTimers(shard, tdata.Database, tdata.Shared);

            Task LocalDispatch()
                => DispatchTimer(tdata);
        }

        private static async Task DispatchTimer(TimerData tdata)
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
            else if (timer.ActionType == TimerActionType.Unban)
            {
                var data = timer.GetData<TimerUnbanData>();
                if (client.Guilds.Any(x => x.Key == (ulong)timer.GuildId))
                {
                    using (var db = tdata.Database.CreateContext())
                    {
                        var Guild = client.Guilds[(ulong)timer.GuildId];
                        try
                        {
                            await Guild.UnbanMemberAsync((ulong)data.UserId);
                        }
                        catch (Exception) { }

                        var settings = Guild.GetGuildSettings(db);
                        await client.LogAutoActionAsync(Guild, db, $"Member unbanned: {data.DisplayName}#{data.Discriminator} (ID: {data.UserId})");
                    }
                }
            }
            else if (timer.ActionType == TimerActionType.Unmute)
            {
                var data = timer.GetData<TimerUnmuteData>();
                if (client.Guilds.Any(x => x.Key == (ulong)timer.GuildId))
                {
                    using (var db = tdata.Database.CreateContext())
                    {
                        var Guild = client.Guilds[(ulong)timer.GuildId];
                        var Member = await Guild.GetMemberAsync((ulong)data.UserId);
                        var Role = (DiscordRole)null;
                        try
                        {
                            Role = Guild.GetRole((ulong)data.MuteRoleId);
                        }
                        catch (Exception)
                        {
                            try
                            {
                                Role = Guild.GetRole(Guild.GetGuildSettings(db).MuteRoleId);
                            }
                            catch (Exception)
                            {
                                await client.LogAutoActionAsync(Guild, db, $"**[IMPORTANT]**\nFailed to unmute member: {data.DisplayName}#{data.Discriminator} (ID: {data.UserId})\nMute role does not exist!");
                                return;
                            }
                        }
                        await Member.RevokeRoleAsync(Role, "");
                        await client.LogAutoActionAsync(Guild, db, $"Member unmuted: {data.DisplayName}#{data.Discriminator} (ID: {data.UserId})");
                    }
                }
            }
            else if (timer.ActionType == TimerActionType.Pin)
            {
                var data = timer.GetData<TimerPinData>();
                if (client.Guilds.Any(x => x.Key == (ulong)timer.GuildId))
                {
                    using (var db = tdata.Database.CreateContext())
                    {
                        var Guild = client.Guilds[(ulong)timer.GuildId];
                        var Channel = Guild.GetChannel((ulong)data.ChannelId);
                        var Message = await Channel.GetMessageAsync((ulong)data.MessageId);
                        await Message.PinAsync();
                        await client.LogAutoActionAsync(Guild, db, $"Scheduled pin: Message with ID: {data.MessageId} in Channel #{Channel.Name} ({Channel.Id})");
                    }
                }
            }
            else if (timer.ActionType == TimerActionType.Unpin)
            {
                var data = timer.GetData<TimerPinData>();
                if (client.Guilds.Any(x => x.Key == (ulong)timer.GuildId))
                {
                    using (var db = tdata.Database.CreateContext())
                    {
                        var Guild = client.Guilds[(ulong)timer.GuildId];
                        var Channel = Guild.GetChannel((ulong)data.ChannelId);
                        var Message = await Channel.GetMessageAsync((ulong)data.MessageId);
                        await Message.UnpinAsync();
                        await client.LogAutoActionAsync(Guild, db, $"Scheduled unpin: Message with ID: {data.MessageId} in Channel #{Channel.Name} ({Channel.Id})");
                    }
                }
            }
        }

        public static TimerData RescheduleTimers(DiscordClient client, DatabaseContextBuilder database, SharedData shared)
        {
            // lock the timers
            shared.TimerSempahore.Wait();

            // set a new timer
            DatabaseTimer[] timers = null;
            bool force = false;
            using (var db = database.CreateContext())
            {
                var ids = client.Guilds.Select(xg => (long)xg.Key).ToArray();
                timers = db.Timers.Where(xt => ids.Contains(xt.GuildId))
                    .OrderBy(xt => xt.DispatchAt)
                    .ToArray();

                if (shared.TimerData != null)
                    force = db.Timers.Count(xt => xt.Id == shared.TimerData.DbTimer.Id) == 0; // .Any() throws
            }
            

            var nearest = timers.FirstOrDefault();
            if (nearest == null)
            {
                // there's no nearest timer
                shared.TimerSempahore.Release();
                return null;
            }

            var tdata = shared.TimerData;
            if (tdata != null && tdata.DbTimer.Id == nearest.Id)
            {
                // it's the same timer
                shared.TimerSempahore.Release();
                return tdata;
            }


            if (CancelIfLaterThan(nearest.DispatchAt, shared, force))
            {
                var cts = new CancellationTokenSource();
                var t = Task.Delay(nearest.DispatchAt - DateTimeOffset.UtcNow, cts.Token);
                tdata = new TimerData(t, nearest, client, database, shared, cts);
                _ = t.ContinueWith(TimerCallback, tdata, TaskContinuationOptions.OnlyOnRanToCompletion);
                shared.TimerData = tdata;

            }

            // release the lock
            shared.TimerSempahore.Release();

            return tdata;
        }

        public static async Task<TimerData> UnscheduleTimerAsync(DatabaseTimer timer, DiscordClient shard, DatabaseContextBuilder database, SharedData shared)
        {
            // lock the timers
            await shared.TimerSempahore.WaitAsync();

            // remove the requested timer
            using (var db = database.CreateContext())
            {
                db.Timers.Remove(timer);
                await db.SaveChangesAsync();
            }

            // release the lock
            shared.TimerSempahore.Release();

            // trigger a reschedule
            return RescheduleTimers(shard, database, shared);
        }

        public static async Task<TimerData> UnscheduleTimersAsync(List<DatabaseTimer> timers, DiscordClient shard, DatabaseContextBuilder database, SharedData shared) 
        {
            // lock the timers
            await shared.TimerSempahore.WaitAsync();

            // remove the requested timers
            using (var db = database.CreateContext())
            {
                db.Timers.RemoveRange(timers);
                await db.SaveChangesAsync();
            }

            // release the lock
            shared.TimerSempahore.Release();

            // trigger a reschedule
            return RescheduleTimers(shard, database, shared);
        }

        public static DatabaseTimer FindNearestTimer(TimerActionType actionType, ulong userId, ulong channelId, ulong guildId, DatabaseContextBuilder database)
        {
            using (var db = database.CreateContext())
                return db.Timers.FirstOrDefault(xt => xt.ActionType == actionType && xt.UserId == (long)userId && xt.ChannelId == (long)channelId && xt.GuildId == (long)guildId);
        }

        public static DatabaseTimer FindTimer(int id, TimerActionType actionType, ulong userId, DatabaseContextBuilder database)
        {
            using (var db = database.CreateContext())
                return db.Timers.FirstOrDefault(xt => xt.Id == id && xt.ActionType == actionType && xt.UserId == (long)userId);
        }

        private static bool CancelIfLaterThan(DateTimeOffset dto, SharedData shared, bool force)
        {
            // check if a timer is going
            if (force || shared.TimerData == null || shared.TimerData.DispatchTime >= dto)
            {
                var xtdata = shared.TimerData;
                xtdata?.Cancel?.Cancel();
                return true;
            }

            return false;
        }
    }
}
