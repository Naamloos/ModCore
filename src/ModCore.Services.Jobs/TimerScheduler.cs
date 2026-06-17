using ModCore.Common.Database;
using ModCore.Common.Database.Entities;
using ModCore.Services.Jobs.Jobs;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Services.Jobs
{
    public class TimerScheduler
    {
        private const string JOB_IDENTIFIER = "modcore-timers";
        private const string JOB_TRIGGER_IDENTIFIER = "modcore-timers-trigger";

        private readonly DatabaseContext _database;
        private readonly ISchedulerFactory _schedulerFactory;

        public TimerScheduler(DatabaseContext database, ISchedulerFactory schedulerFactory)
        {
            _database = database;
            _schedulerFactory = schedulerFactory;
        }

        public async Task RescheduleAsync(CancellationToken token)
        {
            var timer = _database.Timers.OrderBy(t => t.TriggersAt).FirstOrDefault();

            if (timer is default(DatabaseTimer))
            {
                // No timers, nothing to schedule
                return;
            }

            var scheduler = await _schedulerFactory.GetScheduler(token);

            var job = JobBuilder.Create<TimerJob>()
                .WithIdentity(JOB_IDENTIFIER)
                .UsingJobData("id", timer.TimerId)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity(JOB_TRIGGER_IDENTIFIER)
                .StartAt(timer.TriggersAt)
                .Build();

            var triggerExists = await scheduler.CheckExists(trigger.Key, token);

            if (triggerExists)
            {
                var existingTrigger = await scheduler.GetTrigger(trigger.Key, token);
                await scheduler.RescheduleJob(trigger.Key, trigger);
            }
            else
            {
                await scheduler.ScheduleJob(job, trigger, token);
            }
        }
    }
}
