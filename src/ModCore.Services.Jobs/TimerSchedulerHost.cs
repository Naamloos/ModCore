using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModCore.Common.Cache;
using ModCore.Common.Database;
using ModCore.Common.Database.Entities;
using ModCore.Common.PubSub.Implementation;
using ModCore.Common.PubSub.Payloads;
using ModCore.Services.Jobs.Jobs;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Services.Jobs
{
    public class TimerSchedulerHost : IHostedService
    {
        private readonly Common.PubSub.PubSubService _pubSub;
        private readonly TimerScheduler _timerScheduler;

        public TimerSchedulerHost(Common.PubSub.PubSubService pubSub, TimerScheduler timerScheduler) 
        {
            _pubSub = pubSub;
            _timerScheduler = timerScheduler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _pubSub.SubscribeAsync<CreateTimerPayload>(async (payload, cancellation) =>
            {
                await this._timerScheduler.RescheduleAsync(cancellation);
            });

            await this._timerScheduler.RescheduleAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _pubSub.UnsubscribeAsync<CreateTimerPayload>(cancellationToken);
        }
    }
}
