using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Rest
{
    internal class RateLimitBucket
    {
        private SemaphoreSlim semaphore;
        private ManualResetEventSlim manualResetEvent;

        public RateLimitBucket()
        {
            // Ensure the bucket is being accessed sequentially
            manualResetEvent = new ManualResetEventSlim();
            manualResetEvent.Set();
        }

        public async Task WaitAsync()
        {
            // If rate limit hits 0, we wait for it to unblock.
            manualResetEvent.Wait();
        }

        public async Task SignalDoneAsync(int remaining, float reset_after)
        {
            if(remaining < 1)
            {
                manualResetEvent.Reset();

                _ = Task.Delay((int)(reset_after * 1000)).ContinueWith(async x =>
                {
                    reset();
                });
            }
            else
            {
                reset();
            }
        }

        private void reset()
        {
            // Unlock the manualResetEvent when rate limit resets
            if (!manualResetEvent.IsSet)
            {
                manualResetEvent.Set();
            }
        }
    }
}
