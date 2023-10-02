using ModCore.Common.Discord.Entities;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Rest.Entities
{
    public class User
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
    }

    public enum PremiumType
    {
        None = 0,
        NitroClassic = 1,
        Nitro = 2,
        NitroBasic = 3
    }

    [Flags]
    public enum UserFlags
    {
        Staff = 1<<0,
        Partner = 1<<1,
        Hypesquad = 1<<2,
        BugHunterLevel1 = 1<<3,
        HypesquadBravery = 1<<6,
        HypesquadBrilliance = 1<<7,
        HypesquadBalance = 1<<8,
        EarlySupporter = 1<<9,
        IsTeam = 1<<10,
        BugHunterLevel2 = 1<<14,
        VerifiedBot = 1<<16,
        VerifiedDeveloper = 1<<17,
        CertifiedModerator = 1<<18,
        BotOnlyUsesHttpInteractions = 1<<19,
        ActiveDeveloper = 1<<22
    }
}
