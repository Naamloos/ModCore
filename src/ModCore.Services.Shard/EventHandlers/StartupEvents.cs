using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Gateway.Events;
using ModCore.Common.Discord.Rest;

namespace ModCore.Services.Shard.EventHandlers
{
    /// <summary>
    /// This event handler handles anything that should be ran around startup.
    /// </summary>
    public class StartupEvents : ISubscriber<Ready>, ISubscriber<Hello>, ISubscriber<GuildCreate>
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
            if (application.Success)
            {
                _logger.LogInformation("Application is registered under ID {0}. Owner username is {1}.", data.Application.Id, application.Value!.Owner!.Value.Username);
            }
            else
            {
                _logger.LogCritical("Failed to fetch application info!");
            }

            var resp = await _rest.BulkOverwriteGlobalApplicationCommandsAsync(data.Application.Id, new ApplicationCommand()
            {
                Name = "about", 
                Type = ApplicationCommandType.ChatInput,
                Description = "Information about ModCore",
                CanBeUsedInDM = false
            });

            var respc = await resp.HttpResponse.Content.ReadAsStringAsync();
        }

        public async Task HandleEvent(GuildCreate data)
        {
            _logger.LogInformation("Guild {0} has {1} members! Sent from event handler.", data.Name, data.MemberCount);
        }
    }
}
