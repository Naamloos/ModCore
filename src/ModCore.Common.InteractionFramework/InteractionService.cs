using Microsoft.Extensions.DependencyInjection;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Gateway;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Rest;
using System.ComponentModel.Design;
using System.Linq.Expressions;
using System.Reflection;

namespace ModCore.Common.InteractionFramework
{
    public class InteractionService
    {
        private DiscordRest Rest { get; set; }

        private List<ApplicationCommand> Commands { get; set; }
        private Dictionary<string, ExecutableCommand> CommandHandlers { get; set; }
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
                    //Expression<Func<SlashCommandContext, ValueTask>> expression = x => command.Value(x);

                    CommandHandlers.Add(command.Key, new ExecutableCommand(enabledHandler, command.Value));
                }
            }
        }

        public async ValueTask PublishCommands(Snowflake appId)
        {
            // This part is purely to ensure that Discord's default "Launch Activity" command isn't there.
            // We keep pure control over how to launch the config activity this way.
            var commands = await Rest.GetGlobalApplicationCommandsAsync(appId);
            if(commands.Success)
            {
                foreach(var command in commands.Value)
                {
                    if(command.Name.ToLower() == "launch" && command.Handler == 2)
                    {
                        // This is a discord_launch_activity command, delete it.
                        await Rest.DeleteGlobalApplicationCommandAsync(appId, command.Id);
                        break; // Only one command of this type can exist, so we can break after finding it.
                    }
                }
            }
            await Rest.BulkOverwriteGlobalApplicationCommandsAsync(appId, [.. Commands]);
        }

        public void Start(Gateway gateway)
        {
            gateway.RegisterSubscriber(typeof(InteractionEventHandler));
        }

        internal async ValueTask HandleInteractionAsync(Gateway gateway, InteractionCreate interactionCreate)
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

                    using (var context = new SlashCommandContext(interactionCreate, Rest, gateway, options, Services))
                    {
                        await CommandHandlers[qualifiedName].Execute(context);
                    }
                }
            }
        }
    }
}
