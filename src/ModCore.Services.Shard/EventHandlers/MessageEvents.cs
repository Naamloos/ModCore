using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Gateway.Events;
using ModCore.Common.Discord.Rest;

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

            if(data.GuildId != 438803108978753536)
            {
                return;
            }
            
            if(data.Content == "$modcore")
            {
                var responseMessage = new CreateMessage()
                {
                    Content = $"{data.Author.Mention}",
                    StickerIds = new Snowflake[] { 1158544938485698611 }
                };

                var resp = await _rest.CreateMessageAsync(data.ChannelId, responseMessage);
                if(resp.Success)
                {
                    var createdMessage = resp.Value;
                    _logger.LogInformation("Created message with new ID: {0} {1}", createdMessage.Id, createdMessage.GetJumpLink(data.GuildId));
                }
            }
            else if(data.Content == "$oops")
            {
                throw new InsufficientExecutionStackException("dick too small");
            }
        }

        public async Task HandleEvent(InteractionCreate data)
        {
            _logger.LogDebug("Incoming interaction");
        }
    }
}
