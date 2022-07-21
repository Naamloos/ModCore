using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using ModCore.Entities;
using Newtonsoft.Json;
using ApiStartup = ModCore.Api.ApiStartup;

namespace ModCore
{
    public class ModCore
    {
        public Settings Settings { get; private set; }

        public SharedData SharedData { get; private set; }

        public List<ModCoreShard> Shards { get; set; }

        private CancellationTokenSource CTS { get; set; }

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

            if (args.Contains("--migrate"))
            {
                Console.WriteLine("Applying migrations...");
                this.migrate();
                Console.WriteLine("Done applying migrations.");
                return;
            }

            // Saving config with same values but updated fields
            var newjson = JsonConvert.SerializeObject(Settings, Formatting.Indented);
            File.WriteAllText("settings.json", newjson, new UTF8Encoding(false));

            Shards = new List<ModCoreShard>();
            InitializeSharedData(args);

            var clnt = new DiscordRestClient(new DiscordConfiguration()
            {
                Token = Settings.Token,
                TokenType = TokenType.Bot
            });
            var gateway = await clnt.GetGatewayInfoAsync();
            clnt.Logger.LogInformation($"Recommended shard count according to Discord: {gateway.ShardCount}");
            clnt.Dispose();

            // cnext data that is consistent across shards, so it's fine to share it
            for (var i = 0; i < gateway.ShardCount; i++)
            {
                var shard = new ModCoreShard(Settings, i, SharedData);
                shard.Initialize(gateway.ShardCount);
                Shards.Add(shard);
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
                DefaultPrefix = Settings.DefaultPrefix,
                ModCore = this
            };
        }

        private async Task InitializeDatabaseAsync()
        {
            // Fix for legacy timestamp behavior. This might need a "proper" fix in the future
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public async Task WaitForCancellation()
        {
            while (!CTS.IsCancellationRequested)
                await Task.Delay(500);
        }

        private IHost BuildWebHost()
        {
            var mservice = new ServiceDescriptor(this.GetType(), this);

            return new HostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                    // {
                    //     config.AddJsonFile(
                    //         "appconfig.json", optional: false, reloadOnChange: false);
                    // });

                    webBuilder.UseStartup<ApiStartup>()
                        .ConfigureServices(x => x.Add(mservice))
                        .UseUrls("http://0.0.0.0:6969");
                })
                .Build();
        }

        private void migrate()
        {
            var db = Settings.Database.CreateContextBuilder().CreateContext();
            var pending = db.Database.GetPendingMigrations();

            if (pending.Count() < 1)
            {
                Console.WriteLine("No migrations pending.");
                return;
            }

            Console.WriteLine("Pending migrations:");
            foreach (var migration in pending)
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
                    db.Database.Migrate();
                }
                catch (Exception ex)
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
