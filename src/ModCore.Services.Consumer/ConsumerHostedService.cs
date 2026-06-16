using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Rest;
using ModCore.Common.PubSub;
using ModCore.Common.PubSub.Payloads;
using ModCore.Services.Consumer.Handlers;
using ModCore.Services.Consumer.Interactions.Framework;
using System.Reflection;
using System.Text.Json;

namespace ModCore.Services.Consumer;

public class ConsumerHostedService : IHostedService
{
    private readonly PubSubService _pubsub;
    private readonly ILogger _logger;
    private readonly IDictionary<Type, BaseHandler> _handlers;
    private readonly DiscordRest _rest;
    private readonly IEnumerable<IApplicationCommand> _commands;
    private readonly IEnumerable<IApplicationSubcommand> _subcommands;
    private readonly IConfiguration _configuration;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ConsumerHostedService(
        PubSubService pubsub,
        ILogger<ConsumerHostedService> logger,
        IServiceProvider services,
        IEnumerable<IApplicationCommand> commands,
        IEnumerable<IApplicationSubcommand> subcommands,
        DiscordRest rest,
        IConfiguration configuration,
        JsonSerializerOptions jsonSerializerOptions)
    {
        _pubsub = pubsub;
        _logger = logger;
        _rest = rest;
        _commands = commands;
        _subcommands = subcommands;
        _configuration = configuration;
        _jsonSerializerOptions = jsonSerializerOptions;

        var handlerTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(BaseHandler)));

        _handlers = new Dictionary<Type, BaseHandler>();

        foreach (var handlerType in handlerTypes)
        {
            var payloadType = FindBaseHandlerPayloadType(handlerType);

            if (payloadType is null)
            {
                continue;
            }

            var handler = (BaseHandler)services.GetRequiredService(handlerType);
            _handlers.Add(payloadType, handler);
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        ApplicationCommand[] commands = BuildApplicationCommands();

        var appId = 811197813043494942u;

        var existingCommands = await _rest.GetGlobalApplicationCommandsAsync(appId);

        if (existingCommands.Success)
        {
            foreach (var command in existingCommands.Value)
            {
                if (command.Name.Equals("launch", StringComparison.OrdinalIgnoreCase) &&
                    command.Handler == 2)
                {
                    await _rest.DeleteGlobalApplicationCommandAsync(appId, command.Id);
                    break;
                }
            }
        }

        await _rest.BulkOverwriteGlobalApplicationCommandsAsync(appId, commands);

        await _pubsub.SubscribeAsync<InteractionCreatePayload>(async (payload, cancellationToken) =>
        {
            if (!payload.Interaction.Data.HasValue)
            {
                return;
            }

            var commandName = payload.Interaction.Data.Value.Name;
            var route = GetSubcommandRoute(payload.Interaction);

            if (route.SubcommandName is not null)
            {
                var subcommand = _subcommands.FirstOrDefault(x =>
                    x.ParentName.Equals(commandName, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(x.GroupName, route.GroupName, StringComparison.OrdinalIgnoreCase) &&
                    x.Name.Equals(route.SubcommandName, StringComparison.OrdinalIgnoreCase));

                if (subcommand is not null)
                {
                    await subcommand.InvokeAsync(
                        payload.Interaction,
                        _jsonSerializerOptions);

                    return;
                }

                _logger.LogWarning(
                    "No subcommand handler found for command '{CommandName}', group '{GroupName}', subcommand '{SubcommandName}'.",
                    commandName,
                    route.GroupName,
                    route.SubcommandName);

                return;
            }

            var command = _commands.FirstOrDefault(x =>
                x.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase));

            if (command is not null)
            {
                await command.InvokeAsync(
                    payload.Interaction,
                    _jsonSerializerOptions);

                return;
            }

            _logger.LogWarning(
                "No command handler found for command '{CommandName}'.",
                commandName);
        }, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private ApplicationCommand[] BuildApplicationCommands()
    {
        var subcommandsByParent = _subcommands
            .GroupBy(x => x.ParentName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                x => x.Key,
                x => x.ToArray(),
                StringComparer.OrdinalIgnoreCase);

        var result = new List<ApplicationCommand>();

        foreach (var command in _commands)
        {
            var builtCommand = command.BuildAsCommand();

            if (subcommandsByParent.TryGetValue(command.Name, out var childSubcommands))
            {
                builtCommand.Options = BuildSubcommandOptions(childSubcommands);
            }

            result.Add(builtCommand);
        }

        return result.ToArray();
    }

    private static List<ApplicationCommandOption> BuildSubcommandOptions(
        IEnumerable<IApplicationSubcommand> subcommands)
    {
        var result = new List<ApplicationCommandOption>();

        var directSubcommands = subcommands
            .Where(x => string.IsNullOrWhiteSpace(x.GroupName))
            .OrderBy(x => x.Name);

        foreach (var subcommand in directSubcommands)
        {
            result.Add(subcommand.BuildAsSubcommand());
        }

        var groupedSubcommands = subcommands
            .Where(x => !string.IsNullOrWhiteSpace(x.GroupName))
            .GroupBy(x => x.GroupName!, StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x.Key);

        foreach (var group in groupedSubcommands)
        {
            var first = group.First();

            result.Add(new ApplicationCommandOption
            {
                Name = group.Key,
                Description = first.GroupDescription ?? $"{group.Key} commands",
                Type = ApplicationCommandOptionType.SubcommandGroup,
                Options = group
                    .OrderBy(x => x.Name)
                    .Select(x => x.BuildAsSubcommand())
                    .ToList()
            });
        }

        return result;
    }

    private static (string? GroupName, string? SubcommandName) GetSubcommandRoute(
        Interaction interaction)
    {
        if (!interaction.Data.HasValue ||
            !interaction.Data.Value.Options.HasValue)
        {
            return (null, null);
        }

        var first = interaction.Data.Value.Options.Value.FirstOrDefault();

        if (first is null)
        {
            return (null, null);
        }

        if (first.Type == ApplicationCommandOptionType.Subcommand)
        {
            return (null, first.Name);
        }

        if (first.Type == ApplicationCommandOptionType.SubcommandGroup)
        {
            var child = first.Options.HasValue
                ? first.Options.Value.FirstOrDefault()
                : null;

            return (first.Name, child?.Name);
        }

        return (null, null);
    }

    private static Type? FindBaseHandlerPayloadType(Type handlerType)
    {
        var current = handlerType.BaseType;

        while (current is not null)
        {
            if (current.IsGenericType &&
                current.GetGenericTypeDefinition() == typeof(BaseHandler<>))
            {
                return current.GetGenericArguments()[0];
            }

            current = current.BaseType;
        }

        return null;
    }
}