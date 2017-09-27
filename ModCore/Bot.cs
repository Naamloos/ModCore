using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using ModCore.Commands;
using ModCore.Entities;
using ModCore.Logic;

namespace ModCore
{
    public class Bot
    {
        public readonly DiscordClient Client;
        public readonly InteractivityModule Interactivity;
        public readonly CommandsNextModule Commands;
        public DateTimeOffset ProgramStart;
        public DateTimeOffset SocketStart;
        public readonly CancellationTokenSource CTS;
        public readonly Settings Settings;

        public Bot(Settings settings)
        {
            this.Settings = settings;
            ProgramStart = DateTimeOffset.Now;
            Client = new DiscordClient(new DiscordConfiguration
            {
                AutoReconnect = true,
                EnableCompression = true,
                LogLevel = LogLevel.Debug,
                Token = settings.Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true
            });

            Interactivity = Client.UseInteractivity();

            var deps = new DependencyCollectionBuilder().AddInstance(this).Build();

            Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                CaseSensitive = false,
                EnableDefaultHelp = true,
                EnableDms = false,
                EnableMentionPrefix = true,
                StringPrefix = settings.Prefix,
                Dependencies = deps
            });

            Commands.RegisterCommands<Main>();
            Commands.RegisterCommands<Owner>();

            CTS = new CancellationTokenSource();

            Client.SocketOpened += async () =>
            {
                await Task.Yield();
                SocketStart = DateTimeOffset.Now;
            };

            Commands.CommandErrored += async e =>
            {
                e.Context.Client.DebugLogger.LogMessage(LogLevel.Critical, "Commands", e.Exception.ToString(), DateTime.Now);
            };
            
            AsyncListenerHandler.InstallListeners(Client, this);
        }

        public async Task RunAsync()
        {
            await Client.ConnectAsync();
            await WaitForCancellation();
            await Client.DisconnectAsync();
            Client.Dispose();
            CTS.Dispose();
        }

        public async Task WaitForCancellation()
        {
            while (!CTS.IsCancellationRequested)
            {
                await Task.Delay(500);
            }
        }
    }
}
