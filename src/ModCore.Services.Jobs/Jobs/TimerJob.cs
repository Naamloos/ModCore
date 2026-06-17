using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModCore.Common.Database;
using ModCore.Common.Database.Entities;
using ModCore.Common.Database.Timers;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Entities.Utils;
using ModCore.Common.Discord.Rest;
using ModCore.Common.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Services.Jobs.Jobs
{
    [DisallowConcurrentExecution]
    public class TimerJob : IJob
    {
        private const int CHUNK_SIZE = 15;

        private readonly TimerScheduler _timerScheduler;
        private readonly DatabaseContext _database;
        private readonly ILogger<TimerJob> _logger;
        private readonly DiscordRest _rest;

        public TimerJob(
            TimerScheduler timerScheduler,
            DatabaseContext database,
            ILogger<TimerJob> logger,
            DiscordRest rest)
        {
            _timerScheduler = timerScheduler;
            _database = database;
            _logger = logger;
            _rest = rest;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var token = context.CancellationToken;

            try
            {
                await triggerDueTimers(token);
            }
            finally
            {
                await _timerScheduler.RescheduleAsync(token);
            }
        }

        private async Task triggerDueTimers(CancellationToken token)
        {
            var now = DateTimeOffset.UtcNow;

            DateTimeOffset? lastTriggersAt = null;
            long lastTimerId = 0;

            while (true)
            {
                var query = _database.Timers
                    .Where(t => t.TriggersAt <= now);

                if (lastTriggersAt is not null)
                {
                    query = query.Where(t =>
                        t.TriggersAt > lastTriggersAt.Value ||
                        (
                            t.TriggersAt == lastTriggersAt.Value &&
                            t.TimerId > lastTimerId
                        ));
                }

                var timers = await query
                    .OrderBy(t => t.TriggersAt)
                    .ThenBy(t => t.TimerId)
                    .Take(CHUNK_SIZE)
                    .ToListAsync(token);

                if (timers.Count == 0)
                {
                    return;
                }

                foreach (var timer in timers)
                {
                    try
                    {
                        await triggerTimer(timer);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Failed to trigger timer {TimerId} of type {Type}",
                            timer.TimerId,
                            timer.Type);
                    }
                    finally
                    {
                        // Remove even if it fails, otherwise a bad reminder can loop forever.
                        _database.Timers.Remove(timer);
                    }
                }

                var lastTimer = timers[^1];

                lastTriggersAt = lastTimer.TriggersAt;
                lastTimerId = lastTimer.TimerId;

                await _database.SaveChangesAsync(token);
            }
        }

        private async Task triggerTimer(DatabaseTimer timer)
        {
            switch (timer.Type)
            {
                default:
                    _logger.LogWarning(
                        "Unknown timer type {Type} for timer {TimerId}",
                        timer.Type,
                        timer.TimerId);

                    break;

                case TimerTypes.Reminder:
                    var reminderData = timer.GetData<ReminderTimerData>();
                    await triggerReminder(timer, reminderData);
                    break;
            }
        }

        // TODO: Check if user is still in server, otherwise DM, otherwise fail silently
        private async Task triggerReminder(DatabaseTimer timer, ReminderTimerData data)
        {
            var message = new MessageBuilder()
                .AddContainer(container =>
                {
                    container.AddText(
                        $"🔔 <@{data.UserId}>, you wanted to be reminded " +
                        $"{DiscordFormatter.Timestamp(timer.TriggersAt, DiscordFormatter.TimestampFormat.RelativeTime)}:");

                    container.AddText($"```\n{data.Text.InCodeBlock()}\n```");
                })
                .Build();

            message.AllowedMentions = new AllowedMention()
            {
                Users = new List<Snowflake>() { data.UserId },
            };

            await _rest.CreateMessageAsync(data.ChannelId, message);
        }
    }
}