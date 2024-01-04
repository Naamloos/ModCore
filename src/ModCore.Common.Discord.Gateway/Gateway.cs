using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using CommunityToolkit.HighPerformance.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Serializer;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Gateway.EventData.Outgoing;
using ModCore.Common.Discord.Gateway.Events;
using System.Buffers;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;

namespace ModCore.Common.Discord.Gateway
{
    // TODO implement resume
    public partial class Gateway : IHostedService
    {
        const short API_VERSION = 10;
        const string ENCODING = "json";
        const int BUFFER_SIZE = 4096;

        public ReadyApplication? Application { get; private set; }

        private GatewayConfiguration configuration;

        private string gatewayUrl = "";

        private ClientWebSocket websocket;
        private ArrayPoolBufferWriter<byte> arrayPoolBufferWriter;
        private JsonSerializerOptions jsonSerializerOptions;
        private SemaphoreSlim sendingSemaphore;
        private Channel<Payload> gatewayEventChannel;

        private CancellationToken serviceCancellationToken;
        private CancellationTokenSource gatewayCancellationTokenSource;

        private int? lastSequenceNumber = null;

        private IServiceProvider services;
        private ILogger logger;

        private List<ISubscriber> subscribers = new();

        private string token;
        private int shard_id;
        private int shard_count;

        private Ready lastReadyEvent;

        public Gateway(Action<GatewayConfiguration> configure, IServiceProvider services)
        {
            this.services = services;
            logger = services.GetRequiredService<ILogger<Gateway>>();
            this.configuration = new GatewayConfiguration();
            configure(this.configuration);

            gatewayCancellationTokenSource = new CancellationTokenSource();

            registerSubscribersInternal();

            // fetch configuration data from host services.
            var hostConfig = services.GetRequiredService<IConfiguration>();
            token = hostConfig.GetRequiredSection("discord_token").Value;
            shard_id = int.Parse(hostConfig.GetRequiredSection("current_shard").Value);
            shard_count = int.Parse(hostConfig.GetRequiredSection("shard_count").Value);

            // Preparing base websocket uri
            var uribuilder = new UriBuilder(this.configuration.GatewayUrl);
            uribuilder.Scheme = "wss";
            uribuilder.Port = 443;
            uribuilder.Query = $"v={API_VERSION}&encoding={ENCODING}";
            gatewayUrl = uribuilder.ToString();

            this.jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions.Default)
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                Converters = { new OptionalJsonSerializerFactory() }
            };

            // Semaphore for sending websocket data
            this.sendingSemaphore = new SemaphoreSlim(1);

            this.arrayPoolBufferWriter = new ArrayPoolBufferWriter<byte>();
            this.gatewayEventChannel = Channel.CreateUnbounded<Payload>(new UnboundedChannelOptions()
            {
                SingleWriter = true,
                SingleReader = true
            });

