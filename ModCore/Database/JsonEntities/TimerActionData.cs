using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using ModCore.Entities;
using Newtonsoft.Json;

namespace ModCore.Database.JsonEntities
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

        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }
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

    /// <summary>
    /// Represents information for the timer dispatcher.
    /// </summary>
    public class TimerData
    {
        /// <summary>
        /// Gets the current timer task.
        /// </summary>
        public Task Timer { get; }

        /// <summary>
        /// Gets the database timer instance.
        /// </summary>
        public DatabaseTimer DbTimer { get; }

        /// <summary>
        /// Gets the time at which this timer is to be dispatched.
        /// </summary>
        public DateTimeOffset DispatchTime =>
            DbTimer.DispatchAt;

        /// <summary>
        /// Gets the client for which this timer is to be dispatched.
        /// </summary>
        public DiscordClient Context { get; }

        /// <summary>
        /// Gets the database context builder for this timer.
        /// </summary>
        public DatabaseContextBuilder Database { get; }

        /// <summary>
        /// Gets the shard shared data.
        /// </summary>
        public SharedData Shared { get; }

        /// <summary>
        /// Gets the cancel method for this timer.
        /// </summary>
        public CancellationTokenSource Cancel { get; }

        /// <summary>
        /// Gets the cancel token for this timer.
        /// </summary>
        public CancellationToken CancelToken => Cancel.Token;

        /// <summary>
        /// Creates new timer dispatcher information model.
        /// </summary>
        /// <param name="task">Task which will be dispatched.</param>
        /// <param name="dbtimer">Database model with data about the timer.</param>
        /// <param name="context">Context in which this timer is to be dispatched.</param>
        /// <param name="db">Database connection builder for this timer.</param>
        /// <param name="shared">Data shared across the shard.</param>
        /// <param name="cts">Cancellation token source for this timer.</param>
        public TimerData(Task task, DatabaseTimer dbtimer, DiscordClient context, DatabaseContextBuilder db, SharedData shared, CancellationTokenSource cts)
        {
            Timer = task;
            DbTimer = dbtimer;
            Context = context;
            Database = db;
            Shared = shared;
            Cancel = cts;
        }
    }
}
