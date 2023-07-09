using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using CommunityToolkit.HighPerformance.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Gateway.EventData.Outgoing;
using ModCore.Common.Discord.Rest;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Gateway
{
    // TODO implement resume
    public partial class Gateway : IHostedService
    {
        const short API_VERSION = 10;
        const string ENCODING = "json";
        const int BUFFER_SIZE = 4096;

        private GatewayConfiguration configuration;

        private string gatewayUrl = "";

        private ClientWebSocket websocket;
        private ArrayPoolBufferWriter<byte> arrayPoolBufferWriter;
        private JsonSerializerOptions jsonSerializerOptions;
        private SemaphoreSlim sendingSemaphore;
        private Channel<Payload> gatewayEventChannel;
        private CancellationToken cancellationToken;

        private int? lastSequenceNumber = null;

        private IServiceProvider services;
        private ILogger logger;

        public Gateway(Action<GatewayConfiguration> configure, IServiceProvider services)
        {
            this.services = services;
            logger = services.GetRequiredService<ILogger<Gateway>>();
            this.configuration = new GatewayConfiguration();
            configure(this.configuration);

            var uribuilder = new UriBuilder(this.configuration.GatewayUrl);
            uribuilder.Scheme = "wss";
            uribuilder.Port = 443;
            uribuilder.Query = $"v={API_VERSION}&encoding={ENCODING}";
            gatewayUrl = uribuilder.ToString();

            this.jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions.Default)
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            this.sendingSemaphore = new SemaphoreSlim(1);

            this.arrayPoolBufferWriter = new ArrayPoolBufferWriter<byte>();
            this.gatewayEventChannel = Channel.CreateUnbounded<Payload>(new UnboundedChannelOptions()
            {
                SingleWriter = true,
                SingleReader = true
            });

            this.websocket = new ClientWebSocket();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            _ = Task.Run(GatewayMessageReceiverAsync);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task GatewayMessageReceiverAsync()
        {
            await websocket.ConnectAsync(new Uri(gatewayUrl), cancellationToken);
            logger.LogInformation("Successfully connected to Discord's Gateway.");

            _ = Task.Run(GatewayMessageHandlerAsync, cancellationToken);

            logger.LogInformation("Started Gateway Receiver Logic");
            while (!cancellationToken.IsCancellationRequested
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
            while (!cancellationToken.IsCancellationRequested)
            {
                var gatewayEvent = await gatewayEventChannel.Reader.ReadAsync(cancellationToken);
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
            await sendingSemaphore.WaitAsync(cancellationToken);
            var memoryStream = new MemoryStream();

            JsonSerializer.Serialize(memoryStream, gatewayEvent, jsonSerializerOptions);

            if(memoryStream.Length > 4096)
            {
                sendingSemaphore.Release();
                logger.LogError("Gateway event being sent to Discord is too big! {0} Maximum size is {1}", gatewayEvent.EventName, 4096);
                // ThatsWhatSheSaidException
                throw new Exception($"Gateway event being sent to Discord is too big! {gatewayEvent.EventName}");
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            if(!memoryStream.TryGetBuffer(out var buffer))
            {
                sendingSemaphore.Release();
                logger.LogError($"Failed to fetch memory buffer!");
                throw new Exception($"Failed to fetch memory buffer!");
            }

            await websocket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
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
                result = await websocket.ReceiveAsync(buffer, cancellationToken);
                arrayPoolBufferWriter.Advance(result.Count);
            } 
            while (!result.EndOfMessage);

            return JsonSerializer.Deserialize<Payload>(arrayPoolBufferWriter.WrittenSpan, jsonSerializerOptions);
        }

        private async Task GatewayHeartbeatHandlerAsync(Hello hello)
        {
            var jitter = new Random();
            while(!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(hello.HeartbeatInterval + jitter.Next(0, 2));
                logger.LogInformation("Sending new Heartbeat.");
                await SendWebsocketPacketAsync(new Payload(OpCodes.Heartbeat).WithData(lastSequenceNumber));
            }
        }

        private async Task HandleHelloAsync(Payload gatewayEvent)
        {
            var hello = gatewayEvent.GetDataAs<Hello>();
            // start heartbeat task
            _ = Task.Run(async () =>
            {
                if (hello != null)
                {
                    await GatewayHeartbeatHandlerAsync(hello);
                }
            });

            logger.LogInformation("Sending client identity.");
            // Send IDENTIFY
            await SendWebsocketPacketAsync(new Payload(OpCodes.Identify).WithData(new Identify()
            {
                Token = configuration.Token,
                Intents = configuration.Intents
            }));
        }

        private async Task HandleDispatchAsync(Payload gatewayEvent)
        {
            await Task.Yield();

            switch(gatewayEvent.EventName)
            {
                default:
                    logger.LogWarning("Received yet unknown DISPATCH event: {0}", gatewayEvent.EventName);
                    break;
                case "READY":
                    logger.LogInformation("Gateway is ready for operation.");
                    break;
            }
        }
    }
}
