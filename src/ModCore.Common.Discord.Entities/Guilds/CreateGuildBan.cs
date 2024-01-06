using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record CreateGuildBan
    {
        [JsonPropertyName("delete_message_days")]
        public Optional<int> DeleteMessageDays { get; set; }

        [JsonPropertyName("delete_message_seconds")]
        public Optional<int> DeleteMessageSeconds { get; set; }
    }
}
