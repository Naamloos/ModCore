using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Invites;
using ModCore.Common.Discord.Entities.Requests;
using ModCore.Common.Discord.Entities.Responses;
using ModCore.Common.Discord.Entities.Voice;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<Guild>> GetGuildWithOptionsAsync(
            Snowflake guildId,
            GetGuildQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Guild>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}",
                $"guilds/{EscapeRouteValue(guildId)}",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildPreview>> GetGuildPreviewAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildPreview>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/preview",
                $"guilds/{EscapeRouteValue(guildId)}/preview",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Guild>> ModifyGuildAsync(
            Snowflake guildId,
            ModifyGuildRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Guild>(
                HttpMethod.Patch,
                $"guilds/{EscapeRouteValue(guildId)}",
                $"guilds/{EscapeRouteValue(guildId)}",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Channel[]>> GetGuildChannelsWithOptionsAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Channel[]>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/channels",
                $"guilds/{EscapeRouteValue(guildId)}/channels",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Channel>> CreateGuildChannelAsync(
            Snowflake guildId,
            CreateGuildChannelRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Channel>(
                HttpMethod.Post,
                $"guilds/{EscapeRouteValue(guildId)}/channels",
                $"guilds/{EscapeRouteValue(guildId)}/channels",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> ModifyGuildChannelPositionsAsync(
            Snowflake guildId,
            ModifyGuildChannelPositionsRequest[] body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Patch,
                $"guilds/{EscapeRouteValue(guildId)}/channels",
                $"guilds/{EscapeRouteValue(guildId)}/channels",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<ActiveThreads>> ListActiveGuildThreadsAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<ActiveThreads>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/threads/active",
                $"guilds/{EscapeRouteValue(guildId)}/threads/active",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Member>> GetGuildMemberWithOptionsAsync(
            Snowflake guildId,
            Snowflake userId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Member>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/members/{EscapeRouteValue(userId)}",
                $"guilds/{EscapeRouteValue(guildId)}/members/:user_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Member[]>> ListGuildMembersAsync(
            Snowflake guildId,
            ListGuildMembersQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Member[]>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/members",
                $"guilds/{EscapeRouteValue(guildId)}/members",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Member[]>> SearchGuildMembersAsync(
            Snowflake guildId,
            SearchGuildMembersQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Member[]>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/members/search",
                $"guilds/{EscapeRouteValue(guildId)}/members/search",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Member>> AddGuildMemberAsync(
            Snowflake guildId,
            Snowflake userId,
            AddGuildMemberRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Member>(
                HttpMethod.Put,
                $"guilds/{EscapeRouteValue(guildId)}/members/{EscapeRouteValue(userId)}",
                $"guilds/{EscapeRouteValue(guildId)}/members/:user_id",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Member>> ModifyGuildMemberAsync(
            Snowflake guildId,
            Snowflake userId,
            ModifyGuildMemberRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Member>(
                HttpMethod.Patch,
                $"guilds/{EscapeRouteValue(guildId)}/members/{EscapeRouteValue(userId)}",
                $"guilds/{EscapeRouteValue(guildId)}/members/:user_id",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Member>> ModifyCurrentMemberAsync(
            Snowflake guildId,
            ModifyCurrentMemberRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Member>(
                HttpMethod.Patch,
                $"guilds/{EscapeRouteValue(guildId)}/members/@me",
                $"guilds/{EscapeRouteValue(guildId)}/members/@me",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildNickname>> ModifyCurrentUserNickAsync(
            Snowflake guildId,
            ModifyCurrentUserNickRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildNickname>(
                HttpMethod.Patch,
                $"guilds/{EscapeRouteValue(guildId)}/members/@me/nick",
                $"guilds/{EscapeRouteValue(guildId)}/members/@me/nick",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> AddGuildMemberRoleAsync(
            Snowflake guildId,
            Snowflake userId,
            Snowflake roleId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Put,
                $"guilds/{EscapeRouteValue(guildId)}/members/{EscapeRouteValue(userId)}/roles/{EscapeRouteValue(roleId)}",
                $"guilds/{EscapeRouteValue(guildId)}/members/:user_id/roles/:role_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> RemoveGuildMemberRoleAsync(
            Snowflake guildId,
            Snowflake userId,
            Snowflake roleId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"guilds/{EscapeRouteValue(guildId)}/members/{EscapeRouteValue(userId)}/roles/{EscapeRouteValue(roleId)}",
                $"guilds/{EscapeRouteValue(guildId)}/members/:user_id/roles/:role_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> RemoveGuildMemberAsync(
            Snowflake guildId,
            Snowflake userId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"guilds/{EscapeRouteValue(guildId)}/members/{EscapeRouteValue(userId)}",
                $"guilds/{EscapeRouteValue(guildId)}/members/:user_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Ban[]>> GetGuildBansAsync(
            Snowflake guildId,
            GetGuildBansQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Ban[]>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/bans",
                $"guilds/{EscapeRouteValue(guildId)}/bans",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Ban>> GetGuildBanAsync(
            Snowflake guildId,
            Snowflake userId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Ban>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/bans/{EscapeRouteValue(userId)}",
                $"guilds/{EscapeRouteValue(guildId)}/bans/:user_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> CreateGuildBanWithOptionsAsync(
            Snowflake guildId,
            Snowflake userId,
            CreateGuildBanRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Put,
                $"guilds/{EscapeRouteValue(guildId)}/bans/{EscapeRouteValue(userId)}",
                $"guilds/{EscapeRouteValue(guildId)}/bans/:user_id",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> RemoveGuildBanAsync(
            Snowflake guildId,
            Snowflake userId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"guilds/{EscapeRouteValue(guildId)}/bans/{EscapeRouteValue(userId)}",
                $"guilds/{EscapeRouteValue(guildId)}/bans/:user_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<BulkBanResponse>> BulkGuildBanAsync(
            Snowflake guildId,
            BulkGuildBanRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<BulkBanResponse>(
                HttpMethod.Post,
                $"guilds/{EscapeRouteValue(guildId)}/bulk-ban",
                $"guilds/{EscapeRouteValue(guildId)}/bulk-ban",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Role[]>> GetGuildRolesWithOptionsAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Role[]>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/roles",
                $"guilds/{EscapeRouteValue(guildId)}/roles",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Role>> GetGuildRoleAsync(
            Snowflake guildId,
            Snowflake roleId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Role>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/roles/{EscapeRouteValue(roleId)}",
                $"guilds/{EscapeRouteValue(guildId)}/roles/:role_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Dictionary<string, int>>> GetGuildRoleMemberCountsAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Dictionary<string, int>>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/roles/member-counts",
                $"guilds/{EscapeRouteValue(guildId)}/roles/member-counts",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Role>> CreateGuildRoleAsync(
            Snowflake guildId,
            CreateGuildRoleRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Role>(
                HttpMethod.Post,
                $"guilds/{EscapeRouteValue(guildId)}/roles",
                $"guilds/{EscapeRouteValue(guildId)}/roles",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Role[]>> ModifyGuildRolePositionsAsync(
            Snowflake guildId,
            ModifyGuildRolePositionsRequest[] body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Role[]>(
                HttpMethod.Patch,
                $"guilds/{EscapeRouteValue(guildId)}/roles",
                $"guilds/{EscapeRouteValue(guildId)}/roles",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Role>> ModifyGuildRoleAsync(
            Snowflake guildId,
            Snowflake roleId,
            ModifyGuildRoleRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Role>(
                HttpMethod.Patch,
                $"guilds/{EscapeRouteValue(guildId)}/roles/{EscapeRouteValue(roleId)}",
                $"guilds/{EscapeRouteValue(guildId)}/roles/:role_id",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteGuildRoleAsync(
            Snowflake guildId,
            Snowflake roleId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"guilds/{EscapeRouteValue(guildId)}/roles/{EscapeRouteValue(roleId)}",
                $"guilds/{EscapeRouteValue(guildId)}/roles/:role_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildPruneResult>> GetGuildPruneCountAsync(
            Snowflake guildId,
            GetGuildPruneCountQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildPruneResult>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/prune",
                $"guilds/{EscapeRouteValue(guildId)}/prune",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildPruneResult>> BeginGuildPruneAsync(
            Snowflake guildId,
            BeginGuildPruneRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildPruneResult>(
                HttpMethod.Post,
                $"guilds/{EscapeRouteValue(guildId)}/prune",
                $"guilds/{EscapeRouteValue(guildId)}/prune",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<VoiceRegion[]>> GetGuildVoiceRegionsAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<VoiceRegion[]>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/regions",
                $"guilds/{EscapeRouteValue(guildId)}/regions",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Invite[]>> GetGuildInvitesAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Invite[]>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/invites",
                $"guilds/{EscapeRouteValue(guildId)}/invites",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Integration[]>> GetGuildIntegrationsAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Integration[]>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/integrations",
                $"guilds/{EscapeRouteValue(guildId)}/integrations",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteGuildIntegrationAsync(
            Snowflake guildId,
            Snowflake integrationId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"guilds/{EscapeRouteValue(guildId)}/integrations/{EscapeRouteValue(integrationId)}",
                $"guilds/{EscapeRouteValue(guildId)}/integrations/:integration_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildWidgetSettings>> GetGuildWidgetSettingsAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildWidgetSettings>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/widget",
                $"guilds/{EscapeRouteValue(guildId)}/widget",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildWidgetSettings>> ModifyGuildWidgetAsync(
            Snowflake guildId,
            GuildWidgetSettings body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildWidgetSettings>(
                HttpMethod.Patch,
                $"guilds/{EscapeRouteValue(guildId)}/widget",
                $"guilds/{EscapeRouteValue(guildId)}/widget",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildWidget>> GetGuildWidgetAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildWidget>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/widget.json",
                $"guilds/{EscapeRouteValue(guildId)}/widget.json",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildVanityUrl>> GetGuildVanityURLAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildVanityUrl>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/vanity-url",
                $"guilds/{EscapeRouteValue(guildId)}/vanity-url",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<byte[]>> GetGuildWidgetImageAsync(
            Snowflake guildId,
            GetGuildWidgetImageQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<byte[]>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/widget.png",
                $"guilds/{EscapeRouteValue(guildId)}/widget.png",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<WelcomeScreen>> GetGuildWelcomeScreenAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<WelcomeScreen>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/welcome-screen",
                $"guilds/{EscapeRouteValue(guildId)}/welcome-screen",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<WelcomeScreen>> ModifyGuildWelcomeScreenAsync(
            Snowflake guildId,
            ModifyGuildWelcomeScreenRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<WelcomeScreen>(
                HttpMethod.Patch,
                $"guilds/{EscapeRouteValue(guildId)}/welcome-screen",
                $"guilds/{EscapeRouteValue(guildId)}/welcome-screen",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildOnboarding>> GetGuildOnboardingAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildOnboarding>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/onboarding",
                $"guilds/{EscapeRouteValue(guildId)}/onboarding",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GuildOnboarding>> ModifyGuildOnboardingAsync(
            Snowflake guildId,
            ModifyGuildOnboardingRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GuildOnboarding>(
                HttpMethod.Put,
                $"guilds/{EscapeRouteValue(guildId)}/onboarding",
                $"guilds/{EscapeRouteValue(guildId)}/onboarding",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<IncidentsData>> ModifyGuildIncidentActionsAsync(
            Snowflake guildId,
            ModifyGuildIncidentActionsRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<IncidentsData>(
                HttpMethod.Put,
                $"guilds/{EscapeRouteValue(guildId)}/incident-actions",
                $"guilds/{EscapeRouteValue(guildId)}/incident-actions",
                body,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
