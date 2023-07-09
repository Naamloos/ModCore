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
    /// <summary>
    /// This event handler handles anything that should be ran around startup.
    /// </summary>
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
            _logger.LogInformation("Ready from event handler! User is {0} with ID {1}.", 
                data.User.Username, data.User.Id);
            var application = await _rest.GetApplicationAsync(data.Application.Id);
            _logger.LogInformation("Application is registered under ID {0}. Owner username is {1}.", data.Application.Id, application.Value.Owner.Username);
            var naamloos = await _rest.GetUserAsync(127408598010560513);
            if (naamloos.Success)
            {
                _logger.LogInformation("Fetched Naamloos user in Ready, username is {0}", naamloos.Value.Username);
            }
            else
            {
                _logger.LogInformation("User fetch request failed: {0}", (int)naamloos.HttpResponse.StatusCode);
            }
        }
    }
}
