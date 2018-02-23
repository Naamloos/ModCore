﻿using System;
using System.Collections.Generic;
using System.Threading;
using ModCore.Api;

namespace ModCore.Entities
{
    public class SharedData
    {
        public CancellationTokenSource CTS { get; internal set; }
        public DateTime ProcessStartTime { get; internal set; }
        public SemaphoreSlim TimerSempahore { get; internal set; }
        public TimerData TimerData { get; internal set; }
        public Perspective Perspective { get; internal set; }
        public (ulong guild, ulong channel) StartNotify { get; internal set; }
        public List<ulong> BotManagers { get; internal set; }

        public SharedData()
        {
            this.TimerSempahore = new SemaphoreSlim(1, 1);
        }
    }
}
