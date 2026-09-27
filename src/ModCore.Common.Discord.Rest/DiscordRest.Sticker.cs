using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Requests;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<Sticker>> GetStickerAsync(
            Snowflake stickerId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Sticker>(
                HttpMethod.Get,
                $"stickers/{EscapeRouteValue(stickerId)}",
                $"stickers/:sticker_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<StickerPacks>> ListStickerPacksAsync(
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<StickerPacks>(
                HttpMethod.Get,
                $"sticker-packs",
                $"sticker-packs",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<StickerPack>> GetStickerPackAsync(
            Snowflake packId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<StickerPack>(
                HttpMethod.Get,
                $"sticker-packs/{EscapeRouteValue(packId)}",
                $"sticker-packs/:pack_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Sticker[]>> ListGuildStickersAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Sticker[]>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/stickers",
                $"guilds/{EscapeRouteValue(guildId)}/stickers",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Sticker>> GetGuildStickerAsync(
            Snowflake guildId,
            Snowflake stickerId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Sticker>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/stickers/{EscapeRouteValue(stickerId)}",
                $"guilds/{EscapeRouteValue(guildId)}/stickers/:sticker_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Sticker>> CreateGuildStickerAsync(
            Snowflake guildId,
            MultipartRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Sticker>(
                HttpMethod.Post,
                $"guilds/{EscapeRouteValue(guildId)}/stickers",
                $"guilds/{EscapeRouteValue(guildId)}/stickers",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Sticker>> ModifyGuildStickerAsync(
            Snowflake guildId,
            Snowflake stickerId,
            ModifyGuildStickerRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Sticker>(
                HttpMethod.Patch,
                $"guilds/{EscapeRouteValue(guildId)}/stickers/{EscapeRouteValue(stickerId)}",
                $"guilds/{EscapeRouteValue(guildId)}/stickers/:sticker_id",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteGuildStickerAsync(
            Snowflake guildId,
            Snowflake stickerId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"guilds/{EscapeRouteValue(guildId)}/stickers/{EscapeRouteValue(stickerId)}",
                $"guilds/{EscapeRouteValue(guildId)}/stickers/:sticker_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
