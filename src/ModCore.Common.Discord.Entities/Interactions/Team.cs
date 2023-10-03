using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record Team
    {
        [JsonPropertyName("icon")]
        public string? Icon { get; set; }

        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("members")]
        public List<TeamMember> Members { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("owner_user_id")]
        public Snowflake OwnerUserId { get; set; }
    }
}
