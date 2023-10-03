using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Rest.Entities;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities
{
    public record Message
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
        public Optional<ChannelMention[]> MentionChannels { get; set; }

        [JsonPropertyName("attachments")]
        public Attachment[] Attachments { get; set; }

        [JsonPropertyName("embeds")]
        public Embed[] Embeds { get; set; }

        [JsonPropertyName("reactions")]
        public Optional<Reaction[]> Reactions { get; set; }

        [JsonPropertyName("nonce")] // can be a string or int, I hope that doesn't break this
        public Optional<string> Nonce { get; set; } // no ur a nonce

        [JsonPropertyName("pinned")]
        public bool Pinned { get; set; }

        [JsonPropertyName("webhook_id")]
        public Optional<Snowflake> WebhookId { get; set; }

        [JsonPropertyName("type")]
        public MessageType MessageType { get; set; }

        [JsonPropertyName("activity")]
        public Optional<MessageActivity> Activity { get; set; }

        [JsonPropertyName("application")]
        public Optional<Application> Application { get; set; }

        [JsonPropertyName("application_id")]
        public Optional<Snowflake> ApplicationId { get; set; }

        [JsonPropertyName("message_reference")]
        public Optional<MessageReference> MessageReference { get; set; }

        [JsonPropertyName("flags")]
        public Optional<MessageFlags> Flags { get; set; }

        [JsonPropertyName("referenced_message")]
        public Optional<Message?> ReferencedMessage { get; set; }

        [JsonPropertyName("interaction")]
        public Optional<MessageInteraction> Interaction { get; set; }

        [JsonPropertyName("thread")]
        public Optional<Channel> Thread { get; set; }

        [JsonPropertyName("components")]
        public Optional<MessageComponent[]> Components { get; set; }

        [JsonPropertyName("sticker_items")]
        public Optional<MessageStickerItem[]> StickerItems { get; set; }

        [JsonPropertyName("stickers")]
        public Optional<Sticker[]> Stickers { get; set; }

        [JsonPropertyName("position")]
        public Optional<int> Position { get; set; }

        [JsonPropertyName("role_subscription_data")]
        public Optional<RoleSubscriptionData> RoleSubscriptionData { get; set; }

        [JsonPropertyName("resolved")]
        public Optional<ResolvedDataStructure> Resolved { get; set; }

        private const string JUMP_LINK_FORMAT = "https://discord.com/channels/{0}/{1}/{2}";
        public string GetJumpLink(Snowflake GuildId) => string.Format(JUMP_LINK_FORMAT, GuildId, ChannelId, Id);
    }
}
