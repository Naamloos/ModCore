using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using ModCore.Services;
using Microsoft.Extensions.DependencyInjection;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using ModCore.Modules;

namespace ModCore.Services
{
    public class BotService : IHostedService
    {
        private BotMetaService meta;

        private DiscordClient client;

        public BotService(DiscordClient client, CommandsNextExtension cnext, BotMetaService meta)
        {
            this.meta = meta;
            this.client = client;
            cnext.RegisterCommands<GeneralModule>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            meta.StartTime = DateTimeOffset.Now;
            client.SocketOpened += OnSocketConnect;
            await client.ConnectAsync(new DiscordActivity("over your memes", ActivityType.Watching), UserStatus.DoNotDisturb);
        }

        private async Task OnSocketConnect(DiscordClient sender, DSharpPlus.EventArgs.SocketEventArgs e)
        {
            await Task.Yield();
            meta.SocketStartTime = DateTimeOffset.Now;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await client.DisconnectAsync();
        }
    }
}
