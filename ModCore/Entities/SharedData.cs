using ModCore.Api;
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
        public Perspective Perspective { get; private set; }

        public SharedData(CancellationTokenSource cts, DateTime processStartTime, Perspective psp)
        {
            this.CTS = cts;
            this.ProcessStartTime = processStartTime;
            this.TimerSempahore = new SemaphoreSlim(1, 1);
            this.Perspective = psp;
        }
    }
}
