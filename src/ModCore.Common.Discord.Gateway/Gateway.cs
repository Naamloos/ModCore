using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModCore.Common.Configuration;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Gateway.EventData.Outgoing;
using ModCore.Common.Discord.Gateway.Events;
using ModCore.Common.Discord.Rest;
using ModCore.Common.Utils;
using GatewayBotInfo = ModCore.Common.Discord.Entities.Gateway.GatewayBot;

namespace ModCore.Common.Discord.Gateway
{
    public partial class Gateway : IHostedService
    {
        private readonly GatewayConfiguration configuration;
        private readonly IServiceProvider services;
        private readonly ILogger logger;
        private readonly JsonSerializerOptions jsonSerializerOptions =
            JsonSerializerOptionsFactory.GetOptions();
        private readonly SemaphoreSlim sendingSemaphore = new(1, 1);
        private readonly List<ISubscriber> subscribers = new();
        private readonly string token;
        private readonly int shard_id;
        private readonly int shard_count;
        private readonly object lifecycleLock = new();
        private CancellationTokenSource? lifetime;
        private Task? runner;
        private ClientWebSocket? websocket;
        private volatile Ready? lastReadyEvent;
        private int sequence = -1;
        private int? lastSequenceNumber
        {
            get
            {
                var value = Volatile.Read(ref sequence);
                return value < 0 ? null : value;
            }
            set => Volatile.Write(ref sequence, value ?? -1);
        }
        private int freshSessionRequested;
        private int awaitingHeartbeatAck;
        private int connectionReady;
        private readonly Queue<DateTimeOffset> sentPackets = new();
        private readonly Queue<DateTimeOffset> presenceUpdates = new();
        private readonly ConcurrentDictionary<
            (int Opcode, string GuildId),
            DateTimeOffset
        > commandRetryTimes = new();
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> identifyLocks = new();
        private static readonly ConcurrentDictionary<string, DateTimeOffset> identifyTimes = new();

        public ReadyApplication? Application { get; private set; }
        public int ShardId => shard_id;
        public int ShardCount => shard_count;

        public Gateway(Action<GatewayConfiguration> configure, IServiceProvider services)
        {
            this.services = services;
            logger = services.GetRequiredService<ILogger<Gateway>>();
            configuration = new GatewayConfiguration();
            configure(configuration);
            var host = services.GetRequiredService<IConfiguration>();
            token = host.GetRequiredSection(
                ConfigurationHelper.GetConfigKeyString(ConfigKey.DiscordToken)
            ).Value!;
            shard_id = int.Parse(
                host.GetRequiredSection(
                    ConfigurationHelper.GetConfigKeyString(ConfigKey.CurrentShard)
                ).Value!
            );
            shard_count = int.Parse(
                host.GetRequiredSection(
                    ConfigurationHelper.GetConfigKeyString(ConfigKey.ShardCount)
                ).Value!
            );
            if (shard_count < 1 || shard_id < 0 || shard_id >= shard_count)
                throw new ArgumentException(
                    "Shard ID must be between zero and shard count minus one."
                );
            foreach (var subscriber in configuration.subscribers)
                RegisterSubscriber(subscriber);
        }

