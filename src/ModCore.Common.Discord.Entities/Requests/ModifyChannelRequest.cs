using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Channels;

namespace ModCore.Common.Discord.Entities.Requests
{
    public record ModifyChannelRequest
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("icon")]
        public Optional<string> Icon { get; set; }

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

        [JsonPropertyName("archived")]
        public Optional<bool> Archived { get; set; }

        [JsonPropertyName("auto_archive_duration")]
        public Optional<int> AutoArchiveDuration { get; set; }

        [JsonPropertyName("locked")]
        public Optional<bool> Locked { get; set; }

        [JsonPropertyName("invitable")]
        public Optional<bool> Invitable { get; set; }

        [JsonPropertyName("applied_tags")]
        public Optional<Snowflake[]> AppliedTags { get; set; }
    }
}
