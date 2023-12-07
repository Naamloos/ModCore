using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Gateway.Events;
using ModCore.Common.Discord.Rest;
using ModCore.Common.Utils;
using ModCore.Services.Shard.Interactions;

namespace ModCore.Services.Shard.EventHandlers
{
    public class MessageEvents : ISubscriber<MessageCreate>, ISubscriber<InteractionCreate>
    {
        private readonly ILogger _logger;
        private readonly DiscordRest _rest;

        public MessageEvents(ILogger<StartupEvents> logger, DiscordRest rest)
        {
            _logger = logger;
            _rest = rest;
        }

        public async Task HandleEvent(MessageCreate data)
        {
            _logger.LogInformation("@{0}: {1}", data.Author.Username, data.Content);
        }

        public async Task HandleEvent(InteractionCreate data)
        {
        }
    }
}
