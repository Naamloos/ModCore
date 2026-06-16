using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Gateway;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Gateway.Events;
using ModCore.Common.PubSub;
using ModCore.Common.PubSub.Payloads;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModCore.Services.GatewayIngest.Subscribers
{
    public class InteractionCreateSubscriber : ISubscriber<InteractionCreate>
    {
        public required Gateway Gateway { get; set; }

        private ILogger<InteractionCreateSubscriber> _logger;
        private readonly PubSubService _pubsub;

        public InteractionCreateSubscriber(ILogger<InteractionCreateSubscriber> logger, PubSubService pubsub)
        {
            this._logger = logger;
            this._pubsub = pubsub;
        }

        public async ValueTask HandleEvent(InteractionCreate data)
        {
            _logger.LogInformation("Received InteractionCreate event with ID {Id} and Type {Type}", data.Id, data.Type);
            await this._pubsub.PublishAsync<InteractionCreatePayload>(new()
            {
                ShardId = this.Gateway.ShardId,
                Interaction = data
            });
        }
    }
}
