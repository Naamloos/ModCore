using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Rest;
using ModCore.Common.PubSub;
using ModCore.Common.PubSub.Payloads;
using ModCore.Services.Consumer.Handlers;
using ModCore.Services.Consumer.Interactions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ModCore.Services.Consumer
{
    public class ConsumerHostedService : IHostedService
    {
        private readonly PubSubService _pubsub;
        private readonly ILogger _logger;
        private readonly IDictionary<Type, BaseHandler> _handlers;
        private readonly DiscordRest _rest;
        private readonly IEnumerable<IApplicationCommand> _commands;
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ConsumerHostedService(PubSubService pubsub, 
            ILogger<ConsumerHostedService> logger, 
            IServiceProvider services, 
            IEnumerable<IApplicationCommand> commands, 
            DiscordRest rest, 
            IConfiguration configuration,
            JsonSerializerOptions jsonSerializerOptions) 
        {
            _pubsub = pubsub;
            _logger = logger;
            _rest = rest;
            _commands = commands;
            _configuration = configuration;
            _jsonSerializerOptions = jsonSerializerOptions;

            var handlerTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(BaseHandler)));
            // Create a dictionary of handlers
            _handlers = new Dictionary<Type, BaseHandler>();
            foreach (var handlerType in handlerTypes)
            {
                _handlers.Add(handlerType.BaseType!.GetGenericTypeDefinition().GetGenericArguments()[0], (BaseHandler)services.GetRequiredService(handlerType));
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // TODO Subcommands support, Arguments support
            ApplicationCommand[] commands = _commands.Select(c => c.BuildAsCommand()).ToArray();

            //var application = await _rest.GetApplicationAsync(data.Application.Id);
            var appId = 811197813043494942u;

            var existingCommands = await _rest.GetGlobalApplicationCommandsAsync(appId);
            if (existingCommands.Success)
            {
                foreach (var command in existingCommands.Value)
                {
                    if (command.Name.ToLower() == "launch" && command.Handler == 2)
                    {
                        // This is a discord_launch_activity command, delete it.
                        await _rest.DeleteGlobalApplicationCommandAsync(appId, command.Id);
                        break; // Only one command of this type can exist, so we can break after finding it.
                    }
                }
            }

            await _rest.BulkOverwriteGlobalApplicationCommandsAsync(appId, commands);

            await _pubsub.SubscribeAsync<InteractionCreatePayload>(async (payload, cancellationToken) =>
            {
                var handler = _commands.FirstOrDefault(c => c.Name == payload.Interaction.Data.Value?.Name);
                if(handler != default)
                {
                    await handler.InvokeAsync(payload.Interaction, _jsonSerializerOptions);
                }
            }, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            
        }
    }
}
