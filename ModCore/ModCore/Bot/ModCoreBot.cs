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

namespace ModCore.Bot
{
    public class ModCoreBot : IHostedService
    {
        private DiscordClient client;

        public ModCoreBot(ILoggerFactory loggerFactory, IServiceProvider services, ConfigService configUtility)
        {
            var config = configUtility.GetConfig();

            this.client = new DiscordClient(new DiscordConfiguration()
            {
                LoggerFactory = loggerFactory,
                Token = config.Token,
                TokenType = TokenType.Bot
            });

            this.client.UseCommandsNext(new CommandsNextConfiguration()
            {
                Services = services,
                EnableMentionPrefix = true,
                StringPrefixes = new string[] { config.DefaultPrefix }
            });
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
