using System;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;
using System.Runtime.InteropServices;
using DSharpPlus.Net.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace ModCore
{
    public class ModCoreShard
    {
        public int ShardId { get; private set; }
        public StartTimes StartTimes { get; private set; }

        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public SharedData SharedData { get; set; }
        internal Settings Settings { get; }

        public DatabaseContextBuilder Database { get; }
        
        public ModCoreShard(Settings settings, int id, SharedData sharedData)
        {
            this.Settings = settings;
            this.SharedData = sharedData;
            Database = settings.Database.CreateContextBuilder();
        }

        internal void Initialize()
        {
            // Store the Start Times to use in DI
            // SocketStartTime will be updated in the SocketOpened event,
            // For now we just need to make sure its not null.
            StartTimes = new StartTimes(SharedData.ProcessStartTime, SharedData.ProcessStartTime);

            // Initialize the DiscordClient
            this.Client = new DiscordClient(new DiscordConfiguration
            {
                AutoReconnect = true,
                GatewayCompressionLevel = GatewayCompressionLevel.Stream,
                LargeThreshold = 250,
                LogLevel = LogLevel.Debug,
                Token = Settings.Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                ShardCount = this.Settings.ShardCount,
                ShardId = this.ShardId
            });
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.OSVersion.Version <= new Version(6, 1, 7601, 65536))
            {
                // NT 6.1 (Win7 SP1)
                Client.SetWebSocketClient<WebSocket4NetCoreClient>();
            }
    
            Client.Ready += StartupNotifyHandler;
            
            Client.ClientErrored += async args =>
            {
                await Task.Yield();
                Console.WriteLine(args.Exception);
            };

            this.Interactivity = Client.UseInteractivity(new InteractivityConfiguration()
            {
                PaginationBehaviour = TimeoutBehaviour.DeleteReactions,
                PaginationTimeout = TimeSpan.FromSeconds(30),
                Timeout = TimeSpan.FromSeconds(30)
            });

            // Add the instances we need to dependencies
            var deps = new ServiceCollection()
                .AddSingleton(this.SharedData)
                //.AddInstance(this.Settings)
                .AddSingleton(this.Interactivity)
                .AddSingleton(this.StartTimes)
                .AddSingleton(this.Database)
                .BuildServiceProvider();

            // enable commandsnext
            this.Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                CaseSensitive = false,
                EnableDefaultHelp = false,
                EnableDms = false,
                EnableMentionPrefix = true,
                CustomPrefixPredicate = this.GetPrefixPositionAsync,
                Services = deps
            });

            // set the converters
            CommandsNextUtilities.RegisterConverter(new AugmentedBoolConverter());

            // register commands
            this.Commands.RegisterCommands(Assembly.GetExecutingAssembly());

            // Update the SocketStartTime
            this.Client.SocketOpened += async () =>
            {
                await Task.Yield();
                StartTimes.SocketStartTime = DateTime.Now;
            };

            // register event handlers
            this.Client.Ready += Client_Ready;

            AsyncListenerHandler.InstallListeners(Client, this);
        }
        
        private async Task StartupNotifyHandler(ReadyEventArgs e)
        {
            await Task.Yield();
            if (SharedData.StartNotify.guild == 0UL) return;
            if (e.Client.Guilds.TryGetValue(SharedData.StartNotify.guild, out var guild)) {
                _ = guild.GetChannel(SharedData.StartNotify.channel).SendMessageAsync("Heeey, VSauce here.");
            }
        }

        private Task Client_Ready(ReadyEventArgs e)
        {
            Client.UpdateStatusAsync(new DiscordActivity($"over {this.Settings.ShardCount} shard" + (this.Settings.ShardCount > 1 ? "s!" : "!"), ActivityType.Watching));
            return Task.Delay(0);
        }

        public Task RunAsync() =>
            this.Client.ConnectAsync();

        internal async Task DisconnectAndDispose()
        {
            await this.Client.DisconnectAsync();
            this.Client.Dispose();
        }

        public Task<int> GetPrefixPositionAsync(DiscordMessage msg)
        {
            GuildSettings cfg = null;
            using (var db = Database.CreateContext())
                cfg = msg.Channel.Guild.GetGuildSettings(db);
            if (cfg?.Prefix != null)
                return Task.FromResult(msg.GetStringPrefixLength(cfg.Prefix));

            return Task.FromResult(msg.GetStringPrefixLength(Settings.DefaultPrefix));
        }
    }
}
