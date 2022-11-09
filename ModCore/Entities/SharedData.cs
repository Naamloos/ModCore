using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using ModCore.Database.JsonEntities;
using ModCore.Listeners;

namespace ModCore.Entities
{
    public class SharedData
    {
        public CancellationTokenSource CancellationTokenSource { get; internal set; }
        public DateTime ProcessStartTime { get; internal set; }
        public SemaphoreSlim TimerSempahore { get; internal set; }
        public TimerData TimerData { get; internal set; }
        public string DefaultPrefix { get; internal set; }
		public int ReadysReceived { get; internal set; } = 0;
		public List<Permissions> AllPermissions { get; internal set; } = new List<Permissions>();
        public ConcurrentDictionary<ulong, DiscordMessage> DeletedMessages = new ConcurrentDictionary<ulong, DiscordMessage>();
        public ConcurrentDictionary<ulong, DiscordMessage> EditedMessages = new ConcurrentDictionary<ulong, DiscordMessage>();

        public ModCore ModCore;

        public SharedData()
        {
            this.TimerSempahore = new SemaphoreSlim(1, 1);
        }
	}
}