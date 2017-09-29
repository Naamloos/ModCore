using System;
using System.Threading;

namespace ModCore.Entities
{
    public class SharedData
    {
        public CancellationTokenSource CTS { get; private set; }
        public DateTime ProcessStartTime { get; private set; }

        public SharedData(CancellationTokenSource cts, DateTime processStartTime)
        {
            CTS = cts;
            ProcessStartTime = processStartTime;
        }
    }
}