        public void RegisterSubscriber(Type subscriberType)
        {
            if (!typeof(ISubscriber).IsAssignableFrom(subscriberType))
                throw new ArgumentException(
                    "The type must implement ISubscriber.",
                    nameof(subscriberType)
                );
            var takesGateway = subscriberType
                .GetConstructors()
                .Any(c => c.GetParameters().Any(p => p.ParameterType == typeof(Gateway)));
            var subscriber = (ISubscriber)
                ActivatorUtilities.CreateInstance(
                    services,
                    subscriberType,
                    takesGateway ? [this] : []
                );
            subscriber.Gateway = this;
            lock (subscribers)
                subscribers.Add(subscriber);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lock (lifecycleLock)
            {
                if (runner is { IsCompleted: false })
                    return Task.CompletedTask;
                lifetime?.Dispose();
                lifetime = new CancellationTokenSource();
                runner = RunAsync(lifetime.Token);
            }
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Task? running;
            lock (lifecycleLock)
            {
                lifetime?.Cancel();
                websocket?.Abort();
                running = runner;
            }
            if (running != null)
            {
                try
                {
                    await running.WaitAsync(cancellationToken);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                { }
            }
        }

        public Task ReconnectAsync()
        {
            Interlocked.Exchange(ref freshSessionRequested, 1);
            lock (lifecycleLock)
                websocket?.Abort();
            return Task.CompletedTask;
        }

        public Task ResumeAsync()
        {
            lock (lifecycleLock)
                websocket?.Abort();
            return Task.CompletedTask;
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var failures = 0;
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (Interlocked.Exchange(ref freshSessionRequested, 0) != 0)
                    {
                        lastReadyEvent = null;
                        lastSequenceNumber = null;
                        Application = null;
                    }
                    using var connection = CancellationTokenSource.CreateLinkedTokenSource(
                        cancellationToken
                    );
                    using var socket = new ClientWebSocket();
                    lock (lifecycleLock)
                        websocket = socket;
                    Interlocked.Exchange(ref connectionReady, 0);
                    Task? heartbeat = null;
                    Task? handshake = null;
                    var reconnectDelay = TimeSpan.Zero;
                    try
                    {
                        GatewayBotInfo? discovery = null;
                        if (lastReadyEvent == null)
                        {
                            var rest =
                                services.GetService<DiscordRest>()
                                ?? new DiscordRest(c => { }, services);
                            var response = await rest.GetGatewayBotAsync(
                                cancellationToken: cancellationToken
                            );
                            using (response.HttpResponse)
                            {
                                if (!response.Success || response.Value == null)
                                    throw new InvalidOperationException(
                                        "Failed to discover the Discord gateway."
                                    );
                                discovery = response.Value;
                            }
                            if (discovery.SessionStartLimit.Remaining <= 0)
                            {
                                await Task.Delay(
                                    TimeSpan.FromMilliseconds(
                                        discovery.SessionStartLimit.ResetAfter
                                    ),
                                    cancellationToken
                                );
                                continue;
                            }
                        }
                        var url =
                            lastReadyEvent?.ResumeGatewayUrl
                            ?? (
                                configuration.GatewayUrl != "gateway.discord.gg"
                                    ? configuration.GatewayUrl
                                    : discovery!.Url
                            );
                        var uri = new UriBuilder(url.Contains("://") ? url : "wss://" + url)
                        {
                            Scheme = "wss",
                            Port = 443,
                            Query = "v=10&encoding=json",
                        };
                        await socket.ConnectAsync(uri.Uri, cancellationToken);
                        var helloPacket = await ReceiveAsync(socket, connection.Token)
                            .WaitAsync(TimeSpan.FromSeconds(30), connection.Token);
                        if (helloPacket?.OpCode == OpCodes.Reconnect)
                            continue;
                        if (helloPacket?.OpCode != OpCodes.Hello)
                            throw new InvalidOperationException("Expected gateway Hello.");
                        var hello = helloPacket.GetDataAs<Hello>(jsonSerializerOptions)!;
                        if (hello.HeartbeatInterval <= 0)
                            throw new InvalidOperationException("Invalid heartbeat interval.");
                        Interlocked.Exchange(ref awaitingHeartbeatAck, 0);
                        heartbeat = HeartbeatAsync(
                            socket,
                            hello.HeartbeatInterval,
                            connection.Token
                        );
                        handshake = IdentifyOrResumeAsync(socket, discovery, connection.Token);
                        DispatchEventToSubscribers(hello);
                        while (socket.State == WebSocketState.Open)
                        {
                            var packet = await ReceiveAsync(socket, connection.Token);
                            if (packet == null)
                                break;
                            if (packet.Sequence.HasValue)
                                lastSequenceNumber = packet.Sequence;
                            switch (packet.OpCode)
                            {
                                case OpCodes.Dispatch:
                                    if (packet.EventName is "READY" or "RESUMED")
                                    {
                                        failures = 0;
                                        Interlocked.Exchange(ref connectionReady, 1);
                                    }
                                    await HandleDispatchAsync(packet);
                                    break;
                                case OpCodes.Heartbeat:
                                    await SendHeartbeatAsync(socket, connection.Token);
                                    break;
                                case OpCodes.HeartbeatAck:
                                    Interlocked.Exchange(ref awaitingHeartbeatAck, 0);
                                    break;
                                case OpCodes.Reconnect:
                                    socket.Abort();
                                    break;
                                case OpCodes.InvalidSession:
                                    if (packet.Data.ValueKind != JsonValueKind.True)
                                        Interlocked.Exchange(ref freshSessionRequested, 1);
                                    reconnectDelay = TimeSpan.FromSeconds(Random.Shared.Next(1, 6));
                                    socket.Abort();
                                    break;
                            }
                        }
                        var closeCode = (int?)socket.CloseStatus;
                        if (closeCode is 4004 or 4010 or 4011 or 4012 or 4013 or 4014)
                        {
                            logger.LogError(
                                "Discord closed the gateway with fatal code {Code}.",
                                closeCode
                            );
                            return;
                        }
                        if (closeCode is 1000 or 1001 or 4007 or 4009)
                            Interlocked.Exchange(ref freshSessionRequested, 1);
                        if (closeCode == 4008)
                            reconnectDelay = TimeSpan.FromSeconds(5);
                    }
                    catch (OperationCanceledException)
                        when (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        var closeCode = (int?)socket.CloseStatus;
                        if (closeCode is 4004 or 4010 or 4011 or 4012 or 4013 or 4014)
                        {
                            logger.LogError(
                                "Discord closed the gateway with fatal code {Code}.",
                                closeCode
                            );
                            return;
                        }
                        if (closeCode is 1000 or 1001 or 4007 or 4009)
                            Interlocked.Exchange(ref freshSessionRequested, 1);
                        failures++;
                        logger.LogWarning(
                            "Gateway connection interrupted ({ExceptionType}); reconnecting.",
                            ex.GetType().Name
                        );
                        reconnectDelay = TimeSpan.FromSeconds(
                            Math.Min(30, Math.Pow(2, Math.Min(failures, 5)))
                                + Random.Shared.NextDouble()
                        );
                    }
                    finally
                    {
                        Interlocked.Exchange(ref connectionReady, 0);
                        lock (lifecycleLock)
                        {
                            if (ReferenceEquals(websocket, socket))
                                websocket = null;
                        }
                        connection.Cancel();
                        socket.Abort();
                        foreach (var task in new[] { heartbeat, handshake })
                        {
                            if (task == null)
                                continue;
                            try
                            {
                                await task;
                            }
                            catch (Exception ex)
                            {
                                logger.LogDebug(
                                    "Gateway connection task ended: {Type}.",
                                    ex.GetType().Name
                                );
                            }
                        }
                    }
                    if (reconnectDelay > TimeSpan.Zero)
                        await Task.Delay(reconnectDelay, cancellationToken);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
            finally
            {
                websocket = null;
            }
        }

        private async Task IdentifyOrResumeAsync(
            ClientWebSocket socket,
            GatewayBotInfo? discovery,
            CancellationToken cancellationToken
        )
        {
            try
            {
                if (lastReadyEvent != null && lastSequenceNumber.HasValue)
                {
                    await SendAsync(
                        socket,
                        OpCodes.Resume,
                        new Resume
                        {
                            Token = token,
                            SessionId = lastReadyEvent.SessionId,
                            LastSequenceNumber = lastSequenceNumber.Value,
                        },
                        cancellationToken
                    );
                    return;
                }
                var concurrency = Math.Max(1, discovery!.SessionStartLimit.MaxConcurrency);
                var key = token + ":" + shard_id % concurrency;
                var gate = identifyLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
                await gate.WaitAsync(cancellationToken);
                try
                {
                    var delay = identifyTimes.GetValueOrDefault(key) - DateTimeOffset.UtcNow;
                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay, cancellationToken);
                    if (configuration.WaitForIdentifyAsync != null)
                        await configuration.WaitForIdentifyAsync(
                            shard_id,
                            concurrency,
                            cancellationToken
                        );
                    await SendAsync(
                        socket,
                        OpCodes.Identify,
                        new Identify
                        {
                            Token = token,
                            Intents = configuration.Intents,
                            Shard = [shard_id, shard_count],
                            Presence = new EventData.Outgoing.PresenceUpdate
                            {
                                Status = "dnd",
                                activities = configuration.Activity.Value is { } activity
                                    ? [activity]
                                    : [],
                            },
                        },
                        cancellationToken
                    );
                    identifyTimes[key] = DateTimeOffset.UtcNow.AddSeconds(5);
                }
                finally
                {
                    gate.Release();
                }
            }
            catch
            {
                socket.Abort();
                throw;
            }
        }

