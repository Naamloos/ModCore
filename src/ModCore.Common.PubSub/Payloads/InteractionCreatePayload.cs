using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.PubSub.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ModCore.Common.PubSub.Payloads
{
    [EventChannel(EventChannels.InteractionReceived)]
    public record InteractionCreatePayload : IPubSubPayload
    {
        [JsonPropertyName("shard_id")]
        public required int ShardId { get; set; }

        [JsonPropertyName("interaction")]
        public required Interaction Interaction { get; set; }
    }
}
