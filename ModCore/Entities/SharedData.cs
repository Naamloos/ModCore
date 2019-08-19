using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using ModCore.Api;
using ModCore.CoreApi;
using ModCore.Listeners;

namespace ModCore.Entities
{
    public class SharedData
    {
        public CancellationTokenSource CTS { get; internal set; }
        public DateTime ProcessStartTime { get; internal set; }
        public SemaphoreSlim TimerSempahore { get; internal set; }
        public TimerData TimerData { get; internal set; }
        public Perspective Perspective { get; internal set; }
		public Strawpoll Strawpoll { get; internal set; }
		public DiscordBots DiscordBots { get; internal set; }
		public BotsDiscordPl BotsDiscordPl { get; internal set; }
        public (ulong guild, ulong channel) StartNotify { get; internal set; }
        public List<ulong> BotManagers { get; internal set; }
        public string DefaultPrefix { get; internal set; }
		public int ReadysReceived { get; internal set; } = 0;
		public List<Permissions> AllPerms { get; internal set; } = new List<Permissions>();
        public ConcurrentDictionary<ulong, DiscordMessage> DeletedMessages = new ConcurrentDictionary<ulong, DiscordMessage>();
        public MCoreEmojis Emojis;

        /// <summary>
        /// Every command, top-level or not, along with full qualified name.
        /// </summary>
        public (string name, Command cmd)[] Commands { get; set; }

        public string ApiToken = null;

        public ModCore ModCore;

        public SharedData()
        {
            this.TimerSempahore = new SemaphoreSlim(1, 1);
        }

        public void Initialize(ModCoreShard shard)
        {
            Commands = shard.Commands.RegisteredCommands.SelectMany(SelectCommandsFromDict).Distinct().ToArray();
            Emojis = MCoreEmojis.LoadEmojis(shard.Client);
        }

        private static IEnumerable<(string name, Command cmd)> SelectCommandsFromDict(KeyValuePair<string, Command> c)
            => CommandSelector(c.Value);

        private static IEnumerable<(string name, Command cmd)> CommandSelector(Command c)
        {
            yield return (c.QualifiedName, c);
            if (!(c is CommandGroup group)) yield break;
            if (group.Children == null) yield break;

            foreach (var cmd in group.Children)
            {
                foreach (var res in CommandSelector(cmd))
                {
                    yield return res;
                }
            }
        }
	}
}