using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using ModCore.Commands;
using ModCore.Entities;
using ModCore.Logic;
using System;
using System.Threading.Tasks;

namespace ModCore
{
    public class ModCoreShard
    {
        public int ShardId { get; private set; }
        public StartTimes StartTimes { get; private set; }

        public DiscordClient Client { get; private set; }
        public InteractivityModule Interactivity { get; private set; }
        public CommandsNextModule Commands { get; private set; }

        private SharedData ShardData { get; set; }
        internal Settings Settings { get; }


        public ModCoreShard(Settings settings, int id, SharedData sharedData)
        {
            this.Settings = settings;
            this.ShardData = sharedData;
        }

        internal void Initialize()
        {

            // Initialize the DiscordClient
            this.Client = new DiscordClient(new DiscordConfiguration
            {
                AutoReconnect = true,
                EnableCompression = true,
                LargeThreshold = 250,
                LogLevel = LogLevel.Debug,
                Token = Settings.Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                ShardCount = this.Settings.ShardCount,
                ShardId = this.ShardId
            });

            this.Interactivity = Client.UseInteractivity();


            // Store the Start Times to use in DI
            // SocketStartTime will be updated in the SocketOpened event,
            // For now we just need to make sure its not null.
            StartTimes = new StartTimes(ShardData.ProcessStartTime, ShardData.ProcessStartTime);

            // Add the instances we need to dependencies
            var deps = new DependencyCollectionBuilder()
                .AddInstance(this.ShardData)
                .AddInstance(this.Settings)
                .AddInstance(this.Interactivity)
                .AddInstance(this.StartTimes)
                .Build();

            this.Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                CaseSensitive = false,
                EnableDefaultHelp = true,
                EnableDms = false,
                EnableMentionPrefix = true,
                StringPrefix = Settings.Prefix,
                Dependencies = deps
            });

            this.Commands.RegisterCommands<Main>();
            this.Commands.RegisterCommands<Owner>();

            // Update the SocketStartTime
            this.Client.SocketOpened += async () =>
            {
                await Task.Yield();
                StartTimes.SocketStartTime = DateTime.Now;
            };

            this.Client.Ready += Client_Ready;

            this.Commands.CommandErrored += Commands_CommandErrored;

            AsyncListenerHandler.InstallListeners(Client, this);
        }

        private Task Client_Ready(ReadyEventArgs e)
        {
           Client.UpdateStatusAsync(new Game($"I'm on {this.Settings.ShardCount} shards! This is Shard {++this.ShardId}!"));
           return Task.Delay(0);
        }

        private Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Critical, "Commands", e.Exception.ToString(), DateTime.Now);
            return Task.Delay(0);
        }

        public async Task RunAsync()
        {
            await this.Client.ConnectAsync();
        }

        internal async Task DisconnectAndDispose()
        {
            await this.Client.DisconnectAsync();
            this.Client.Dispose();
        }
    }
}
