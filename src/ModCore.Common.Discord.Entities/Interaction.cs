using ModCore.Common.Discord.Rest.Entities;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities
{
    public class Interaction
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("application_id")]
        public Snowflake ApplicationId { get; set; }

        [JsonPropertyName("type")]
        public InteractionType Type { get; set; }

        [JsonPropertyName("data")]
        public Optional<InteractionData> Data { get; set; } // TODO implement mechanic for parsing this

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("channel")]
        public Optional<Channel> Channel { get; set; }

        [JsonPropertyName("channel_id")]
        public Optional<Snowflake> ChannelId { get; set; }

        [JsonPropertyName("member")]
        public Optional<Member> Member { get; set; }

        [JsonPropertyName("user")]
        public Optional<User> User { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("version")]
        public int Version { get; set; } // Always 1, why does Discord even send this

        [JsonPropertyName("message")]
        public Optional<Message> Message { get; set; }

        [JsonPropertyName("app_permissions")]
        public Optional<string> AppPermissions { get; set; }

        [JsonPropertyName("locale")]
        public Optional<string> Locale { get; set; }

        [JsonPropertyName("guild_locale")]
        public Optional<string> GuildLocale { get; set; }

        [JsonPropertyName("entitlements")]
        public Entitlement[] Entitlements { get; set; }
    }

    public enum InteractionType
    {
        Ping = 1,
        ApplicationCommand = 2,
        MessageComponent = 3,
        ApplicationCommandAutocomplete = 4,
        ModalSubmit = 5
    }
}
