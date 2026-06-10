using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Common.PubSub.Payloads
{
    /// <summary>
    /// Lets the timer service know that a new timer was created and it should check if it needs to update its timers.
    /// </summary>
    public record TimerCreatedPayload
    {
        public int TimerId { get; init; }
    }
}