        private async Task HeartbeatAsync(
            ClientWebSocket socket,
            int interval,
            CancellationToken cancellationToken
        )
        {
            try
            {
                await Task.Delay(
                    TimeSpan.FromMilliseconds(interval * Random.Shared.NextDouble()),
                    cancellationToken
                );
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (Volatile.Read(ref awaitingHeartbeatAck) != 0)
                    {
                        socket.Abort();
                        return;
                    }
                    await SendHeartbeatAsync(socket, cancellationToken);
                    await Task.Delay(interval, cancellationToken);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
            catch
            {
                socket.Abort();
                throw;
            }
        }

        private Task SendHeartbeatAsync(ClientWebSocket socket, CancellationToken cancellationToken)
        {
            Interlocked.Exchange(ref awaitingHeartbeatAck, 1);
            return SendAsync(socket, OpCodes.Heartbeat, lastSequenceNumber, cancellationToken);
        }

        private async Task SendAsync<T>(
            ClientWebSocket socket,
            OpCodes op,
            T data,
            CancellationToken cancellationToken
        )
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(
                new Payload(op).WithData(data, jsonSerializerOptions),
                jsonSerializerOptions
            );
            if (bytes.Length > 4096)
                throw new ArgumentException("Gateway payload exceeds 4096 bytes.", nameof(data));
            await sendingSemaphore.WaitAsync(cancellationToken);
            try
            {
                var now = DateTimeOffset.UtcNow;
                while (sentPackets.TryPeek(out var sent) && sent <= now.AddSeconds(-60))
                    sentPackets.Dequeue();
                // Reserve capacity for heartbeats and session management instead of delaying them behind user commands.
                var control = op is OpCodes.Heartbeat or OpCodes.Identify or OpCodes.Resume;
                if (sentPackets.Count >= (control ? 120 : 110))
                    throw new InvalidOperationException(
                        "Gateway send limit reached; retry after the current minute window."
                    );
                if (op == OpCodes.PresenceUpdate)
                {
                    while (presenceUpdates.TryPeek(out var sent) && sent <= now.AddSeconds(-20))
                        presenceUpdates.Dequeue();
                    if (presenceUpdates.Count >= 5)
                        throw new InvalidOperationException("Presence update limit reached.");
                    presenceUpdates.Enqueue(now);
                }
                await socket.SendAsync(
                    bytes.AsMemory(),
                    WebSocketMessageType.Text,
                    true,
                    cancellationToken
                );
                sentPackets.Enqueue(now);
            }
            finally
            {
                sendingSemaphore.Release();
            }
        }

