using System.Text.Json;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Entities.Requests;
using ModCore.Common.Discord.Entities.Webhooks;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<Webhook>> CreateWebhookAsync(
            Snowflake channelId,
            CreateWebhookRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Webhook>(
                HttpMethod.Post,
                $"channels/{EscapeRouteValue(channelId)}/webhooks",
                $"channels/{EscapeRouteValue(channelId)}/webhooks",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Webhook[]>> GetChannelWebhooksAsync(
            Snowflake channelId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Webhook[]>(
                HttpMethod.Get,
                $"channels/{EscapeRouteValue(channelId)}/webhooks",
                $"channels/{EscapeRouteValue(channelId)}/webhooks",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Webhook[]>> GetGuildWebhooksAsync(
            Snowflake guildId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Webhook[]>(
                HttpMethod.Get,
                $"guilds/{EscapeRouteValue(guildId)}/webhooks",
                $"guilds/{EscapeRouteValue(guildId)}/webhooks",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Webhook>> GetWebhookAsync(
            Snowflake webhookId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Webhook>(
                HttpMethod.Get,
                $"webhooks/{EscapeRouteValue(webhookId)}",
                $"webhooks/{EscapeRouteValue(webhookId)}",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Webhook>> GetWebhookWithTokenAsync(
            Snowflake webhookId,
            string webhookToken,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Webhook>(
                HttpMethod.Get,
                $"webhooks/{EscapeRouteValue(webhookId)}/{EscapeRouteValue(webhookToken)}",
                $"webhooks/{EscapeRouteValue(webhookId)}/{EscapeRouteValue(webhookToken)}",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Webhook>> ModifyWebhookAsync(
            Snowflake webhookId,
            ModifyWebhookRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Webhook>(
                HttpMethod.Patch,
                $"webhooks/{EscapeRouteValue(webhookId)}",
                $"webhooks/{EscapeRouteValue(webhookId)}",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Webhook>> ModifyWebhookWithTokenAsync(
            Snowflake webhookId,
            string webhookToken,
            ModifyWebhookRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Webhook>(
                HttpMethod.Patch,
                $"webhooks/{EscapeRouteValue(webhookId)}/{EscapeRouteValue(webhookToken)}",
                $"webhooks/{EscapeRouteValue(webhookId)}/{EscapeRouteValue(webhookToken)}",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteWebhookAsync(
            Snowflake webhookId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"webhooks/{EscapeRouteValue(webhookId)}",
                $"webhooks/{EscapeRouteValue(webhookId)}",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteWebhookWithTokenAsync(
            Snowflake webhookId,
            string webhookToken,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"webhooks/{EscapeRouteValue(webhookId)}/{EscapeRouteValue(webhookToken)}",
                $"webhooks/{EscapeRouteValue(webhookId)}/{EscapeRouteValue(webhookToken)}",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Message>> ExecuteWebhookAsync(
            Snowflake webhookId,
            string webhookToken,
            ExecuteWebhookRequest body,
            ExecuteWebhookQuery? query = null,
            IReadOnlyList<UploadFile>? files = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Message>(
                HttpMethod.Post,
                $"webhooks/{EscapeRouteValue(webhookId)}/{EscapeRouteValue(webhookToken)}",
                $"webhooks/{EscapeRouteValue(webhookId)}/{EscapeRouteValue(webhookToken)}",
                files == null ? body : new MultipartRequest { Payload = body, Files = files },
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> ExecuteSlackCompatibleWebhookAsync(
            Snowflake webhookId,
            string webhookToken,
            JsonElement body,
            ExecuteSlackCompatibleWebhookQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Post,
                $"webhooks/{EscapeRouteValue(webhookId)}/{EscapeRouteValue(webhookToken)}/slack",
                $"webhooks/{EscapeRouteValue(webhookId)}/{EscapeRouteValue(webhookToken)}/slack",
                body,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> ExecuteGitHubCompatibleWebhookAsync(
            Snowflake webhookId,
            string webhookToken,
            JsonElement body,
            ExecuteGitHubCompatibleWebhookQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Post,
                $"webhooks/{EscapeRouteValue(webhookId)}/{EscapeRouteValue(webhookToken)}/github",
                $"webhooks/{EscapeRouteValue(webhookId)}/{EscapeRouteValue(webhookToken)}/github",
                body,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Message>> GetWebhookMessageAsync(
            Snowflake webhookId,
            string webhookToken,
            Snowflake messageId,
            GetWebhookMessageQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Message>(
                HttpMethod.Get,
                $"webhooks/{EscapeRouteValue(webhookId)}/{EscapeRouteValue(webhookToken)}/messages/{EscapeRouteValue(messageId)}",
                $"webhooks/{EscapeRouteValue(webhookId)}/{EscapeRouteValue(webhookToken)}/messages/:message_id",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Message>> EditWebhookMessageAsync(
            Snowflake webhookId,
            string webhookToken,
            Snowflake messageId,
            EditWebhookMessageRequest body,
            EditWebhookMessageQuery? query = null,
            IReadOnlyList<UploadFile>? files = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Message>(
                HttpMethod.Patch,
                $"webhooks/{EscapeRouteValue(webhookId)}/{EscapeRouteValue(webhookToken)}/messages/{EscapeRouteValue(messageId)}",
                $"webhooks/{EscapeRouteValue(webhookId)}/{EscapeRouteValue(webhookToken)}/messages/:message_id",
                files == null ? body : new MultipartRequest { Payload = body, Files = files },
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteWebhookMessageAsync(
            Snowflake webhookId,
            string webhookToken,
            Snowflake messageId,
            DeleteWebhookMessageQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"webhooks/{EscapeRouteValue(webhookId)}/{EscapeRouteValue(webhookToken)}/messages/{EscapeRouteValue(messageId)}",
                $"webhooks/{EscapeRouteValue(webhookId)}/{EscapeRouteValue(webhookToken)}/messages/:message_id",
                null,
                query,
                auditLogReason,
                cancellationToken
            );
    }
}
