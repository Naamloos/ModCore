using ModCore.Common.Database.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Tools.DatabaseMigrator.ClassicDatabase.JsonEntities
{
    /// <summary>
    /// Base interface representing all data that can be attached to a timer.
    /// </summary>
    public interface ITimerData { }

    /// <summary>
    /// Represents reminder data for a timer.
    /// </summary>
    public class TimerReminderData : ITimerData
    {
        /// <summary>
        /// Gets or sets the reminder text set by the user.
        /// </summary>
        [JsonProperty("text")]
        public string ReminderText { get; set; }

        /// <summary>
        /// Gets or sets the original message ID set for this reminder.
        /// </summary>
        [JsonProperty("message")]
        public ulong? MessageId { get; set; } = null;

        [JsonProperty("snoozed")]
        public bool Snoozed { get; set; } = false;

        [JsonProperty("originalUnix")]
        public long OriginalUnix { get; set; } = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        [JsonProperty("snoozecontext")]
        public string SnoozedContext { get; set; } = "";
    }

    /// <summary>
    /// Represents unban data for a timer.
    /// </summary>
    public class TimerUnbanData : ITimerData
    {
        /// <summary>
        /// Gets or sets the unbanned user's Id.
        /// </summary>
        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("displayname")]
        public string DisplayName { get; set; }
    }

    /// <summary>
    /// Represents timer action type.
    /// </summary>
    public enum TimerActionType
    {
        Unknown = 0, // Action type that is not known
        Reminder = 1, // Reminders
        Unban = 2, // Temp ban unban action
        [Obsolete("Timeouts are now a built-in Discord feature.")]
        Unmute = 3, // Temp mute unmute action
        [Obsolete("Nobody used this.")]
        Pin = 4, // Timed pin action
        [Obsolete("Nobody used this.")]
        Unpin = 5, // Temporary pin unpin action
    }
}
