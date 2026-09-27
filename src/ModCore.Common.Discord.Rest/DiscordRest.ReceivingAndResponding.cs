using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Entities.Requests;
using ModCore.Common.Discord.Entities.Responses;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<
            RestResponse<InteractionCallbackResponse>
        > CreateInteractionResponseWithOptionsAsync(
            Snowflake interactionId,
            string interactionToken,
            InteractionResponse body,
            CreateInteractionResponseQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<InteractionCallbackResponse>(
                HttpMethod.Post,
                $"interactions/{EscapeRouteValue(interactionId)}/{EscapeRouteValue(interactionToken)}/callback",
                $"interactions/:interaction_id/:interaction_token/callback",
                body,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Message>> GetOriginalInteractionResponseAsync(
            Snowflake applicationId,
            string interactionToken,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Message>(
                HttpMethod.Get,
                $"webhooks/{EscapeRouteValue(applicationId)}/{EscapeRouteValue(interactionToken)}/messages/@original",
                $"webhooks/{EscapeRouteValue(applicationId)}/{EscapeRouteValue(interactionToken)}/messages/@original",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Message>> EditOriginalInteractionResponseWithOptionsAsync(
            Snowflake applicationId,
            string interactionToken,
            EditWebhookMessageRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Message>(
                HttpMethod.Patch,
                $"webhooks/{EscapeRouteValue(applicationId)}/{EscapeRouteValue(interactionToken)}/messages/@original",
                $"webhooks/{EscapeRouteValue(applicationId)}/{EscapeRouteValue(interactionToken)}/messages/@original",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteOriginalInteractionResponseAsync(
            Snowflake applicationId,
            string interactionToken,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"webhooks/{EscapeRouteValue(applicationId)}/{EscapeRouteValue(interactionToken)}/messages/@original",
                $"webhooks/{EscapeRouteValue(applicationId)}/{EscapeRouteValue(interactionToken)}/messages/@original",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Message>> CreateFollowupMessageAsync(
            Snowflake applicationId,
            string interactionToken,
            ExecuteWebhookRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Message>(
                HttpMethod.Post,
                $"webhooks/{EscapeRouteValue(applicationId)}/{EscapeRouteValue(interactionToken)}",
                $"webhooks/{EscapeRouteValue(applicationId)}/{EscapeRouteValue(interactionToken)}",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Message>> GetFollowupMessageAsync(
            Snowflake applicationId,
            string interactionToken,
            Snowflake messageId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Message>(
                HttpMethod.Get,
                $"webhooks/{EscapeRouteValue(applicationId)}/{EscapeRouteValue(interactionToken)}/messages/{EscapeRouteValue(messageId)}",
                $"webhooks/{EscapeRouteValue(applicationId)}/{EscapeRouteValue(interactionToken)}/messages/:message_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Message>> EditFollowupMessageAsync(
            Snowflake applicationId,
            string interactionToken,
            Snowflake messageId,
            EditWebhookMessageRequest body,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Message>(
                HttpMethod.Patch,
                $"webhooks/{EscapeRouteValue(applicationId)}/{EscapeRouteValue(interactionToken)}/messages/{EscapeRouteValue(messageId)}",
                $"webhooks/{EscapeRouteValue(applicationId)}/{EscapeRouteValue(interactionToken)}/messages/:message_id",
                body,
                null,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<object>> DeleteFollowupMessageAsync(
            Snowflake applicationId,
            string interactionToken,
            Snowflake messageId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<object>(
                HttpMethod.Delete,
                $"webhooks/{EscapeRouteValue(applicationId)}/{EscapeRouteValue(interactionToken)}/messages/{EscapeRouteValue(messageId)}",
                $"webhooks/{EscapeRouteValue(applicationId)}/{EscapeRouteValue(interactionToken)}/messages/:message_id",
                null,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
