using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ModCore.Entities;
using Newtonsoft.Json;
using ModCore.Api;

namespace ModCore
{
    internal class ModCore
    {
        internal Settings Settings { get; private set; }
        internal SharedData SharedData { get; private set; }
        private List<ModCoreShard> Shards { get; set; }
        private CancellationTokenSource CTS { get; set; }
        private Perspective PerspectiveApi { get; set; }

        internal async Task InitializeAsync()
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
            PerspectiveApi = new Perspective(Settings.PerspectiveToken);

            Shards = new List<ModCoreShard>();
            InitializeSharedData();

            for (var i = 0; i < Settings.ShardCount; i++)
            {
                var shard = new ModCoreShard(Settings, i, SharedData);
                shard.Initialize();
                Shards.Add(shard);
            }

            foreach (var shard in Shards)
                await shard.RunAsync();

            await WaitForCancellation();
        }

        /// <summary>
        /// Initialized the SharedData we need for the shards.
        /// </summary>
        private void InitializeSharedData()
        {
            CTS = new CancellationTokenSource();
            var pst = Process.GetCurrentProcess().StartTime;
            SharedData = new SharedData(CTS, pst, PerspectiveApi);
        }

        public async Task WaitForCancellation()
        {
            while (!CTS.IsCancellationRequested)
                await Task.Delay(500);
        }
    }
}
