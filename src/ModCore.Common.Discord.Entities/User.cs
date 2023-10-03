using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Enums;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Rest.Entities
{
    public record User
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("discriminator")]
        public string Discriminator { get; set; }

        [JsonPropertyName("global_name")]
        public string? GlobalName { get; set; }

        [JsonPropertyName("avatar")]
        public string? AvatarHash { get; set; }

        [JsonPropertyName("bot")]
        public Optional<bool> IsBot { get; set; }

        [JsonPropertyName("system")]
        public Optional<bool> IsSystem { get; set; }

        [JsonPropertyName("mfa_enabled")]
        public Optional<bool> MfaEnabled { get; set; }

        [JsonPropertyName("banner")]
        public Optional<string?> BannerHash { get; set; }

        [JsonPropertyName("accent_color")]
        public Optional<int?> AccentColor { get; set; }

        [JsonPropertyName("locale")]
        public Optional<string> Locale { get; set; }

        [JsonPropertyName("verified")]
        public Optional<bool> Verified { get; set; }

        [JsonPropertyName("email")]
        public Optional<string?> Email { get; set; }

        [JsonPropertyName("flags")]
        public Optional<UserFlags> Flags { get; set; }

        [JsonPropertyName("premium_type")]
        public Optional<PremiumType> PremiumType { get; set; }

        [JsonPropertyName("public_flags")]
        public Optional<UserFlags> PublicFlags { get; set; }

        [JsonPropertyName("avatar_decoration")]
        public Optional<string?> AvatarDecorationHash { get; set; }

        private const string MENTION_FORMAT = "<@{0}>";
        public string Mention => string.Format(MENTION_FORMAT, Id);
    }
}
