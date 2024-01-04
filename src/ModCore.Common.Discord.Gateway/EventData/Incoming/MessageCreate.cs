﻿using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Gateway.Events;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Gateway.EventData.Incoming
{
    public record MessageCreate : Message, IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake? GuildId { get; set; }

        [JsonPropertyName("member")]
        public Member Member { get; set; }
    }
}