        private async Task<Payload?> ReceiveAsync(
            ClientWebSocket socket,
            CancellationToken cancellationToken
        )
        {
            using var stream = new MemoryStream();
            var buffer = new byte[8192];
            ValueWebSocketReceiveResult result;
            do
            {
                result = await socket.ReceiveAsync(buffer.AsMemory(), cancellationToken);
                if (result.MessageType == WebSocketMessageType.Close)
                    return null;
                if (result.MessageType != WebSocketMessageType.Text)
                    throw new InvalidOperationException("Expected JSON text gateway frame.");
                stream.Write(buffer, 0, result.Count);
            } while (!result.EndOfMessage);
            return JsonSerializer.Deserialize<Payload>(
                stream.GetBuffer().AsSpan(0, (int)stream.Length),
                jsonSerializerOptions
            );
        }

        private void DispatchEventToSubscribers<T>(T? data)
            where T : IPublishable
        {
            if (data == null)
                return;
            ISubscriber[] targets;
            lock (subscribers)
                targets = subscribers.ToArray();
            foreach (var subscriber in targets)
                if (subscriber is ISubscriber<T> typed)
                    _ = Task.Run(() => runEventHandlerAsync(typed, data));
        }

        internal async Task runEventHandlerAsync<T>(ISubscriber<T> subscriber, T data)
            where T : IPublishable
        {
            try
            {
                await subscriber.HandleEvent(data);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Gateway subscriber {Subscriber} failed for {Event}.",
                    subscriber.GetType().Name,
                    typeof(T).Name
                );
            }
        }
    }
}
