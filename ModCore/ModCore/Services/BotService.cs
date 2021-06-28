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
        private DiscordClient client;

        public BotService(DiscordClient client, CommandsNextExtension cnext)
        {
            this.client = client;
            cnext.RegisterCommands<GeneralModule>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await client.ConnectAsync(new DiscordActivity("over your memes", ActivityType.Watching), UserStatus.DoNotDisturb);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await client.DisconnectAsync();
        }
    }
}
