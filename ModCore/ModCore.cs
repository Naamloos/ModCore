using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using ModCore.Api;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Listeners;
using Newtonsoft.Json;

namespace ModCore
{
    internal class ModCore
    {
        private DatabaseContextBuilder GlobalContextBuilder { get; set; }
        private DiscordClient FirstClient { get; set; }
        public CommandsNextExtension FirstCNext { get; set; }

        internal Settings Settings { get; private set; }
        internal SharedData SharedData { get; private set; }
        public static List<ModCoreShard> Shards { get; set; }
        private CancellationTokenSource CTS { get; set; }
        private Perspective PerspectiveApi { get; set; }

        internal async Task InitializeAsync(string[] args)
        {
            if (!File.Exists("settings.json"))
            {
                var json = JsonConvert.SerializeObject(new Settings(), Formatting.Indented);
                File.WriteAllText("settings.json", json, new UTF8Encoding(false));
                Console.WriteLine("Config file was not found, a new one was generated. Fill it with proper values and rerun this program");
                Console.ReadKey();
                return;
            }

            var input = File.ReadAllText("settings.json", new UTF8Encoding(false));
            Settings = JsonConvert.DeserializeObject<Settings>(input);
            GlobalContextBuilder = Settings.Database.CreateContextBuilder();
            PerspectiveApi = new Perspective(Settings.PerspectiveToken);

            Shards = new List<ModCoreShard>();
            InitializeSharedData(args);

            for (var i = 0; i < Settings.ShardCount; i++)
            {
                var shard = new ModCoreShard(Settings, i, SharedData);
                shard.Initialize();
                Shards.Add(shard);
                if (i == 0)
                {
                    FirstClient = shard.Client;
                    FirstCNext = shard.Commands;
                }
            }
            
            await InitializeDatabaseAsync();

            foreach (var shard in Shards)
                await shard.RunAsync();

            await WaitForCancellation();

			foreach (var shard in Shards)
				await shard.DisconnectAndDispose();

			this.SharedData.CTS.Dispose();
			this.SharedData.TimerData?.Cancel?.Cancel();
			this.SharedData.TimerSempahore?.Dispose();
        }

        private async Task InitializeDatabaseAsync()
        {
            // add command id mappings if they don't already exist
            var modifications = new List<string>();
            using (var db = this.CreateGlobalContext())
            {
                foreach (var (name, _) in FirstCNext.RegisteredCommands.SelectMany(ErrorLog.CommandSelector).Distinct())
                {
                    //Console.WriteLine("Command: " + name);
                    if (db.CommandIds.FirstOrDefault(e => e.Command == name) != null) continue;
                    
                    modifications.Add(name);
                    await db.CommandIds.AddAsync(new DatabaseCommandId()
                    {
                        Command = name
                    });
                }                
                await db.SaveChangesAsync();
            }
            
        }

        /// <summary>
        /// Initialized the SharedData we need for the shards.
        /// </summary>
        private void InitializeSharedData(string[] args)
        {
            CTS = new CancellationTokenSource();
            SharedData = new SharedData {
                CTS = CTS,
                ProcessStartTime = Process.GetCurrentProcess().StartTime,
                Perspective = PerspectiveApi,
                BotManagers = Settings.BotManagers,
				DefaultPrefix = Settings.DefaultPrefix
            };
            if (args.Length == 2) {
                SharedData.StartNotify = (ulong.Parse(args[0]), ulong.Parse(args[1]));
            }
        }

        public async Task WaitForCancellation()
        {
            while (!CTS.IsCancellationRequested)
                await Task.Delay(500);
        }

        public DatabaseContext CreateGlobalContext()
        {
            return GlobalContextBuilder.CreateContext();
        }
    }
}
