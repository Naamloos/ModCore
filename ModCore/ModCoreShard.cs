﻿using System;
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
using DSharpPlus.Net.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;
using ModCore.Logic.Extensions;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;

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
                //.AddInstance(this.Settings)
                .AddSingleton(this.Interactivity)
                .AddSingleton(this.StartTimes)
                .AddSingleton(this.Database)
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

            // register commands
            this.Commands.RegisterCommands(Assembly.GetExecutingAssembly());

			foreach(var c in this.Commands.RegisteredCommands)
			{
				var reqperm = c.Value.ExecutionChecks.Where(x => x.GetType() == typeof(RequirePermissionsAttribute));
				foreach(RequirePermissionsAttribute att in reqperm)
				{
					if (!SharedData.AllPerms.Contains(att.Permissions))
						SharedData.AllPerms.Add(att.Permissions);
				}
				var requsrperm = c.Value.ExecutionChecks.Where(x => x.GetType() == typeof(RequireBotPermissionsAttribute));
				foreach (RequireBotPermissionsAttribute att in requsrperm)
				{
					if(!SharedData.AllPerms.Contains(att.Permissions))
						SharedData.AllPerms.Add(att.Permissions);
				}
			}

            // Update the SocketStartTime
            this.Client.SocketOpened += async (c, e) =>
            {
                await Task.Yield();
                StartTimes.SocketStartTime = DateTime.Now;
            };

            // register event handlers
            this.Client.Ready += Client_Ready;

            AsyncListenerHandler.InstallListeners(Client, this);
        }

        private async Task Client_Ready(DiscordClient c, ReadyEventArgs e)
        {
            await c.UpdateStatusAsync(new DiscordActivity($"over {this.Settings.ShardCount} shard" + (this.Settings.ShardCount > 1 ? "s!" : "!"), ActivityType.Watching));
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
