using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Entities.Components;
using ModCore.Common.Discord.Entities.Messages;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record ModifyChannelJSONGroupDMRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("icon")]
        public Optional<string> Icon { get; set; }
    }

    public record ModifyChannelJSONGuildChannelRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("type")]
        public Optional<int> Type { get; set; }

        [JsonPropertyName("position")]
        public Optional<int?> Position { get; set; }

        [JsonPropertyName("topic")]
        public Optional<string?> Topic { get; set; }

        [JsonPropertyName("nsfw")]
        public Optional<bool?> Nsfw { get; set; }

        [JsonPropertyName("rate_limit_per_user")]
        public Optional<int?> RateLimitPerUser { get; set; }

        [JsonPropertyName("bitrate")]
        public Optional<int?> Bitrate { get; set; }

        [JsonPropertyName("user_limit")]
        public Optional<int?> UserLimit { get; set; }

        [JsonPropertyName("permission_overwrites")]
        public Optional<Overwrite[]?> PermissionOverwrites { get; set; }

        [JsonPropertyName("parent_id")]
        public Optional<Snowflake?> ParentId { get; set; }

        [JsonPropertyName("rtc_region")]
        public Optional<string?> RtcRegion { get; set; }

        [JsonPropertyName("video_quality_mode")]
        public Optional<int?> VideoQualityMode { get; set; }

        [JsonPropertyName("default_auto_archive_duration")]
        public Optional<int?> DefaultAutoArchiveDuration { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long> Flags { get; set; }

        [JsonPropertyName("available_tags")]
        public Optional<Tag[]> AvailableTags { get; set; }

        [JsonPropertyName("default_reaction_emoji")]
        public Optional<DefaultReaction?> DefaultReactionEmoji { get; set; }

        [JsonPropertyName("default_thread_rate_limit_per_user")]
        public Optional<int> DefaultThreadRateLimitPerUser { get; set; }

        [JsonPropertyName("default_sort_order")]
        public Optional<int?> DefaultSortOrder { get; set; }

        [JsonPropertyName("default_forum_layout")]
        public Optional<int> DefaultForumLayout { get; set; }
    }

    public record ModifyChannelJSONThreadRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("archived")]
        public Optional<bool> Archived { get; set; }

        [JsonPropertyName("auto_archive_duration")]
        public Optional<int> AutoArchiveDuration { get; set; }

        [JsonPropertyName("locked")]
        public Optional<bool> Locked { get; set; }

        [JsonPropertyName("invitable")]
        public Optional<bool> Invitable { get; set; }

        [JsonPropertyName("rate_limit_per_user")]
        public Optional<int?> RateLimitPerUser { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long> Flags { get; set; }

        [JsonPropertyName("applied_tags")]
        public Optional<Snowflake[]> AppliedTags { get; set; }
    }

    public record SetVoiceChannelStatusRequest
    {
        [JsonPropertyName("status")]
        public Optional<string?> Status { get; set; }
    }

    public record EditChannelPermissionsRequest
    {
        [JsonPropertyName("allow")]
        public Optional<string> Allow { get; set; }

        [JsonPropertyName("deny")]
        public Optional<string> Deny { get; set; }

        [JsonPropertyName("type")]
        public Optional<int> Type { get; set; }
    }

    public record CreateChannelInviteRequest
    {
        [JsonPropertyName("max_age")]
        public Optional<int> MaxAge { get; set; }

        [JsonPropertyName("max_uses")]
        public Optional<int> MaxUses { get; set; }

        [JsonPropertyName("temporary")]
        public Optional<bool> Temporary { get; set; }

        [JsonPropertyName("unique")]
        public Optional<bool> Unique { get; set; }

        [JsonPropertyName("target_type")]
        public Optional<int> TargetType { get; set; }

        [JsonPropertyName("target_user_id")]
        public Optional<Snowflake> TargetUserId { get; set; }

        [JsonPropertyName("target_application_id")]
        public Optional<Snowflake> TargetApplicationId { get; set; }

        [JsonPropertyName("role_ids")]
        public Optional<Snowflake[]> RoleIds { get; set; }
    }

    public record FollowAnnouncementChannelRequest
    {
        [JsonPropertyName("webhook_channel_id")]
        public Optional<Snowflake> WebhookChannelId { get; set; }
    }

    public record GroupDMAddRecipientRequest
    {
        [JsonPropertyName("access_token")]
        public Optional<string> AccessToken { get; set; }

        [JsonPropertyName("nick")]
        public Optional<string> Nick { get; set; }
    }

    public record StartThreadFromMessageRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("auto_archive_duration")]
        public Optional<int> AutoArchiveDuration { get; set; }

        [JsonPropertyName("rate_limit_per_user")]
        public Optional<int?> RateLimitPerUser { get; set; }
    }

    public record StartThreadWithoutMessageRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("auto_archive_duration")]
        public Optional<int> AutoArchiveDuration { get; set; }

        [JsonPropertyName("type")]
        public Optional<int> Type { get; set; }

        [JsonPropertyName("invitable")]
        public Optional<bool> Invitable { get; set; }

        [JsonPropertyName("rate_limit_per_user")]
        public Optional<int?> RateLimitPerUser { get; set; }
    }

    public record StartThreadInForumOrMediaChannelRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("auto_archive_duration")]
        public Optional<int> AutoArchiveDuration { get; set; }

        [JsonPropertyName("rate_limit_per_user")]
        public Optional<int?> RateLimitPerUser { get; set; }

        [JsonPropertyName("message")]
        public Optional<StartThreadInForumOrMediaChannelForumAndMediaThreadMessageObjectRequest> Message { get; set; }

        [JsonPropertyName("applied_tags")]
        public Optional<Snowflake[]> AppliedTags { get; set; }
    }

    public record StartThreadInForumOrMediaChannelForumAndMediaThreadMessageObjectRequest
    {
        [JsonPropertyName("content")]
        public Optional<string> Content { get; set; }

        [JsonPropertyName("embeds")]
        public Optional<Embed[]> Embeds { get; set; }

        [JsonPropertyName("allowed_mentions")]
        public Optional<AllowedMention> AllowedMentions { get; set; }

        [JsonPropertyName("components")]
        public Optional<Component[]> Components { get; set; }

        [JsonPropertyName("sticker_ids")]
        public Optional<Snowflake[]> StickerIds { get; set; }

        [JsonPropertyName("attachments")]
        public Optional<AttachmentRequest[]> Attachments { get; set; }

        [JsonPropertyName("flags")]
        public Optional<long> Flags { get; set; }
    }

    public record GetThreadMemberQuery
    {
        [JsonPropertyName("with_member")]
        public Optional<bool> WithMember { get; set; }
    }

    public record ListThreadMembersQuery
    {
        [JsonPropertyName("with_member")]
        public Optional<bool> WithMember { get; set; }

        [JsonPropertyName("after")]
        public Optional<Snowflake> After { get; set; }

        [JsonPropertyName("limit")]
        public Optional<int> Limit { get; set; }
    }

    public record ListPublicArchivedThreadsQuery
    {
        [JsonPropertyName("before")]
        public Optional<DateTimeOffset> Before { get; set; }

        [JsonPropertyName("limit")]
        public Optional<int> Limit { get; set; }
    }

    public record ListPrivateArchivedThreadsQuery
    {
        [JsonPropertyName("before")]
        public Optional<DateTimeOffset> Before { get; set; }

        [JsonPropertyName("limit")]
        public Optional<int> Limit { get; set; }
    }

    public record ListJoinedPrivateArchivedThreadsQuery
    {
        [JsonPropertyName("before")]
        public Optional<Snowflake> Before { get; set; }

        [JsonPropertyName("limit")]
        public Optional<int> Limit { get; set; }
    }
}
