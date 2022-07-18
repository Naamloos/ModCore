using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using Microsoft.Extensions.DependencyInjection;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Utils;
using ModCore.Utils.Extensions;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using DSharpPlus.SlashCommands;
using ModCore.Extensions;
using ModCore.Database.JsonEntities;

namespace ModCore
{
    public class ModCoreShard
    {
        public int ShardId { get; private set; }
        public StartTimes StartTimes { get; private set; }

        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public SlashCommandsExtension Slashies { get; private set; }
        public ModalExtension Modals { get; private set; }
        public ButtonExtension Buttons { get; private set; }

        public SharedData SharedData { get; set; }
		public Settings Settings { get; }

        public DatabaseContextBuilder Database { get; }

		public ModCore Parent;
        
        public ModCoreShard(Settings settings, int id, SharedData sharedData)
        {
            Settings = settings;
            SharedData = sharedData;
            Database = settings.Database.CreateContextBuilder();
            ShardId = id;
        }

        internal void Initialize()
        {
            // Store the Start Times to use in DI
            // SocketStartTime will be updated in the SocketOpened event,
            // For now we just need to make sure its not null.
            StartTimes = new StartTimes(SharedData.ProcessStartTime, SharedData.ProcessStartTime);

            // Initialize the DiscordClient
            var cfg = new DiscordConfiguration
            {
                AutoReconnect = true,
                GatewayCompressionLevel = GatewayCompressionLevel.Stream,
                LargeThreshold = 250,
                MinimumLogLevel = LogLevel.Debug,
                Token = Settings.Token,
                TokenType = TokenType.Bot,
                ShardCount = this.Settings.ShardCount,
                ShardId = this.ShardId,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildMessages | DiscordIntents.GuildMembers
            };

            this.Client = new DiscordClient(cfg);

            Client.ClientErrored += async (client, args) =>
            {
                await Task.Yield();
                Console.WriteLine(args.Exception);
            };

            this.Interactivity = Client.UseInteractivity(new InteractivityConfiguration
            {
                PollBehaviour = PollBehaviour.DeleteEmojis,
                Timeout = TimeSpan.FromSeconds(15)
            });

            // Add the instances we need to dependencies
            var deps = new ServiceCollection()
                .AddSingleton(this.SharedData)
                .AddSingleton(this.Settings)
                .AddSingleton(this.Interactivity)
                .AddSingleton(this.StartTimes)
                .AddSingleton(this.Database)
                .AddSingleton(this)
                .AddSingleton(this.Client)
                .BuildServiceProvider();

            // enable commandsnext
            this.Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                CaseSensitive = false,
                EnableDefaultHelp = true,
                EnableDms = false,
                EnableMentionPrefix = true,
                PrefixResolver = this.GetPrefixPositionAsync,
                Services = deps,
            });

            // set the converters
            this.Commands.RegisterConverter(new AugmentedBoolConverter());
			this.Commands.RegisterConverter(new CustomDiscordMessageConverter());
            this.Commands.RegisterConverter(new MentionUlongConverter());

            // register commands
            this.Commands.RegisterCommands(Assembly.GetExecutingAssembly());

			foreach(var c in this.Commands.RegisteredCommands)
			{
				var reqperm = c.Value.ExecutionChecks.Where(x => x.GetType() == typeof(RequirePermissionsAttribute));
				foreach(RequirePermissionsAttribute att in reqperm)
				{
					if (!SharedData.AllPermissions.Contains(att.Permissions))
						SharedData.AllPermissions.Add(att.Permissions);
				}
				var requsrperm = c.Value.ExecutionChecks.Where(x => x.GetType() == typeof(RequireBotPermissionsAttribute));
				foreach (RequireBotPermissionsAttribute att in requsrperm)
				{
					if(!SharedData.AllPermissions.Contains(att.Permissions))
						SharedData.AllPermissions.Add(att.Permissions);
				}
			}

            this.Slashies = this.Client.UseSlashCommands(new SlashCommandsConfiguration()
            {
                Services = deps
            });

            this.Slashies.RegisterCommands(Assembly.GetExecutingAssembly());

            // Update the SocketStartTime
            this.Client.SocketOpened += async (c, e) =>
            {
                await Task.Yield();
                StartTimes.SocketStartTime = DateTime.Now;
                await this.Slashies.RefreshCommands();
            };

            this.Modals = this.Client.UseModals(deps);
            this.Buttons = this.Client.UseButtons(deps);

            var asyncListeners = this.Client.UseAsyncListeners(deps);
            asyncListeners.RegisterListeners(this.GetType().Assembly);

            this.Client.GuildCreated += onGuildCreated;
            // register event handlers
            this.Client.Ready += onClientReady;
        }

        private async Task onGuildCreated(DiscordClient c, GuildCreateEventArgs e)
        {
            await update_status(c);
        }

        private async Task onClientReady(DiscordClient c, ReadyEventArgs e)
        {
            //await c.UpdateStatusAsync(new DiscordActivity($"over {this.Settings.ShardCount} shard" + (this.Settings.ShardCount > 1 ? "s!" : "!"), ActivityType.Watching));
            await update_status(c);
        }

        private async Task update_status(DiscordClient c)
        {
            await c.UpdateStatusAsync(new DiscordActivity($"over {c.Guilds.Count} servers! (on shard {this.ShardId})",
                ActivityType.Watching));
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
            // Force ModCore to only accept mentions.
            return Task.FromResult(-1);
        }
    }
}
