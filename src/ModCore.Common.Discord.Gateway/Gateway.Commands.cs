using System.Net.WebSockets;
using System.Text;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Gateway;
using ModCore.Common.Discord.Entities.Requests;

namespace ModCore.Common.Discord.Gateway
{
    public partial class Gateway
    {
        public Task UpdatePresenceAsync(
            GatewayPresence presence,
            CancellationToken cancellationToken = default
        ) => SendCommandAsync(OpCodes.PresenceUpdate, presence, cancellationToken);

        public Task UpdateVoiceStateAsync(
            GatewayVoiceState voiceState,
            CancellationToken cancellationToken = default
        ) => SendCommandAsync(OpCodes.VoiceStateUpdate, voiceState, cancellationToken);

        public Task RequestGuildMembersAsync(
            RequestGuildMembers request,
            CancellationToken cancellationToken = default
        )
        {
            if (request.Limit < 0 || request.Limit > 1000)
                throw new ArgumentOutOfRangeException(nameof(request.Limit));
            if (request.UserIds.Value is { Length: > 100 })
                throw new ArgumentException(
                    "At most 100 user IDs may be requested.",
                    nameof(request)
                );
            if (request.Nonce.Value is { } nonce && Encoding.UTF8.GetByteCount(nonce) > 32)
                throw new ArgumentException("Nonce must not exceed 32 bytes.", nameof(request));
            if (!request.UserIds.HasValue && !request.Query.HasValue)
                request.Query = string.Empty;
            return SendCommandAsync(OpCodes.RequestGuildMembers, request, cancellationToken);
        }

        public Task RequestSoundboardSoundsAsync(
            RequestSoundboardSounds request,
            CancellationToken cancellationToken = default
        ) => SendCommandAsync(OpCodes.RequestSoundboardSounds, request, cancellationToken);

        public Task RequestChannelInfoAsync(
            RequestChannelInfo request,
            CancellationToken cancellationToken = default
        ) => SendCommandAsync(OpCodes.RequestChannelInfo, request, cancellationToken);

        private async Task SendCommandAsync<T>(
            OpCodes opcode,
            T data,
            CancellationToken cancellationToken
        )
        {
            var socket = websocket;
            if (
                socket?.State != WebSocketState.Open
                || Volatile.Read(ref connectionReady) == 0
                || lifetime == null
            )
                throw new InvalidOperationException("The gateway is not ready.");
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                lifetime.Token
            );
            var guildId = data is RequestGuildMembers members ? members.GuildId.ToString() : "";
            if (commandRetryTimes.TryGetValue(((int)opcode, guildId), out var retryAt))
            {
                var delay = retryAt - DateTimeOffset.UtcNow;
                if (delay > TimeSpan.Zero)
                    await Task.Delay(delay, linked.Token);
            }
            await SendAsync(socket, opcode, data, linked.Token);
        }
    }
}
