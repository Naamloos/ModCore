using System.Text.Json;
using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Components;
using ModCore.Common.Discord.Entities.Messages;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record GetChannelMessagesQuery
    {
        [JsonPropertyName("around")]
        public Optional<Snowflake> Around { get; set; }

        [JsonPropertyName("before")]
        public Optional<Snowflake> Before { get; set; }

        [JsonPropertyName("after")]
        public Optional<Snowflake> After { get; set; }

        [JsonPropertyName("limit")]
        public Optional<int> Limit { get; set; }
    }

    public record SearchGuildMessagesQuery
    {
        [JsonPropertyName("limit")]
        public Optional<int> Limit { get; set; }

        [JsonPropertyName("offset")]
        public Optional<int> Offset { get; set; }

        [JsonPropertyName("max_id")]
        public Optional<Snowflake> MaxId { get; set; }

        [JsonPropertyName("min_id")]
        public Optional<Snowflake> MinId { get; set; }

        [JsonPropertyName("slop")]
        public Optional<int> Slop { get; set; }

        [JsonPropertyName("content")]
        public Optional<string> Content { get; set; }

        [JsonPropertyName("channel_id")]
        public Optional<Snowflake[]> ChannelId { get; set; }

        [JsonPropertyName("author_type")]
        public Optional<string[]> AuthorType { get; set; }

        [JsonPropertyName("author_id")]
        public Optional<Snowflake[]> AuthorId { get; set; }

        [JsonPropertyName("mentions")]
        public Optional<Snowflake[]> Mentions { get; set; }

        [JsonPropertyName("mentions_role_id")]
        public Optional<Snowflake[]> MentionsRoleId { get; set; }

        [JsonPropertyName("mention_everyone")]
        public Optional<bool> MentionEveryone { get; set; }

        [JsonPropertyName("replied_to_user_id")]
        public Optional<Snowflake[]> RepliedToUserId { get; set; }

        [JsonPropertyName("replied_to_message_id")]
        public Optional<Snowflake[]> RepliedToMessageId { get; set; }

        [JsonPropertyName("pinned")]
        public Optional<bool> Pinned { get; set; }

        [JsonPropertyName("has")]
        public Optional<string[]> Has { get; set; }

        [JsonPropertyName("embed_type")]
        public Optional<string[]> EmbedType { get; set; }

        [JsonPropertyName("embed_provider")]
        public Optional<string[]> EmbedProvider { get; set; }

        [JsonPropertyName("link_hostname")]
        public Optional<string[]> LinkHostname { get; set; }

        [JsonPropertyName("attachment_filename")]
        public Optional<string[]> AttachmentFilename { get; set; }

        [JsonPropertyName("attachment_extension")]
        public Optional<string[]> AttachmentExtension { get; set; }

        [JsonPropertyName("sort_by")]
        public Optional<string> SortBy { get; set; }

        [JsonPropertyName("sort_order")]
        public Optional<string> SortOrder { get; set; }

        [JsonPropertyName("include_nsfw")]
        public Optional<bool> IncludeNsfw { get; set; }
    }

    public record CreateMessageRequest
    {
        [JsonPropertyName("content")]
        public Optional<string> Content { get; set; }

        [JsonPropertyName("nonce")]
        public Optional<JsonElement> Nonce { get; set; }

        [JsonPropertyName("tts")]
        public Optional<bool> Tts { get; set; }

        [JsonPropertyName("embeds")]
        public Optional<Embed[]> Embeds { get; set; }

        [JsonPropertyName("allowed_mentions")]
        public Optional<AllowedMention> AllowedMentions { get; set; }

        [JsonPropertyName("message_reference")]
        public Optional<MessageReference> MessageReference { get; set; }

        [JsonPropertyName("components")]
        public Optional<Component[]> Components { get; set; }

        [JsonPropertyName("sticker_ids")]
        public Optional<Snowflake[]> StickerIds { get; set; }

        [JsonPropertyName("attachments")]
        public Optional<AttachmentRequest[]> Attachments { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long> Flags { get; set; }

        [JsonPropertyName("enforce_nonce")]
        public Optional<bool> EnforceNonce { get; set; }

        [JsonPropertyName("poll")]
        public Optional<PollCreateRequest> Poll { get; set; }

        [JsonPropertyName("shared_client_theme")]
        public Optional<SharedClientTheme> SharedClientTheme { get; set; }
    }

    public record GetReactionsQuery
    {
        [JsonPropertyName("type")]
        public Optional<int> Type { get; set; }

        [JsonPropertyName("after")]
        public Optional<Snowflake> After { get; set; }

        [JsonPropertyName("limit")]
        public Optional<int> Limit { get; set; }
    }

    public record EditMessageRequest
    {
        [JsonPropertyName("content")]
        public Optional<string> Content { get; set; }

        [JsonPropertyName("embeds")]
        public Optional<Embed[]> Embeds { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long> Flags { get; set; }

        [JsonPropertyName("allowed_mentions")]
        public Optional<AllowedMention> AllowedMentions { get; set; }

        [JsonPropertyName("components")]
        public Optional<Component[]> Components { get; set; }

        [JsonPropertyName("attachments")]
        public Optional<AttachmentRequest[]> Attachments { get; set; }
    }

    public record BulkDeleteMessagesRequest
    {
        [JsonPropertyName("messages")]
        public Optional<Snowflake[]> Messages { get; set; }
    }

    public record GetChannelPinsQuery
    {
        [JsonPropertyName("before")]
        public Optional<DateTimeOffset> Before { get; set; }

        [JsonPropertyName("limit")]
        public Optional<int> Limit { get; set; }
    }
}
