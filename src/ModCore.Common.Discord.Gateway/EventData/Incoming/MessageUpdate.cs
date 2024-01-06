using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Messages;
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
    public record MessageUpdate : Message, IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake? GuildId { get; set; }

        [JsonPropertyName("member")]
        public Member Member { get; set; }
    }
}
