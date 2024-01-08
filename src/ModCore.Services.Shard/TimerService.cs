using Microsoft.Extensions.Logging;
using ModCore.Common.Cache;
using ModCore.Common.Database;
using ModCore.Common.Database.Entities;
using ModCore.Common.Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Shard
{
    public class TimerService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ILogger _logger;
        private readonly DiscordRest _rest;
        private readonly CacheService _cache;

        private SemaphoreSlim _semaphore;
        private DatabaseTimer _timer;
        private CancellationTokenSource _cancellation;

        public TimerService(ILogger<TimerService> logger, DatabaseContext dbContext, DiscordRest rest, CacheService cache)
        {
            _databaseContext = dbContext;
            _logger = logger;
            _semaphore = new SemaphoreSlim(1, 1);

            _rest = rest;
            _cache = cache;
            _cancellation = new CancellationTokenSource();
        }

        public async ValueTask StartAsync()
        {
            await DispatchExpiredTimersAsync();
            ScheduleNext();
        }

        public void ScheduleNext()
        {
            try
            {
                _semaphore?.Wait();
                if (!_databaseContext.Timers.Any())
                {
                    return;
                }

                // Fetch upcoming timer
                var nextTimer = _databaseContext.Timers.OrderBy(x => x.TriggersAt).FirstOrDefault();

                if (nextTimer != default)
                {
                    // Cancel timer that was already running
                    if (_cancellation != default && !_cancellation.IsCancellationRequested)
                    {
                        _cancellation.Cancel();
                    }
                    _cancellation = new CancellationTokenSource();

                    var delay = _timer.TriggersAt.Subtract(DateTimeOffset.Now);
                    if (delay.TotalMilliseconds > int.MaxValue)
                    {
                        // Time until this timer hits exceeds the total milliseconds max.
                        _timer = null;
                        _ = Task.Delay(TimeSpan.FromMilliseconds(int.MaxValue - 1000), _cancellation.Token)
                            .ContinueWith((t, o) => ScheduleNext(), _cancellation.Token, TaskContinuationOptions.OnlyOnRanToCompletion);
                    }
                    else
                    {
                        // Set a delayed task for next timer
                        _timer = nextTimer;
                        _ = Task.Delay(delay, _cancellation.Token)
                            .ContinueWith((t, o) => DispatchAsync(_timer, true), _cancellation.Token, TaskContinuationOptions.OnlyOnRanToCompletion);
                    }
                }
            }
            finally
            {
                _semaphore?.Release();
            }
        }

        private async ValueTask DispatchExpiredTimersAsync()
        {
            var now = DateTimeOffset.Now;
            var expiredTimers = _databaseContext.Timers.Where(x => x.TriggersAt < now).ToList();

            foreach (var timer in expiredTimers)
            {
                try
                {
                    await DispatchAsync(timer, false);
                }
                catch (Exception) { }
            }
        }

        private async ValueTask DispatchAsync(DatabaseTimer timer, bool scheduleNext)
        {
            _semaphore.Wait();
            switch(_timer.Type)
            {
                default:
                    _logger.LogWarning("Unknown timer type triggered! type: {0}", _timer.Type); 
                    break;

                // TODO implement other timers
            }

            _databaseContext.Timers.Remove(timer);
            await _databaseContext.SaveChangesAsync();

            _semaphore.Release();
            if (scheduleNext)
            {
                ScheduleNext();
            }
        }
    }
}
