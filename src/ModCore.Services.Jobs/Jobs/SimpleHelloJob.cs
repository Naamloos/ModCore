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
    [ModCoreJob("hello", "0 0/1 * * * ?")] // every 1 mins
    public class SimpleHelloJob : BaseModCoreJob
    {
        private readonly ILogger _logger;

        public SimpleHelloJob(ILogger<SimpleHelloJob> logger) 
        {
            _logger = logger;
        }
        public override async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Hello!");
        }
    }
}
