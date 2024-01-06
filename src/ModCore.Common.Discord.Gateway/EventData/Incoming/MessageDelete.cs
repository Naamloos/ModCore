using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Gateway.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Gateway.EventData.Incoming
{
    public record MessageDelete : IPublishable
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }
    }
}
