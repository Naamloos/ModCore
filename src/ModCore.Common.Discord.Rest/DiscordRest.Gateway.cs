using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Gateway;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<GatewayAddress>> GetGatewayAsync(
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GatewayAddress>(
                HttpMethod.Get,
                $"gateway",
                $"gateway",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<GatewayBot>> GetGatewayBotAsync(
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<GatewayBot>(
                HttpMethod.Get,
                $"gateway/bot",
                $"gateway/bot",
                null,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
