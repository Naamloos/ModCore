using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Timers
{
    public enum TimerTypes
    {
        /// <summary>
        /// Triggered by a reminder set by a user.
        /// </summary>
        Reminder = 0,

        /// <summary>
        /// Triggered by an expiring tempban
        /// </summary>
        Unban = 1,

        /// <summary>
        /// Triggered when the data retention period for a guild expires, 
        /// meaning guild data gets deleted.
        /// </summary>
        GuildDataDeletion = 2
    }
}
