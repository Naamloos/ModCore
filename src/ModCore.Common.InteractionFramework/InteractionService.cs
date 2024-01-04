using Microsoft.Extensions.DependencyInjection;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Gateway;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Rest;
using System.Reflection;

namespace ModCore.Common.InteractionFramework
{
    public class InteractionService
    {
        private DiscordRest Rest { get; set; }

        private List<ApplicationCommand> Commands { get; set; }
        private Dictionary<string, Func<SlashCommandContext, Task>> CommandHandlers { get; set; }
        private IServiceProvider Services { get; set; }

        public InteractionService(DiscordRest rest, IServiceProvider services)
        {
            this.Rest = rest;
            this.Commands = new List<ApplicationCommand>();
            this.CommandHandlers = new();
            Services = services;
        }

        /// <summary>
        /// Registers all commands available in an assembly
        /// </summary>
        /// <param name="assembly"></param>
        public void RegisterCommands(Assembly assembly)
        {
            var handlers = assembly.GetTypes().Where(x => x.IsAssignableTo(typeof(BaseCommandHandler)));
            foreach(var handler in handlers) 
            {
                List<object> activatedServices = new();
                var constructorParams = handler.GetConstructors()[0].GetParameters();
                foreach(var parameter in constructorParams)
                {
                    activatedServices.Add(Services.GetService(parameter.ParameterType)!);
                }

                var enabledHandler = Activator.CreateInstance(handler, activatedServices.ToArray()) as BaseCommandHandler;
                if(enabledHandler is null)
                {
                    continue;
                }

                var loaded = enabledHandler.LoadSlashCommands(Services);
                Commands.AddRange(loaded.Item2);
                foreach(var command in loaded.Item1)
                {
                    CommandHandlers.Add(command.Key, command.Value);
                }
            }
        }

        public async Task PublishCommands(Snowflake appId)
        {
            await Rest.BulkOverwriteGlobalApplicationCommandsAsync(appId, [.. Commands]);
        }

        public void Start(Gateway gateway)
        {
            gateway.RegisterSubscriber(typeof(InteractionEventHandler));
        }

        internal async Task HandleInteractionAsync(Gateway gateway, InteractionCreate interactionCreate)
        {
            if(interactionCreate.Type == InteractionType.ApplicationCommand)
            {
                // This is an application command
                if(interactionCreate.Data.Value.Type == ApplicationCommandType.ChatInput)
                {
                    // This is a chat command
                    List<ApplicationCommandInteractionDataOption> options = interactionCreate.Data.Value.Options;

                    var qualifiedName = interactionCreate.Data.Value.Name;
                    if (interactionCreate.Data.Value.Options.HasValue)
                    {
                        if (interactionCreate.Data.Value.Options.Value.Any(x => x.Type == ApplicationCommandOptionType.Subcommand))
                        {
                            var subCommand = interactionCreate.Data.Value.Options.Value.First();
                            qualifiedName += $" {subCommand.Name}";
                            options = subCommand.Options;
                        }
                        else if (interactionCreate.Data.Value.Options.Value.Any(x => x.Type == ApplicationCommandOptionType.SubcommandGroup))
                        {
                            var subCommandGroup = interactionCreate.Data.Value.Options.Value.First();
                            var subCommand = subCommandGroup.Options.Value.First();
                            qualifiedName += $" {subCommandGroup.Name} {subCommand}";
                            options = subCommand.Options;
                        }
                    }

                    await CommandHandlers[qualifiedName].Invoke(new SlashCommandContext(interactionCreate, Rest, gateway, options, Services));
                }
            }
        }
    }
}
