using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using ModCore.Database;
using Newtonsoft.Json;

namespace ModCore.Entities
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
    }

    /// <summary>
    /// Represents ban data for a timer.
    /// </summary>
    public class TimerBanData : ITimerData
    {

    }

    /// <summary>
    /// Represents mute data for a timer.
    /// </summary>
    public class TimerMuteData : ITimerData
    {

    }

    /// <summary>
    /// Represents timer action type.
    /// </summary>
    public enum TimerActionType : int
    {
        Unknown = 0,
        Reminder = 1,
        Ban = 2,
        Mute = 3
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
            this.DbTimer.DispatchAt;

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
        public CancellationToken CancelToken => this.Cancel.Token;

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
            this.Timer = task;
            this.DbTimer = dbtimer;
            this.Context = context;
            this.Database = db;
            this.Shared = shared;
            this.Cancel = cts;
        }
    }
}
