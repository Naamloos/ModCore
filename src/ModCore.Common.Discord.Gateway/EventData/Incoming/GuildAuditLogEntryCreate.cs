using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Gateway.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Gateway.EventData.Incoming
{
    public record GuildAuditLogEntryCreate : AuditLogEntry, IPublishable
    {
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; set; } = 0;
    }
}
