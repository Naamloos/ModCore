using ConcurrentCollections;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using ModCore.Database.JsonEntities;
using ModCore.Entities;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Enums;
using ModCore.Utils.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModCore.Listeners
{
    public static class Timers
    {
        private static DatabaseContextBuilder databaseContextBuilder = null;
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private static (DatabaseTimer timer, CancellationTokenSource cancellation) current = (null, null);
        private static DiscordClient client = null;

        [AsyncListener(EventType.Ready)]
        public static async Task OnReadyAsync(DatabaseContextBuilder database, DiscordClient clientInject)
        {
            if (databaseContextBuilder == null)
            {
                databaseContextBuilder = database;
            }
            if (client == null)
            {
                client = clientInject;
            }

            await ScheduleNextAsync();
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

        public static async Task UnscheduleTimersAsync(params DatabaseTimer[] timers)
        {
            try
            {
                // lock the timers
                await semaphore.WaitAsync();

                // remove the requested timers
                using (var db = databaseContextBuilder.CreateContext())
                {
                    db.Timers.RemoveRange(timers);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught Exception in Timer Unschedule: {ex.GetType().ToString()}\n{ex.StackTrace}");
            }
            finally
            {
                // release the lock
                semaphore.Release();
            }

            await ScheduleNextAsync();
        }

        public static async Task ScheduleNextAsync()
        {
            try
            {
                await semaphore.WaitAsync();
                await TriggerExpiredTimersAsync();

                using var dbContext = databaseContextBuilder.CreateContext();
                if (!dbContext.Timers.Any())
                {
                    return; // We'll reschedule once a new timer arrives through a command.
                }

                var newTimer = dbContext.Timers.OrderBy(x => x.DispatchAt).FirstOrDefault();
                if (newTimer != null)
                {
                    // Cancel previous if necessary
                    if (current.timer == null || current.timer.DispatchAt > newTimer.DispatchAt)
                    {
                        if (current.cancellation != null)
                        {
                            // We cancel our previous dispatch / interim reschedule
                            current.cancellation.Cancel();
                        }

                        // Schedule next
                        current.cancellation = new CancellationTokenSource();

                        var delay = newTimer.DispatchAt.Subtract(DateTime.Now);
                        // There's a max to a delay. If it's over the max, we reschedule on trigger. Else, just schedule dispatch.
                        if (delay.TotalMilliseconds > Int32.MaxValue)
                        {
                            current.timer = null;
                            _ = Task.Delay(TimeSpan.FromMilliseconds(Int32.MaxValue - 1000/* just to be safe */), current.cancellation.Token)
                                .ContinueWith(InterimScheduleNextAsync, current.cancellation, TaskContinuationOptions.OnlyOnRanToCompletion);
                        }
                        else
                        {
                            current.timer = newTimer;
                            _ = Task.Delay(delay, current.cancellation.Token)
                                .ContinueWith(DispatchAsync, current.timer, TaskContinuationOptions.OnlyOnRanToCompletion);
                        }
                    }
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        private static async Task TriggerExpiredTimersAsync()
        {
            using var dbContext = databaseContextBuilder.CreateContext();

            var now = DateTime.UtcNow.AddSeconds(2);
            var expiredTimers = dbContext.Timers.Where(x => x.DispatchAt < now).ToList();

            foreach (var timer in expiredTimers)
            {
                // Dispatch all expired timers without rescheduling.
                try
                {
                    await DispatchAsync(timer, false);
                }
                catch (Exception) { }
            }
        }

        private static async Task InterimScheduleNextAsync(Task task, object data) => await ScheduleNextAsync();

        private static async Task DispatchAsync(Task task, object timer) => await DispatchAsync((DatabaseTimer)timer, true);

        private static async Task DispatchAsync(DatabaseTimer timer, bool dispatchNext)
        {
            switch (timer.ActionType)
            {
                default:
                    client.Logger.LogWarning($"Unknown timer action triggered! {JsonConvert.SerializeObject(timer)}");
                    break;

                case TimerActionType.Reminder:
                    await DispatchReminderAsync(timer, timer.GetData<TimerReminderData>());
                    break;

                case TimerActionType.Unban:
                    await DispatchUnbanAsync(timer, timer.GetData<TimerUnbanData>());
                    break;
            }

            using(var dbContext = databaseContextBuilder.CreateContext())
            {
                dbContext.Timers.Remove(timer);
                await dbContext.SaveChangesAsync();
            }

            if (dispatchNext)
            {
                current.timer = null;
                current.cancellation = null;
                await ScheduleNextAsync();
            }
        }

        private static async Task DispatchReminderAsync(DatabaseTimer timer, TimerReminderData data)
        {
            DiscordChannel channel;
            try
            {
                channel = await client.GetChannelAsync((ulong)timer.ChannelId);
            }
            catch (Exception)
            {
                return;
            }

            var mention = $"<@{timer.UserId}>";
            var unixTimestamp = new DateTimeOffset(timer.DispatchAt);
            // generating a snowflake from original unix timestamp
            ulong fakeContextId = ((ulong)data.OriginalUnix - 1420070400000ul) << 22;
            var link = string.IsNullOrEmpty(data.SnoozedContext) ? $"https://discord.com/channels/{channel.GuildId}/{channel.Id}/{fakeContextId}" : data.SnoozedContext;
            var snooze = new DiscordButtonComponent(ButtonStyle.Secondary, "snooze", "Snooze", emoji: new DiscordComponentEmoji(DiscordEmoji.FromUnicode("⏰")));
            var contextLink = new DiscordLinkButtonComponent(link, "Jump to Context");

            var msg = new DiscordMessageBuilder()
            .AddEmbed
            (
                new DiscordEmbedBuilder()
                .WithTitle("⏰ Reminder")
                .WithDescription(data.Snoozed ? $"You snoozed a reminder to be re-reminded <t:{unixTimestamp.ToUnixTimeSeconds()}:R>"
                    : $"You wanted to be reminded <t:{unixTimestamp.ToUnixTimeSeconds()}:R>")
                .AddField("✏️ Reminder Text", data.ReminderText)
            )
            .WithReply(data.MessageId, true, false)
            .AddComponents(snooze, contextLink)
            .WithContent(mention)
            .WithAllowedMention(new UserMention((ulong)timer.UserId));

            await msg.SendAsync(channel);
        }

        private static async Task DispatchUnbanAsync(DatabaseTimer timer, TimerUnbanData data)
        {
            DiscordGuild guild;

            try
            {
                guild = await client.GetGuildAsync((ulong)timer.GuildId);
            }
            catch(Exception)
            {
                return;
            }

            using (var db = databaseContextBuilder.CreateContext())
            {
                try
                {
                    await guild.UnbanMemberAsync((ulong)data.UserId);
                }
                catch
                {
                    // ignored
                }
                var embed = new DiscordEmbedBuilder()
                    .WithTitle($"Tempban Expired")
                    .WithDescription($"{data.DisplayName} has been unbanned. ({data.UserId})")
                    .WithColor(DiscordColor.Green);
                await guild.ModLogAsync(db, embed);
            }
        }
    }
}
