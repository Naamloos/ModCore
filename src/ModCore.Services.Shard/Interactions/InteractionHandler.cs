using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Gateway.Events;
using ModCore.Common.Discord.Rest;
using ModCore.Services.Shard.Interactions.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ModCore.Services.Shard.Interactions
{
    public class InteractionHandler
    {
        private readonly ILogger _logger;
        private readonly DiscordRest _rest;

        private readonly ConcurrentDictionary<string, RegisteredCommandData> _commandBelongsToType;
        private readonly ConcurrentDictionary<Type, BaseInteractionContainer> _registeredTypes;
        private readonly IServiceProvider _services;

        private bool loaded = false;

        public InteractionHandler(ILogger<InteractionHandler> logger, DiscordRest rest, IServiceProvider services)
        {
            _logger = logger;
            _rest = rest;
            _commandBelongsToType = new ConcurrentDictionary<string, RegisteredCommandData>();
            _registeredTypes = new ConcurrentDictionary<Type, BaseInteractionContainer>();
            _services = services;
        }

        /// <summary>
        /// Preloads interactions from codebase
        /// </summary>
        public void LoadInteractions()
        {
            _logger.LogInformation("Loading interactions...");

            var types = Assembly.GetExecutingAssembly().DefinedTypes.Where(x => x.IsAssignableTo(typeof(BaseInteractionContainer)) && x != typeof(BaseInteractionContainer));
            foreach (var type in types)
            {
                // first we construct an instance of our type with dep. injection
                var constructors = type.GetConstructors();
                if (constructors.Count() != 1)
                {
                    throw new NotSupportedException($"Your InteractionContainer of type {type} needs exactly 1 constructor! It has {constructors.Count()}!");
                }

                var constructor = constructors[0];
                var parameters = constructor.GetParameters().Select(x => x.ParameterType).ToArray();
                var qualifiedParameters = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    qualifiedParameters[i] = _services.GetService(parameters[i]);
                }
                _registeredTypes.TryAdd(type, Activator.CreateInstance(type, qualifiedParameters) as BaseInteractionContainer);
                _logger.LogInformation("Registered Interaction Container type {0}", type.Name);

                // Then, we try to register any commands we can find
                var commands = type.GetMethods().Where(x => x.GetCustomAttribute<CommandAttribute>() != null);
                foreach (var command in commands)
                {
                    var attr = command.GetCustomAttribute<CommandAttribute>();
                    _commandBelongsToType.TryAdd(attr.Name, new RegisteredCommandData()
                    {
                        Command = attr,
                        ContainerType = type,
                        MethodInfo = command
                    });
                    _logger.LogInformation(" └ Registered Command for type {0} with name {1}", type.Name, attr.Name);
                }
            }

            loaded = true;
            _logger.LogInformation("Loading interactions done!");
        }

        /// <summary>
        /// Registers interactions such as slash commands and context menu items with Discord.
        /// </summary>
        /// <returns></returns>
        public async ValueTask RegisterInteractionsAsync(Snowflake appId)
        {
            if (!loaded)
            {
                _logger.LogError("Interactions were not loaded, thus registering failed!");
                return;
            }

            _logger.LogInformation("Registering interactions...");

            // Gather all app commands into a list of entities to register with Discord.
            List<ApplicationCommand> commands = new List<ApplicationCommand>();
            foreach (var command in _commandBelongsToType)
            {
                commands.Add(new ApplicationCommand()
                {
                    Name = command.Key,
                    Description = command.Value.Command.Description,
                    CanBeUsedInDM = command.Value.Command.AllowDM,
                    NSFW = command.Value.Command.NSFW,
                    Type = ApplicationCommandType.ChatInput
                });
            }

            // register them
            var resp = await _rest.BulkOverwriteGlobalApplicationCommandsAsync(appId, commands.ToArray());

            if (resp.Success)
            {
                _logger.LogInformation("Successfully registered interactions with Discord!");
            }
            else
            {
                _logger.LogError("Registering interactions failed... Some interactions may not work.");
            }
        }

        /// <summary>
        /// Handles an interaction from a received event.
        /// </summary>
        /// <returns></returns>
        public async ValueTask HandleInteractionAsync(InteractionCreate eventdata)
        {
            string username = eventdata.User.HasValue ? eventdata.User.Value.Username : (eventdata.Member.HasValue ? eventdata.Member.Value.User.Value.Username : "UNKNOWN");
            _logger.LogDebug("Interaction received from user {1} with type: {0}", username, Enum.GetName(eventdata.Type));

            if (eventdata.Type == InteractionType.ApplicationCommand)
            {
                _logger.LogDebug("Requested command name: {0}", eventdata.Data.Value.Name);
                if (_commandBelongsToType.TryGetValue(eventdata.Data.Value.Name, out var commandData))
                {
                    if (_registeredTypes.TryGetValue(commandData.ContainerType, out var container))
                    {
                        var commandTask = (Task)commandData.MethodInfo.Invoke(container, new object[] { eventdata });
                        _logger.LogDebug("Succesfully resolved slash command {0} for user {1}, executing...",
                            commandData.Command.Name, eventdata.Member.Value.User.Value.Username);
                        await commandTask;
                    }
                }
            }
        }
    }

    public static class InteractionHandlerServiceHelper
    {
        public static IServiceCollection AddInteractions(this IServiceCollection services)
        {
            services.AddSingleton<InteractionHandler>();
            return services;
        }
    }
}
