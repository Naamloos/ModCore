using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Guilds;

namespace ModCore.Common.Discord.Entities.Users
{
    public record Connection
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("type")]
        public string Type { get; set; } = default!;

        [JsonPropertyName("revoked")]
        public Optional<bool> Revoked { get; set; }

        [JsonPropertyName("integrations")]
        public Optional<Integration[]> Integrations { get; set; }

        [JsonPropertyName("verified")]
        public bool Verified { get; set; }

        [JsonPropertyName("friend_sync")]
        public bool FriendSync { get; set; }

        [JsonPropertyName("show_activity")]
        public bool ShowActivity { get; set; }

        [JsonPropertyName("two_way_link")]
        public bool TwoWayLink { get; set; }

        [JsonPropertyName("visibility")]
        public int Visibility { get; set; }
    }
}
