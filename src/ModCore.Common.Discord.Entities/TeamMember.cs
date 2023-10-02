using ModCore.Common.Discord.Rest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities
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

    public enum TeamMembershipState
    {
        Invited = 1,
        Accepted = 2
    }
}
