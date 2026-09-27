using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Enums;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record TeamMember
    {
        [JsonPropertyName("membership_state")]
        public TeamMembershipState MembershipState { get; set; }

        [JsonPropertyName("team_id")]
        public Snowflake TeamId { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }
}
