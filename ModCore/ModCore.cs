﻿using System;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModCore.Api;
using ModCore.CoreApi;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Listeners;
using ModCore.Web;
using Newtonsoft.Json;
using Startup = ModCore.Web.Startup;

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

			if (args.Contains("--migrate"))
            {
				this.migrate();
				return;
            }

			// Saving config with same values but updated fields
			var newjson = JsonConvert.SerializeObject(Settings, Formatting.Indented);
			File.WriteAllText("settings.json", newjson, new UTF8Encoding(false));

            PerspectiveApi = new Perspective(Settings.PerspectiveToken);

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

			this.SharedData.CancellationTokenSource.Dispose();
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
		        CancellationTokenSource = CTS,
		        ProcessStartTime = Process.GetCurrentProcess().StartTime,
		        Perspective = PerspectiveApi,
		        BotManagers = Settings.BotManagers,
		        DefaultPrefix = Settings.DefaultPrefix,
		        ModCore = this
	        };
        }

	    private async Task InitializeDatabaseAsync()
	    {
			// Fix for legacy timestamp behavior. This might need a "proper" fix in the future
			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
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
                    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                     {
                         config.AddJsonFile(
                             "appconfig.json", optional: false, reloadOnChange: false);
                     });

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

		private void migrate()
        {
			using var db = this.CreateGlobalContext();
			var pending = db.Database.GetPendingMigrations();

			if(pending.Count() < 1)
            {
				Console.WriteLine("No migrations pending.");
				return;
            }

			Console.WriteLine("Pending migrations:");
			foreach(var migration in pending)
            {
				Console.WriteLine(migration);
            }

			Console.WriteLine("Please ensure you have made a backup of your database. Migrate? (Y/n)");
			var response = Console.ReadKey();
			if (response.KeyChar == 'y')
			{
				Console.WriteLine("Attempting to apply migrations...");
				try
				{
					this.CreateGlobalContext().Database.Migrate();
				}
				catch(Exception ex)
                {
					Console.WriteLine("Migrations failed...");
					Console.WriteLine(ex.Message);
                }
                finally
                {
					Console.WriteLine("Migrations applied.");
				}
			}
			else
			{
				Console.WriteLine("Operation canceled.");
			}
			return;
		}
	}
}
