using System;
using System.Threading;

namespace ModCore.Entities
{
    public class SharedData
    {
        public CancellationTokenSource CTS { get; private set; }
        public DateTime ProcessStartTime { get; private set; }
        public SemaphoreSlim TimerSempahore { get; private set; }
        public TimerData TimerData { get; set; }

        public SharedData(CancellationTokenSource cts, DateTime processStartTime)
        {
            CTS = cts;
            ProcessStartTime = processStartTime;
            this.TimerSempahore = new SemaphoreSlim(1, 1);
        }
    }
}
