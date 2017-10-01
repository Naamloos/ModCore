using System;

namespace ModCore.Entities
{
    public class StartTimes
    {
        public DateTimeOffset ProcessStartTime { get; private set; }
        public DateTimeOffset SocketStartTime { get; set; }

        public StartTimes(DateTime processStartTime, DateTime socketStartTime)
        {
            this.ProcessStartTime = processStartTime;
            this.SocketStartTime = socketStartTime;
        }

    }
}
