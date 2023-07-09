using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Gateway.Events;
using ModCore.Common.Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Shard.EventHandlers
{
    public class StartupEvents : ISubscriber<Ready>, ISubscriber<Hello>
    {
        private readonly ILogger _logger;
        private readonly DiscordRest _rest;

        public StartupEvents(ILogger<StartupEvents> logger, DiscordRest rest)
        {
            _logger = logger;
            _rest = rest;
        }

        public async Task HandleEvent(Hello data)
        {
            _logger.LogInformation("Hello from event handler!");
        }

        public async Task HandleEvent(Ready data)
        {
            _logger.LogInformation($"Ready from event handler! User is {data.User.Username}");
            var naamloos = await _rest.GetUserAsync(127408598010560513);
            if (naamloos.Success)
            {
                _logger.LogInformation($"Fetched Naamloos user in Ready, username is {naamloos.Value.Username}");
            }
            else
            {
                _logger.LogInformation($"User fetch request failed: {naamloos.HttpResponse.StatusCode}");
            }
        }
    }
}
