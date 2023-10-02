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

        //[JsonPropertyName("sticker_items")]
        //public Optional<MessageStickerItem> StickerItems { get; set; }

        [JsonPropertyName("stickers")]
        public Optional<Sticker[]> Stickers { get; set; }

        [JsonPropertyName("position")]
        public Optional<int> Position { get; set; }

        [JsonPropertyName("role_subscription_data")]
        public Optional<RoleSubscriptionData> RoleSubscriptionData { get; set; }

        [JsonPropertyName("resolved")]
        public Optional<Resolved> Resolved { get; set; }
    }

    public record Resolved
    {
    }

    public record RoleSubscriptionData
    {
    }

    public record MessageStickerItem
    {
    }

    public record MessageComponent
    {
    }

    public record MessageInteraction
    {
    }

    [Flags]
    public enum MessageFlags
    {
        CrossPosted = 1<<0,
        IsCrosspost = 1<<1,
        SuppressEmbeds = 1<<2,
        SourceMessageDeleted = 1<<3,
        Urgent = 1<<4,
        HasThread = 1<<5,
        Ephemeral = 1<<6,
        Loading = 1<<7,
        FailedToMentionSomeRolesInThread = 1<<8,
        SuppressNotifications = 1<<12,
        IsVoiceMessage = 1<<13
    }

    public record MessageReference
    {
    }

    public record MessageActivity
    {
    }

    public enum MessageType
    {
        Default = 0,
        RecipientAdded = 1,
        RecipientRemoved = 2,
        Call = 3,
        ChannelNameChange = 4,
        ChannelIconChange = 5,
        ChannelPinnedMessage = 6,
        UserJoin = 7,
        GuildBoost = 8,
        GuildBoostTier1 = 9,
        GuildBoostTier2 = 10,
        GuildBoostTier3 = 11,
        ChannelFollowAdd = 12,
        GuildDiscoveryDisqualified = 14,
        GuildDiscoveryRequalified = 15,
        GuildDiscoveryGracePeriodInitialWarning = 16,
        GuildDiscoveryGracePeriodFinalWarning = 17,
        ThreadCreated = 18,
        Reply = 19,
        ChatInputCommand = 20,
        ThreadStarterMessage = 21,
        GuildInviteReminder = 22,
        ContextMenuCommand = 23,
        AutoModerationAction = 24,
        RoleSubscriptionPurchase = 25,
        InteractionPremiumUpsell = 26,
        StageStart = 27,
        StageEnd = 28,
        StageSpeaker = 29,
        StageTopic = 31,
        GuildApplicationPremiumSubscription = 32
    }

    public record Reaction
    {
    }

    public record Embed
    {
    }

    public record Attachment
    {
    }

    public record ChannelMention
    {
    }
}
