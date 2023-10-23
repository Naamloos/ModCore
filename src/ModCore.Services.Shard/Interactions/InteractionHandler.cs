using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Gateway.Events;
using ModCore.Common.Discord.Rest;
using ModCore.Services.Shard.Interactions.Attributes;
using ModCore.Services.Shard.Interactions.InteractionTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ModCore.Services.Shard.Interactions
{
    public class InteractionHandler
    {
        private readonly ILogger _logger;
        private readonly DiscordRest _rest;
        private readonly IServiceProvider _services;

        private CommandMap _commandMap;

        public InteractionHandler(ILogger<InteractionHandler> logger, DiscordRest rest, IServiceProvider services)
        {
            _logger = logger;
            _rest = rest;
            _services = services;
            _commandMap = new CommandMap();
        }

        /// <summary>
        /// Registers interactions such as slash commands and context menu items with Discord.
        /// </summary>
        /// <returns></returns>
        public async ValueTask RegisterInteractionsAsync(Snowflake appId)
        {
            _logger.LogInformation("Loading and Registering interactions...");

            // Gather all app commands into a list of entities to register with Discord.
            List<ApplicationCommand> commands = new List<ApplicationCommand>();

            var commandTypes = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetCustomAttribute<CommandAttribute>() != null && x.IsAssignableTo(typeof(BaseCommand)));
            foreach (var commandType in commandTypes)
            {
                var commandInfo = commandType.GetCustomAttribute<CommandAttribute>();
                var topLevel = Activator.CreateInstance(commandType, resolveDependenciesForType(commandType, _services));
                var invoke = commandType.GetMethods().FirstOrDefault(x => x.GetCustomAttribute<InvokeInteractionAttribute>() != null);

                var discordCommand = new ApplicationCommand()
                {
                    Name = commandInfo.Name,
                    Description = commandInfo.Description,
                    NSFW = commandInfo.NSFW,
                    CanBeUsedInDM = commandInfo.AllowDM,
                    Options = invoke != default? parseOptions(invoke) : new List<ApplicationCommandOption>()
                };

                _logger.LogInformation("Registering command: {0}", commandInfo.Name);
                _commandMap.Register(commandInfo.Name, topLevel, invoke);

                var subcommandTypes = commandType.GetNestedTypes().Where(x => x.GetCustomAttribute<SubcommandGroupAttribute>() != null);
                // Subcommand Groups
                foreach (var subclass in subcommandTypes)
                {
                    if (subclass.BaseType.GenericTypeArguments.Contains(commandType))
                    {
                        var subgroupInfo = subclass.GetCustomAttribute<SubcommandGroupAttribute>();
                        _logger.LogInformation("Registering command: {0}", $"{commandInfo.Name} {subgroupInfo.Name}");
                        var discordsubgroup = new ApplicationCommandOption()
                        {
                            Name = subgroupInfo.Name,
                            Description = subgroupInfo.Description,
                            Options = new List<ApplicationCommandOption>(),
                            Type = ApplicationCommandOptionType.SubcommandGroup
                        };
                        // seems valid
                        var subcommandClass = Activator.CreateInstance(subclass, resolveDependenciesForType(subclass, _services));
                        var fields = subclass.BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                        fields.First(x=>x.FieldType == commandType).SetValue(subcommandClass, topLevel);

                        foreach (var subsubcommand in subclass.GetMethods().Where(x => x.GetCustomAttribute<SubcommandAttribute>() != null))
                        {
                            var subcommandInfo = subsubcommand.GetCustomAttribute<SubcommandAttribute>();
                            _logger.LogInformation("Registering command: {0}", $"{commandInfo.Name} {subgroupInfo.Name} {subcommandInfo.Name}");
                            _commandMap.Register($"{commandInfo.Name} {subgroupInfo.Name} {subcommandInfo.Name}", subcommandClass, subsubcommand);
                            var discordsubsubcommand = new ApplicationCommandOption()
                            {
                                Name = subcommandInfo.Name,
                                Description = subcommandInfo.Description,
                                Type = ApplicationCommandOptionType.Subcommand,
                                Options = parseOptions(subsubcommand)
                            };
                            discordsubgroup.Options.Value.Add(discordsubsubcommand);
                        }
                        discordCommand.Options.Value.Add(discordsubgroup);
                    }
                }

                var subcommandMethods = commandType.GetMethods().Where(x => x.GetCustomAttribute<SubcommandAttribute>() != null);
                // Subcommands
                foreach (var subcommand in subcommandMethods)
                {
                    var subcommandInfo = subcommand.GetCustomAttribute<SubcommandAttribute>();
                    _logger.LogInformation("Registering command: {0}", $"{commandInfo.Name} {subcommandInfo.Name}");
                    _commandMap.Register($"{commandInfo.Name} {subcommandInfo.Name}", topLevel, subcommand);
                    var discordsubsubcommand = new ApplicationCommandOption()
                    {
                        Name = subcommandInfo.Name,
                        Description = subcommandInfo.Description,
                        Type = ApplicationCommandOptionType.Subcommand,
                        Options = parseOptions(subcommand)
                    };
                    discordCommand.Options.Value.Add(discordsubsubcommand);
                }

                commands.Add(discordCommand);
            }

            // register them
            var resp = await _rest.BulkOverwriteGlobalApplicationCommandsAsync(appId, commands.ToArray());

            if (resp.Success)
            {
                _logger.LogInformation("Successfully registered interactions with Discord!");
            }
            else
            {
                _logger.LogError("Registering interactions failed... Some interactions may not work because of this.");
            }
        }

        private object[] resolveDependenciesForType(Type type, IServiceProvider services)
        {
            var constructors = type.GetConstructors();
            if (constructors.Count() != 1)
            {
                throw new NotSupportedException($"Your type {type} needs exactly 1 constructor! It has {constructors.Count()}!");
            }

            var constructor = constructors[0];
            var parameters = constructor.GetParameters().Select(x => x.ParameterType).ToArray();
            var qualifiedParameters = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                qualifiedParameters[i] = services.GetService(parameters[i]);
            }

            return qualifiedParameters;
        }

        private Optional<List<ApplicationCommandOption>> parseOptions(MethodInfo method)
        {
            var parsedOptions = new List<ApplicationCommandOption>();

            var allParameters = method.GetParameters();
            if (allParameters.Length < 1)
                throw new NotSupportedException($"Invalid parameter structure on {method}");
            if(allParameters.First().ParameterType != typeof(InteractionCreate))
                throw new NotSupportedException($"Invalid parameter structure on {method}");

            foreach(var parameter in allParameters.Skip(1))
            {
                var paramInfo = parameter.GetCustomAttribute<ParameterAttribute>();
                var required = !parameter.ParameterType.IsAssignableTo(typeof(Optional));
                var type = required ? parameter.ParameterType : parameter.ParameterType.GetGenericArguments()[0];
                parsedOptions.Add(new ApplicationCommandOption()
                {
                    Name = parameter.Name,
                    Description = paramInfo.Description,
                    Type = getResolvedType(type, method),
                    Required = required
                });
            }

            return parsedOptions.Count < 1? Optional<List<ApplicationCommandOption>>.None : parsedOptions;
        }

        private ApplicationCommandOptionType getResolvedType(Type paramType, MethodInfo method)
        {
            // TODO type resolvers
            if (paramType == typeof(string))
                return ApplicationCommandOptionType.String;
            else if (paramType == typeof(int))
                return ApplicationCommandOptionType.Integer;
            else if (paramType == typeof(double))
                return ApplicationCommandOptionType.Number;
            else if (paramType == typeof(User))
                return ApplicationCommandOptionType.User;
            else if (paramType == typeof(Channel))
                return ApplicationCommandOptionType.Channel;

            throw new NotSupportedException($"Invalid parameter structure on {method}");
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
                var command = eventdata.Data.Value.Name;
                if(eventdata.Data.Value.options.Value?.FirstOrDefault(x => x.Type == ApplicationCommandOptionType.Subcommand) != default)
                {
                    command = command + " " + eventdata.Data.Value.options.Value.First().Name;
                }
                else if(eventdata.Data.Value?.options.Value?.FirstOrDefault(x => x.Type == ApplicationCommandOptionType.SubcommandGroup) != default)
                {
                    var group = eventdata.Data.Value.options.Value.First();
                    command = command + " " + group.Name + " " + group.Options.Value.First().Name;
                }
                _logger.LogDebug("Requested command name: {0}", command);
                await _commandMap.InvokeAsync(command, eventdata);
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
