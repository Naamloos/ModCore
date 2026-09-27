using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Components;
using ModCore.Common.Discord.Entities.Messages;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record CreateWebhookRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("avatar")]
        public Optional<string?> Avatar { get; set; }
    }

    public record ModifyWebhookRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("avatar")]
        public Optional<string?> Avatar { get; set; }

        [JsonPropertyName("channel_id")]
        public Optional<Snowflake> ChannelId { get; set; }
    }

    public record ExecuteWebhookQuery
    {
        [JsonPropertyName("wait")]
        public Optional<bool> Wait { get; set; }

        [JsonPropertyName("thread_id")]
        public Optional<Snowflake> ThreadId { get; set; }

        [JsonPropertyName("with_components")]
        public Optional<bool> WithComponents { get; set; }
    }

    public record ExecuteWebhookRequest
    {
        [JsonPropertyName("content")]
        public Optional<string> Content { get; set; }

        [JsonPropertyName("username")]
        public Optional<string> Username { get; set; }

        [JsonPropertyName("avatar_url")]
        public Optional<string> AvatarUrl { get; set; }

        [JsonPropertyName("tts")]
        public Optional<bool> Tts { get; set; }

        [JsonPropertyName("embeds")]
        public Optional<Embed[]> Embeds { get; set; }

        [JsonPropertyName("allowed_mentions")]
        public Optional<AllowedMention> AllowedMentions { get; set; }

        [JsonPropertyName("components")]
        public Optional<Component[]> Components { get; set; }

        [JsonPropertyName("attachments")]
        public Optional<AttachmentRequest[]> Attachments { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long> Flags { get; set; }

        [JsonPropertyName("thread_name")]
        public Optional<string> ThreadName { get; set; }

        [JsonPropertyName("applied_tags")]
        public Optional<Snowflake[]> AppliedTags { get; set; }

        [JsonPropertyName("poll")]
        public Optional<PollCreateRequest> Poll { get; set; }
    }

    public record ExecuteSlackCompatibleWebhookQuery
    {
        [JsonPropertyName("thread_id")]
        public Optional<Snowflake> ThreadId { get; set; }

        [JsonPropertyName("wait")]
        public Optional<bool> Wait { get; set; }
    }

    public record ExecuteGitHubCompatibleWebhookQuery
    {
        [JsonPropertyName("thread_id")]
        public Optional<Snowflake> ThreadId { get; set; }

        [JsonPropertyName("wait")]
        public Optional<bool> Wait { get; set; }
    }

    public record GetWebhookMessageQuery
    {
        [JsonPropertyName("thread_id")]
        public Optional<Snowflake> ThreadId { get; set; }
    }

    public record EditWebhookMessageQuery
    {
        [JsonPropertyName("thread_id")]
        public Optional<Snowflake> ThreadId { get; set; }

        [JsonPropertyName("with_components")]
        public Optional<bool> WithComponents { get; set; }
    }

    public record EditWebhookMessageRequest
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

        [JsonPropertyName("poll")]
        public Optional<PollCreateRequest> Poll { get; set; }
    }

    public record DeleteWebhookMessageQuery
    {
        [JsonPropertyName("thread_id")]
        public Optional<Snowflake> ThreadId { get; set; }
    }
}
