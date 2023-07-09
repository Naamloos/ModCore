using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Gateway.Events;
using ModCore.Common.Discord.Rest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Gateway.EventData.Incoming
{
    public class MessageCreate : Message, IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake? GuildId { get; set; }

        [JsonPropertyName("member")]
        public JsonObject? Member { get; set; }

        [JsonPropertyName("mentions")]
        public User[] Mentions { get; set; }
    }
}
