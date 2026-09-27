using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Voice;

namespace ModCore.Common.Discord.Entities
{
    public record GatewayAddress
    {
        public string Url { get; set; } = "";
    }

    public record UnavailableGuild
    {
        public Snowflake Id { get; set; }
        public Optional<bool> Unavailable { get; set; }
    }

    public record ActiveThreads
    {
        public Channel[] Threads { get; set; } = [];
        public ThreadMember[] Members { get; set; } = [];
    }

    public record ArchivedThreads : ActiveThreads
    {
        public bool HasMore { get; set; }
    }

    public record ApplicationEmojis
    {
        public Emoji[] Items { get; set; } = [];
    }

    public record GuildSoundboardSounds
    {
        public SoundboardSound[] Items { get; set; } = [];
    }

    public record StickerPacks
    {
        [JsonPropertyName("sticker_packs")]
        public StickerPack[] Packs { get; set; } = [];
    }

    public record TargetUsersJobStatus
    {
        public int Status { get; set; }
        public int TotalUsers { get; set; }
        public int ProcessedUsers { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public record GuildVanityUrl
    {
        public string? Code { get; set; }
        public int Uses { get; set; }
    }

    public record GuildPruneResult
    {
        public int? Pruned { get; set; }
    }

    public record GuildNickname
    {
        public string? Nick { get; set; }
    }

    public record PollVoters
    {
        public User[] Users { get; set; } = [];
    }

    public record ApplicationIdentities
    {
        public ApplicationIdentity[] Identities { get; set; } = [];
    }
}
