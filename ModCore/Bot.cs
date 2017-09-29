using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using ModCore.Commands;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;

namespace ModCore
{
    public class Bot
    {
        public DiscordClient Client { get; }
        public InteractivityModule Interactivity { get; }
        public CommandsNextModule Commands { get; }
        public DateTimeOffset ProgramStart { get; private set; }
        public DateTimeOffset SocketStart { get; private set; }
        public CancellationTokenSource CTS { get; }
        public Settings Settings { get; }
        public DatabaseContext Database { get; }

        public Bot(Settings settings)
        {
            // store settngs
            this.Settings = settings;

            // store initial data and state
            ProgramStart = DateTimeOffset.Now;
            CTS = new CancellationTokenSource();

            // create database connection
            this.Database = new DatabaseContext(settings.Database.BuildConnectionString());

            // create the client
            Client = new DiscordClient(new DiscordConfiguration
            {
                AutoReconnect = true,
                EnableCompression = true,
                LogLevel = LogLevel.Debug,
                Token = settings.Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true
            });

            // enable interactivity
            Interactivity = Client.UseInteractivity();

            // add dependencies
            var deps = new DependencyCollectionBuilder()
                .AddInstance(this.CTS)
                .AddInstance(this.Database)
                .Build();

            // enable commandsnext
            Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                CaseSensitive = false,
                EnableDefaultHelp = true,
                EnableDms = false,
                EnableMentionPrefix = true,
                StringPrefix = settings.DefaultPrefix,
                Dependencies = deps
            });

            // register commands
            Commands.RegisterCommands<Main>();
            Commands.RegisterCommands<Owner>();

            // register event handlers
            Client.SocketOpened += () =>
            {
                SocketStart = DateTimeOffset.Now;
                return Task.CompletedTask;
            };

            Commands.CommandErrored += e =>
            {
                e.Context.Client.DebugLogger.LogMessage(LogLevel.Critical, "Commands", e.Exception.ToString(), DateTime.Now);
                return Task.CompletedTask;
            };
            
            AsyncListenerHandler.InstallListeners(Client, this);
        }

        public async Task RunAsync()
        {
            await Client.ConnectAsync();

            // run indefinitely
            try
            {
                await Task.Delay(-1, CTS.Token);
            }
            catch (Exception) { }

            await Client.DisconnectAsync();
            Client.Dispose();
            CTS.Dispose();
        }
    }
}
