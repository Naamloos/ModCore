using System.Text.Json.Serialization;
using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Webhooks;

namespace ModCore.Common.Discord.Entities.Guilds
{
    public record AuditLog
    {
        [JsonPropertyName("application_commands")]
        public ApplicationCommand[] ApplicationCommands { get; set; } = default!;

        [JsonPropertyName("audit_log_entries")]
        public AuditLogEntry[] AuditLogEntries { get; set; } = default!;

        [JsonPropertyName("auto_moderation_rules")]
        public AutoModerationRule[] AutoModerationRules { get; set; } = default!;

        [JsonPropertyName("guild_scheduled_events")]
        public GuildScheduledEvent[] GuildScheduledEvents { get; set; } = default!;

        [JsonPropertyName("integrations")]
        public Integration[] Integrations { get; set; } = default!;

        [JsonPropertyName("threads")]
        public Channel[] Threads { get; set; } = default!;

        [JsonPropertyName("users")]
        public User[] Users { get; set; } = default!;

        [JsonPropertyName("webhooks")]
        public Webhook[] Webhooks { get; set; } = default!;
    }
}
