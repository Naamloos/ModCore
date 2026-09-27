using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Requests;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<AutoModerationRule[]>> ListAutoModerationRulesForGuildAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<AutoModerationRule[]>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/auto-moderation/rules",
                $"guilds/{EscapeRouteValue(guildId)}/auto-moderation/rules",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<AutoModerationRule>> GetAutoModerationRuleAsync(
            Snowflake guildId,
            Snowflake autoModerationRuleId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<AutoModerationRule>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/auto-moderation/rules/{EscapeRouteValue(autoModerationRuleId)}",
                $"guilds/{EscapeRouteValue(guildId)}/auto-moderation/rules/:auto_moderation_rule_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<AutoModerationRule>> CreateAutoModerationRuleAsync(
            Snowflake guildId,
            CreateAutoModerationRuleRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<AutoModerationRule>(
                HttpMethod.Post,
                $"guilds/{EscapeRouteValue(guildId)}/auto-moderation/rules",
                $"guilds/{EscapeRouteValue(guildId)}/auto-moderation/rules",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<AutoModerationRule>> ModifyAutoModerationRuleAsync(
            Snowflake guildId,
            Snowflake autoModerationRuleId,
            ModifyAutoModerationRuleRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<AutoModerationRule>(
                HttpMethod.Patch,
                $"guilds/{EscapeRouteValue(guildId)}/auto-moderation/rules/{EscapeRouteValue(autoModerationRuleId)}",
                $"guilds/{EscapeRouteValue(guildId)}/auto-moderation/rules/:auto_moderation_rule_id",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteAutoModerationRuleAsync(
            Snowflake guildId,
            Snowflake autoModerationRuleId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"guilds/{EscapeRouteValue(guildId)}/auto-moderation/rules/{EscapeRouteValue(autoModerationRuleId)}",
                $"guilds/{EscapeRouteValue(guildId)}/auto-moderation/rules/:auto_moderation_rule_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
