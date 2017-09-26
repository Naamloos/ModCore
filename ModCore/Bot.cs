using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using ModCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public Settings settings;

        public Bot(Settings settings)
        {
            this.settings = settings;
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

            Client.MessageCreated += async (e) =>
            {
                if (settings.BlockInvites && (e.Channel.PermissionsFor(e.Author as DiscordMember) & Permissions.ManageMessages) == 0)
                {
                    var m = Regex.Match(e.Message.Content, "discord(\\.gg|app\\.com\\/invite)\\/.+");
                    if (m.Success)
                    {
                        await e.Message.DeleteAsync("Discovered invite and deleted message");
                    }
                }
            };

            Commands.CommandErrored += async (e) =>
            {
                e.Context.Client.DebugLogger.LogMessage(LogLevel.Critical, "Commands", e.Exception.ToString(), DateTime.Now);
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
