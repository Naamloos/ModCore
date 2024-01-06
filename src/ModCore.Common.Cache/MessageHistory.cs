using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Cache
{
    public record MessageHistory
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("history")]
        public List<MessageState> History { get; set; }
    }

    public record MessageState
    {
        [JsonPropertyName("change_type")]
        public MessageChangeType ChangeType { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTimeOffset ChangeTimestamp { get; set; }

        [JsonPropertyName("state")]
        public Message? State { get; set; }
    }

    public enum MessageChangeType
    {
        Initial = 0,
        Update = 1,
        Delete = 2,
        BulkDelete = 3
    }
}
