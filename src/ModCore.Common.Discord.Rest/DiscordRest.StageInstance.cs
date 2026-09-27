using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Requests;
using ModCore.Common.Discord.Entities.Voice;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<StageInstance>> CreateStageInstanceAsync(
            CreateStageInstanceRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<StageInstance>(
                HttpMethod.Post,
                $"stage-instances",
                $"stage-instances",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<StageInstance>> GetStageInstanceAsync(
            Snowflake channelId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<StageInstance>(
                HttpMethod.Get,
                $"stage-instances/{EscapeRouteValue(channelId)}",
                $"stage-instances/{EscapeRouteValue(channelId)}",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<StageInstance>> ModifyStageInstanceAsync(
            Snowflake channelId,
            ModifyStageInstanceRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<StageInstance>(
                HttpMethod.Patch,
                $"stage-instances/{EscapeRouteValue(channelId)}",
                $"stage-instances/{EscapeRouteValue(channelId)}",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteStageInstanceAsync(
            Snowflake channelId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"stage-instances/{EscapeRouteValue(channelId)}",
                $"stage-instances/{EscapeRouteValue(channelId)}",
                null,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
