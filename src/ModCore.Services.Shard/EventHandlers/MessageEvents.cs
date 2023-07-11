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
    public class MessageEvents : ISubscriber<MessageCreate>
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
            
            if(data.Content == "$modcore")
            {
                var resp = await _rest.CreateMessageAsync(data.ChannelId, $"Ayo ima be real <@{data.Author.Id}>, this text command is just a test man..");
            }
            else if(data.Content == "$oops")
            {
                throw new InsufficientExecutionStackException("dick too small");
            }
        }
    }
}