            this.websocket = new ClientWebSocket();
        }

        private void registerSubscribersInternal()
        {
            foreach (var subscriber in configuration.subscribers)
            {
                RegisterSubscriber(subscriber);
            }
        }

        public void RegisterSubscriber(Type subscriberType)
        {
            var constructors = subscriberType.GetConstructors();
            if (constructors.Count() != 1)
            {
                throw new NotSupportedException($"Your subscriber of type {subscriberType} needs exactly 1 constructor! It has {constructors.Count()}!");
            }

            var constructor = constructors[0];
            var parameters = constructor.GetParameters().Select(x => x.ParameterType).ToArray();
            var qualifiedParameters = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] == typeof(Gateway))
                {
                    qualifiedParameters[i] = this;
                    continue;
                }
                qualifiedParameters[i] = services.GetService(parameters[i]);
            }

            var activatedSubscriber = Activator.CreateInstance(subscriberType, qualifiedParameters) as ISubscriber;
            activatedSubscriber!.Gateway = this;
            subscribers.Add(activatedSubscriber);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.serviceCancellationToken = cancellationToken;
            _ = Task.Run(GatewayMessageReceiverAsync);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (websocket.State == WebSocketState.Open)
            {
                return websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            }
            return Task.CompletedTask;
        }

        public async Task ReconnectAsync()
        {
            await StopAsync(serviceCancellationToken);
            this.websocket = new ClientWebSocket();
            await StartAsync(serviceCancellationToken);
        }

        public async Task ResumeAsync()
        {
            // cancel running gateway loops
            gatewayCancellationTokenSource.Cancel();
            // and close websocket if still running
            if (websocket.State == WebSocketState.Open)
            {
                await websocket.CloseAsync(WebSocketCloseStatus.Empty, null, serviceCancellationToken);
            }

            // reconstruct connection
            this.gatewayCancellationTokenSource = new CancellationTokenSource();
            this.websocket = new ClientWebSocket();
            await websocket.ConnectAsync(new Uri(lastReadyEvent.ResumeGatewayUrl), serviceCancellationToken);

            logger.LogInformation("Sending resume packet.");
            // Send RESUME
            await SendWebsocketPacketAsync(new Payload(OpCodes.Resume).WithData(new Resume()
            {
                LastSequenceNumber = lastSequenceNumber ?? 0,
                SessionId = lastReadyEvent.SessionId,
                Token = token
            }, jsonSerializerOptions));
        }

        private async Task GatewayMessageReceiverAsync()
        {
            await websocket.ConnectAsync(new Uri(gatewayUrl), serviceCancellationToken);
            logger.LogInformation("Successfully connected to Discord's Gateway.");

            _ = Task.Run(GatewayMessageHandlerAsync, gatewayCancellationTokenSource.Token);

            logger.LogInformation("Started Gateway Receiver Logic");
            while (!serviceCancellationToken.IsCancellationRequested
                && !gatewayCancellationTokenSource.IsCancellationRequested
                && websocket.State == WebSocketState.Open)
            {
                try
                {
                    Payload? nextEvent = await ReceiveNextWebsocketPacketAsync();
                    if (nextEvent != null)
                    {
                        await gatewayEventChannel.Writer.WriteAsync(nextEvent);
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        private async Task GatewayMessageHandlerAsync()
        {
            logger.LogInformation("Started Gateway Response Logic");
            while (!serviceCancellationToken.IsCancellationRequested && !gatewayCancellationTokenSource.IsCancellationRequested)
            {
                var gatewayEvent = await gatewayEventChannel.Reader.ReadAsync(serviceCancellationToken);
                lastSequenceNumber = gatewayEvent.Sequence ?? lastSequenceNumber;
                logger.LogDebug("Received websocket OpCode: {0}", gatewayEvent.OpCode);

                try
                {
                    switch (gatewayEvent.OpCode)
                    {
                        default:
                            logger.LogWarning("Unimplemented OpCode received: {0}.", gatewayEvent.OpCode);
                            break;

                        case OpCodes.Hello:
                            await HandleHelloAsync(gatewayEvent);
                            break;

                        case OpCodes.Dispatch:
                            logger.LogDebug("Dispatch Event: {0}", gatewayEvent.EventName);
                            await HandleDispatchAsync(gatewayEvent);
                            break;

                        case OpCodes.HeartbeatAck:
                            logger.LogInformation("Received a heartbeat acknowledgement.");
                            break;

                        case OpCodes.Reconnect:
                            logger.LogInformation("Server indicated a reconnect is required.");
                            await ResumeAsync();
                            break;

                        case OpCodes.InvalidSession:
                            logger.LogInformation("Invalid session response by server, reconnect required.");
                            await ReconnectAsync();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Error in gateway event!! {0}", ex);
                }
            }
        }

        private async Task SendWebsocketPacketAsync(Payload gatewayEvent)
        {
            await sendingSemaphore.WaitAsync(serviceCancellationToken);
            var memoryStream = new MemoryStream();

            JsonSerializer.Serialize(memoryStream, gatewayEvent, jsonSerializerOptions);

            if (memoryStream.Length > 4096)
            {
                sendingSemaphore.Release();
                logger.LogError("Gateway event being sent to Discord is too big! {0} Maximum size is {1}", gatewayEvent.EventName, 4096);
                // ThatsWhatSheSaidException
                throw new Exception($"Gateway event being sent to Discord is too big! {gatewayEvent.EventName}");
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            if (!memoryStream.TryGetBuffer(out var buffer))
            {
                sendingSemaphore.Release();
                logger.LogError($"Failed to fetch memory buffer!");
                throw new Exception($"Failed to fetch memory buffer!");
            }

            await websocket.SendAsync(buffer, WebSocketMessageType.Text, true, serviceCancellationToken);
            sendingSemaphore.Release();
            logger.LogDebug("Sent websocket OpCode: {0}", gatewayEvent.OpCode);
        }

        private async Task<Payload?> ReceiveNextWebsocketPacketAsync()
        {
            arrayPoolBufferWriter.Clear();
            ValueWebSocketReceiveResult result;
            do
            {
                var buffer = arrayPoolBufferWriter.GetMemory(BUFFER_SIZE);
                result = await websocket.ReceiveAsync(buffer, serviceCancellationToken);
                arrayPoolBufferWriter.Advance(result.Count);
            }
            while (!result.EndOfMessage);

            return JsonSerializer.Deserialize<Payload>(arrayPoolBufferWriter.WrittenSpan, jsonSerializerOptions);
        }

        private async Task GatewayHeartbeatHandlerAsync(Hello hello)
        {
            var jitter = new Random();
            while (!serviceCancellationToken.IsCancellationRequested)
            {
                await Task.Delay(hello.HeartbeatInterval + jitter.Next(0, 2));
                logger.LogInformation("Sending new Heartbeat.");
                await SendWebsocketPacketAsync(new Payload(OpCodes.Heartbeat).WithData(lastSequenceNumber, jsonSerializerOptions));
            }
        }

        private async Task HandleHelloAsync(Payload gatewayEvent)
        {
            var hello = gatewayEvent.GetDataAs<Hello>(jsonSerializerOptions);
            if (hello == null)
            {
                return;
            }

            // start heartbeat task
            _ = Task.Run(async () =>
            {
                await GatewayHeartbeatHandlerAsync(hello);
            });

            logger.LogInformation("Sending client identity.");
            // Send IDENTIFY
            await SendWebsocketPacketAsync(new Payload(OpCodes.Identify).WithData(new Identify()
            {
                Token = token,
                Intents = configuration.Intents,
                Shard = new int[] { shard_id, shard_count },
                Presence = new PresenceUpdate()
                {
                    Status = "dnd",
                    activities = configuration.Activity.HasValue? new()
                    {
                        configuration.Activity.Value
                    } : new()
                }
            }, jsonSerializerOptions));

            DispatchEventToSubscribers(hello);
        }

        private async Task HandleDispatchAsync(Payload gatewayEvent)
        {
            await Task.Yield();

            switch (gatewayEvent.EventName)
            {
                default:
                    logger.LogWarning("Received yet unknown DISPATCH event: {0}", gatewayEvent.EventName);
                    break;
                case "READY":
                    lastReadyEvent = gatewayEvent.GetDataAs<Ready>(jsonSerializerOptions)!;

                    Application = lastReadyEvent.Application;

                    logger.LogInformation("Gateway is ready for operation.");
                    logger.LogInformation("Resume Gateway URL: {0}", lastReadyEvent.ResumeGatewayUrl);
                    logger.LogInformation("Session ID: {0}", lastReadyEvent.SessionId);
                    DispatchEventToSubscribers(lastReadyEvent);
                    break;
                case "GUILD_CREATE":
                    DispatchEventToSubscribers(gatewayEvent.GetDataAs<GuildCreate>(jsonSerializerOptions));
                    break;
                case "MESSAGE_CREATE":
                    DispatchEventToSubscribers(gatewayEvent.GetDataAs<MessageCreate>(jsonSerializerOptions));
                    break;
                case "INTERACTION_CREATE":
                    DispatchEventToSubscribers(gatewayEvent.GetDataAs<InteractionCreate>(jsonSerializerOptions));
                    break;
            }
        }

        private void DispatchEventToSubscribers<T>(T? data) where T : IPublishable
        {
            if (data is null)
            {
                logger.LogError("Null data passed to event with data type {0}! This should be considered an gateway error!", typeof(T));
                return;
            }

            ParallelHelper.ForEach<ISubscriber, ParallelEventRunner<T>>(subscribers.ToArray(), new ParallelEventRunner<T>(this, data));
        }

        internal async Task runEventHandlerAsync<T>(ISubscriber<T> subscriber, T data) where T : IPublishable
        {
            try
            {
                await subscriber.HandleEvent(data);
            }
            catch (Exception ex)
            {
                logger.LogError("Caught exception of type {0}!", ex.GetType());
                logger.LogError("Event Handler: {0} ({1})", subscriber.GetType(), typeof(T).Name);
                logger.LogError("Message: {0}\n{1}", ex.Message ?? "None provided.", ex.StackTrace);
            }
        }
    }
}
