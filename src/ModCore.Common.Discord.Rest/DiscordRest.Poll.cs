using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Entities.Requests;

namespace ModCore.Common.Discord.Rest
{
    public partial class DiscordRest
    {
        public ValueTask<RestResponse<PollVoters>> GetAnswerVotersAsync(
            Snowflake channelId,
            Snowflake messageId,
            int answerId,
            GetAnswerVotersQuery? query = null,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<PollVoters>(
                HttpMethod.Get,
                $"channels/{EscapeRouteValue(channelId)}/polls/{EscapeRouteValue(messageId)}/answers/{EscapeRouteValue(answerId)}",
                $"channels/{EscapeRouteValue(channelId)}/polls/:message_id/answers/:answer_id",
                null,
                query,
                auditLogReason,
                cancellationToken
            );

        public ValueTask<RestResponse<Message>> EndPollAsync(
            Snowflake channelId,
            Snowflake messageId,
            string? auditLogReason = null,
            CancellationToken cancellationToken = default
        ) =>
            SendRouteAsync<Message>(
                HttpMethod.Post,
                $"channels/{EscapeRouteValue(channelId)}/polls/{EscapeRouteValue(messageId)}/expire",
                $"channels/{EscapeRouteValue(channelId)}/polls/:message_id/expire",
                null,
                null,
                auditLogReason,
                cancellationToken
            );
    }
}
