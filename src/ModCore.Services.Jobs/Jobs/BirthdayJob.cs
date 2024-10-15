using Microsoft.Extensions.Logging;
using ModCore.Services.Jobs.Attributes;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Jobs.Jobs
{
    // more info on the cron-like format: https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontrigger.html
    [ModCoreJob("birthday", "0 0 0 * * ?")] // every day at midnight
    public class BirthdayJob : BaseModCoreJob
    {
        private readonly ILogger _logger;

        public BirthdayJob(ILogger<SimpleHelloJob> logger)
        {
            _logger = logger;
        }
        public override async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Checking for birthdays that are today...");
            // TODO: Implement birthday checking
        }
    }
}
