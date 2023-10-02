using ModCore.Common.Discord.Rest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities
{
    public class Channel
    {
        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }

        [JsonPropertyName("type")]
        public ChannelType Type { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; set; }

        [JsonPropertyName("position")]
        public Optional<int> Position { get; set; }

        public Optional<List<Overwrite>> PermissionOverwrites { get; set; }

        public Optional<string?> Name { get; set; }

        public Optional<string?> Topic { get; set; }

        public Optional<bool> NotSafeForWork { get; set; }

        public Optional<Snowflake?> LastMessageId { get; set; }

        public Optional<int> Bitrate { get; set; }

        public Optional<int> UserLimit { get; set; }

        public Optional<int> RateLimitPerUser { get; set; }

        public Optional<List<User>> Recipients { get; set; }

        public Optional<string?> Icon { get; set; }

        public Optional<Snowflake> OwnerId { get; set; }

        public Optional<Snowflake> ApplicationId { get; set; }

        public Optional<bool> Managed { get; set; }

        public Optional<Snowflake?> ParentId { get; set; }

        public Optional<DateTimeOffset?> LastPinTimestamp { get; set; }

        public Optional<string?> RtcRegion { get; set; }

        public Optional<VideoQualityMode> VideoQualityMode { get; set; }

        public Optional<int> MessageCount { get; set; }

        public Optional<int> MemberCount { get; set; }

        public Optional<ThreadMetadata> ThreadMetadata { get; set; }

        public Optional<ThreadMember> Member { get; set; }

        public Optional<int> DefaultAutoArchiveDuration { get; set; }

        public Optional<string?> Permissions { get; set; }

        public Optional<int> Flags { get; set; }

        public Optional<int> TotalMessageSent { get; set; }

        public Optional<List<Tag>> AvailableTags { get; set; }

        public Optional<List<Snowflake>> AppliedTags { get; set; }

        public Optional<DefaultReaction?> DefaultReactionEmoji { get; set; }

        public Optional<int> DefaultThreadRateLimitPerUser { get; set; }

        public Optional<int?> DefaultSortOrder { get; set; }

        public Optional<int> DefaultForumLayout { get; set; }
    }

    public class DefaultReaction
    {
    }

    public class Tag
    {
    }

    public class ThreadMetadata
    {
    }

    public class ThreadMember
    {
    }

    public class VideoQualityMode
    {
    }

    public enum ChannelType
    {
        GuildText = 0,
        DirectMessage = 1,
        GuildVoice = 2,
        GroupDirectMessage = 3,
        GuildCategory = 4,
        GuildAnnouncement = 5,
        AnnouncementThread = 10,
        PublicThread = 11,
        PrivateThread = 12,
        GuildStageVoice = 13,
        GuildDirectory = 14,
        GuildForum = 15,
        GuildMedia = 16
    }
}
