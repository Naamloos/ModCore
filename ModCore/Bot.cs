using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using ModCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModCore
{
    public class Bot
    {
        public DiscordClient Client;
        public InteractivityModule Interactivity;
        public CommandsNextModule Commands;
        public DateTimeOffset ProgramStart;
        public DateTimeOffset SocketStart;
        public CancellationTokenSource CTS;

        public Bot(Settings settings)
        {
            ProgramStart = DateTimeOffset.Now;
            Client = new DiscordClient(new DiscordConfiguration()
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

            Commands = Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                CaseSensitive = false,
                EnableDefaultHelp = true,
                EnableDms = false,
                EnableMentionPrefix = true,
                StringPrefix = settings.Prefix,
                Dependencies = deps
            });

            Commands.RegisterCommands<ModCore.Commands.Main>();
            Commands.RegisterCommands<ModCore.Commands.Owner>();

            CTS = new CancellationTokenSource();

            Client.SocketOpened += async () =>
            {
                await Task.Yield();
                SocketStart = DateTimeOffset.Now;
            };
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
            return;
        }
    }
}
