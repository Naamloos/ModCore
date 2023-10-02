using ModCore.Common.Discord.Rest.Entities;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities
{
    public class Message
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; set; }

        [JsonPropertyName("author")]
        public User Author { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonPropertyName("edited_timestamp")]
        public DateTimeOffset? EditedTimestamp { get; set; }

        [JsonPropertyName("tts")]
        public bool IsTextToSpeech { get; set; }

        [JsonPropertyName("mention_everyone")]
        public bool MentionsEveryone { get; set; }

        [JsonPropertyName("mentions")]
        public User[] Mentions { get; set; }

        [JsonPropertyName("mention_roles")]
        public Snowflake[] MentionsRoles { get; set; }

        [JsonPropertyName("mention_channels")]
        public JsonObject[] MentionChannels { get; set; }

        [JsonPropertyName("attachments")]
        public JsonObject[] Attachments { get; set; }

        [JsonPropertyName("embeds")]
        public JsonObject[] Embeds { get; set; }

        [JsonPropertyName("reactions")]
        public JsonObject[] Reactions { get; set; }

        [JsonPropertyName("nonce")] // can be a string or int, I hope that doesn't break this
        public string Nonce { get; set; } // no ur a nonce

        [JsonPropertyName("pinned")]
        public bool Pinned { get; set; }

        [JsonPropertyName("webhook_id")]
        public Snowflake? WebhookId { get; set; }

        [JsonPropertyName("type")]
        public int MessageType { get; set; }

        [JsonPropertyName("activity")]
        public JsonObject? Activity { get; set; }

        [JsonPropertyName("application")]
        public JsonObject? Application { get; set; }

        [JsonPropertyName("application_id")]
        public Snowflake? ApplicationId { get; set; }

        [JsonPropertyName("message_reference")]
        public JsonObject? MessageReference { get; set; }

        [JsonPropertyName("flags")]
        public int? Flags { get; set; }

        [JsonPropertyName("referenced_message")]
        public Message? ReferencedMessage { get; set; }

        [JsonPropertyName("interaction")]
        public JsonObject? Interaction { get; set; }

        [JsonPropertyName("thread")]
        public JsonObject? Thread { get; set; }

        [JsonPropertyName("components")]
        public JsonObject[] Components { get; set; }

        [JsonPropertyName("sticker_items")]
        public JsonObject[] StickerItems { get; set; }

        [JsonPropertyName("stickers")]
        public JsonObject[] Stickers { get; set; }

        [JsonPropertyName("position")]
        public int? Position { get; set; }

        [JsonPropertyName("role_subscription_data")]
        public JsonObject? RoleSubscriptionData { get; set; }
    }
}
