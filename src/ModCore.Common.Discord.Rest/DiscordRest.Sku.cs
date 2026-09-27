using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Monetization;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<SKU[]>> ListSKUsAsync(
            Snowflake applicationId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<SKU[]>(
                HttpMethod.Get,
                $"applications/{EscapeRouteValue(applicationId)}/skus",
                $"applications/:application_id/skus",
                null,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
