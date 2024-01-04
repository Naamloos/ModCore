using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Gateway;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Gateway.Events;
using ModCore.Common.Discord.Rest;
using ModCore.Common.InteractionFramework;
using ModCore.Services.Shard.Interactions;
using System.Reflection;

namespace ModCore.Services.Shard.EventHandlers
{
    /// <summary>
    /// This event handler handles anything that should be ran around startup.
    /// </summary>
    public class StartupEvents : ISubscriber<Ready>, ISubscriber<Hello>, ISubscriber<GuildCreate>
    {
        public Gateway Gateway { get; set; }

        private readonly ILogger _logger;
        private readonly DiscordRest _rest;
        private readonly InteractionService _interactions;

        public StartupEvents(ILogger<StartupEvents> logger, DiscordRest rest, InteractionService interactions)
        {
            _logger = logger;
            _rest = rest;
            _interactions = interactions;
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
            _interactions.RegisterCommands(Assembly.GetExecutingAssembly());

            if (application.Success)
            {
                _logger.LogInformation("Application is registered under ID {0}. Owner username is {1}.", data.Application.Id, application.Value!.Owner!.Value.Username);
                if (data.Shard.HasValue && data.Shard.Value[0] == 0)
                {
                    // Only send commands if we know we're definitely on shard 0!!
                    await _interactions.PublishCommands(application.Value.Id);
                }
            }
            else
            {
                _logger.LogCritical("Failed to fetch application info!");
            }

            _interactions.Start(Gateway);
        }

        public async Task HandleEvent(GuildCreate data)
        {
            _logger.LogInformation("Guild {0} has {1} members! Sent from event handler.", data.Name, data.MemberCount);
        }
    }
}
