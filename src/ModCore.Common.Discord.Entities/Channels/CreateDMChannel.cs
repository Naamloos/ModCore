using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Channels
{
    public record CreateDMChannel
    {
        [JsonPropertyName("recipient_id")]
        public Snowflake RecipientId { get; set; }
    }
}
