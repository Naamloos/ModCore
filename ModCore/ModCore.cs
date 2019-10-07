using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModCore.Api;
using ModCore.CoreApi;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Listeners;
using Newtonsoft.Json;
using Startup = ModCore.CoreApi.Startup;

namespace ModCore
{
    public class ModCore
    {
		public Settings Settings { get; private set; }
		public SharedData SharedData { get; private set; }
		public List<ModCoreShard> Shards { get; set; }
	    
	    private DatabaseContextBuilder GlobalContextBuilder { get; set; }
        private CancellationTokenSource CTS { get; set; }
        private Perspective PerspectiveApi { get; set; }
		private Strawpoll Strawpoll { get; set; }

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
			Strawpoll = new Strawpoll();

            Shards = new List<ModCoreShard>();
            InitializeSharedData(args);

	        // cnext data that is consistent across shards, so it's fine to share it
	        for (var i = 0; i < Settings.ShardCount; i++)
            {
                var shard = new ModCoreShard(Settings, i, SharedData);
                shard.Initialize();
                Shards.Add(shard);
	            if (i == 0)
	            {
		            SharedData.Initialize(shard);
	            }
            }

	        await InitializeDatabaseAsync();

	        foreach (var shard in Shards)
		        await shard.RunAsync();

	        await BuildWebHost().RunAsync(CTS.Token);

			await WaitForCancellation();

			foreach (var shard in Shards)
				await shard.DisconnectAndDispose();

			this.SharedData.CTS.Dispose();
			this.SharedData.TimerData.Cancel.Cancel();
			this.SharedData.TimerSempahore.Dispose();
        }

        /// <summary>
        /// Initialized the SharedData we need for the shards.
        /// </summary>
        private void InitializeSharedData(string[] args)
        {
            CTS = new CancellationTokenSource();
	        SharedData = new SharedData
	        {
		        CTS = CTS,
		        ProcessStartTime = Process.GetCurrentProcess().StartTime,
		        Perspective = PerspectiveApi,
		        BotManagers = Settings.BotManagers,
		        DefaultPrefix = Settings.DefaultPrefix,
		        ModCore = this,
				Strawpoll = this.Strawpoll
	        };
	        if (args.Length == 2) {
                SharedData.StartNotify = (ulong.Parse(args[0]), ulong.Parse(args[1]));
            }
        }

	    private async Task InitializeDatabaseAsync()
	    {
		    // add command id mappings if they don't already exist
		    var modifications = new List<string>();
		    using (var db = this.CreateGlobalContext())
		    {
			    foreach (var (name, _) in SharedData.Commands)
			    {
				    if (db.CommandIds.FirstOrDefault(e => e.Command == name) != null) continue;
				    Console.WriteLine($"Registering new command in db: {name}");
				    
				    modifications.Add(name);
				    await db.CommandIds.AddAsync(new DatabaseCommandId()
				    {
					    Command = name
				    });
			    }
			    await db.SaveChangesAsync();
		    }
            
	    }
        public async Task WaitForCancellation()
        {
            while (!CTS.IsCancellationRequested)
                await Task.Delay(500);
        }

		private IHost BuildWebHost()
		{
			var container = new CoreContainer
			{
				mcore = this
			};

			var mservice = new ServiceDescriptor(container.GetType(), container);

			return new HostBuilder()
				.UseContentRoot(Directory.GetCurrentDirectory())
				.ConfigureWebHostDefaults(webBuilder => {
					webBuilder.UseStartup<Startup>()
						.ConfigureServices(x => x.Add(mservice))
						.UseUrls("http://0.0.0.0:6969");
				})
				.Build();
		}
	    
	    public DatabaseContext CreateGlobalContext()
	    {
		    return GlobalContextBuilder.CreateContext();
	    }
	}
}
