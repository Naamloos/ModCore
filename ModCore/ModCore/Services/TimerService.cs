using DSharpPlus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModCore.Services
{
    public class TimerService : BackgroundService
    {
        private ILogger logger;
        private DatabaseContext database;

        public TimerService(ILogger<TimerService> logger, DatabaseService databaseService)
        {
            this.logger = logger;
            this.database = databaseService.GetDatabase();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
        }
    }
}
