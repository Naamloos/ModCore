﻿using Microsoft.Extensions.DependencyInjection;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.InteractionFramework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModCore.Common.InteractionFramework
{
    public abstract class BaseCommandHandler
    {
        private Dictionary<Type, object> classInstances = new();

        public BaseCommandHandler()
        {
            classInstances.Add(this.GetType(), this);
        }

        internal (Dictionary<string, Func<SlashCommandContext, Task>>, List<ApplicationCommand>) LoadSlashCommands(IServiceProvider services)
        {
            Dictionary<string, Func<SlashCommandContext, Task>> executables = new();
            List<ApplicationCommand> commands = new();

            if (this.GetType().GetCustomAttribute<SlashCommandAttribute>() is not null)
            {
                return loadAsTopLevel();
            }
            else
            {
                return loadAsContainer();
            }
        }

        private (Dictionary<string, Func<SlashCommandContext, Task>>, List<ApplicationCommand>) loadAsTopLevel()
        {
            var executables = new Dictionary<string, Func<SlashCommandContext, Task>>();

            var attr = this.GetType().GetCustomAttribute<SlashCommandAttribute>();

            if (attr is null)
            {
                throw new Exception();
            }

            var type = this.GetType();

            var subCommands = loadSubCommands(type, type.Name);
            var subGroups = loadSubGroups(type, type.Name);

            var options = new List<ApplicationCommandOption>();
            options.AddRange(subCommands.Item2);
            options.AddRange(subGroups.Item2);

            var command = new ApplicationCommand()
            {
                Name = type.Name.ToLowerInvariant(),
                Description = attr.Description,
                NSFW = attr.Nsfw,
                CanBeUsedInDM = attr.DmPermission,
                Type = ApplicationCommandType.ChatInput,
                Options = options
            };

            foreach (var subCommand in subCommands.Item1)
            {
                executables.Add(subCommand.Key.ToLowerInvariant(), subCommand.Value);
            }

            foreach (var subGroup in subGroups.Item1)
            {
                executables.Add(subGroup.Key.ToLowerInvariant(), subGroup.Value);
            }

            return (executables, new List<ApplicationCommand>() { command });
        }

        private (Dictionary<string, Func<SlashCommandContext, Task>>, List<ApplicationCommand>) loadAsContainer()
        {
            var executables = new Dictionary<string, Func<SlashCommandContext, Task>>();
            var appCommands = new List<ApplicationCommand>();

            var groups = loadGroups(this.GetType());
            var commands = loadCommands(this.GetType());

            appCommands.AddRange(groups.Item2);
            appCommands.AddRange(commands.Item2);

            foreach (var executable in groups.Item1)
            {
                executables.Add(executable.Key.ToLowerInvariant(), executable.Value);
            }
            foreach (var executable in commands.Item1)
            {
                executables.Add(executable.Key.ToLowerInvariant(), executable.Value);
            }

            return (executables, appCommands);
        }

        private (Dictionary<string, Func<SlashCommandContext, Task>>, List<ApplicationCommand>) loadCommands(Type parent)
        {
            var executables = new Dictionary<string, Func<SlashCommandContext, Task>>();
            var appCommands = new List<ApplicationCommand>();

            var methods = parent.GetMethods()
                .Where(x => x.GetCustomAttribute<SlashCommandAttribute>() != null)
                .Where(x => x.GetParameters().Length > 0);

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<SlashCommandAttribute>()!;

                appCommands.Add(new ApplicationCommand()
                {
                    Name = method.Name.ToLowerInvariant(),
                    Description = attr.Description,
                    NSFW = attr.Nsfw,
                    CanBeUsedInDM = attr.DmPermission,
                    Options = loadOptions(method)
                });

                executables.Add(method.Name.ToLowerInvariant(), async context => await ExecuteCommand(context, method, getTypeInstance(this.GetType())));
            }

            return (executables, appCommands);
        }

        private (Dictionary<string, Func<SlashCommandContext, Task>>, List<ApplicationCommand>) loadGroups(Type parent)
        {
            var executables = new Dictionary<string, Func<SlashCommandContext, Task>>();
            var appCommands = new List<ApplicationCommand>();

            var groups = parent.GetNestedTypes()
                .Where(x => x.GetCustomAttribute<SlashCommandAttribute>() is not null);

            foreach (var group in groups)
            {
                var attr = group.GetCustomAttribute<SlashCommandAttribute>()!;

                var subGroups = loadSubGroups(group, group.Name.ToLowerInvariant());
                var subCommands = loadSubCommands(group, group.Name.ToLowerInvariant());
                var options = new List<ApplicationCommandOption>();

                options.AddRange(subGroups.Item2);
                options.AddRange(subCommands.Item2);

                appCommands.Add(new ApplicationCommand()
                {
                    Name = group.Name.ToLowerInvariant(),
                    Description = attr.Description,
                    NSFW = attr.Nsfw,
                    CanBeUsedInDM = attr.DmPermission,
                    Options = options
                });

                foreach (var executable in subGroups.Item1)
                    executables.Add(executable.Key.ToLowerInvariant(), executable.Value);
                foreach (var executable in subCommands.Item1)
                    executables.Add(executable.Key.ToLowerInvariant(), executable.Value);
            }

            return (executables, appCommands);
        }

        private (Dictionary<string, Func<SlashCommandContext, Task>>, List<ApplicationCommandOption>) loadSubCommands(Type parent, string parentName)
        {
            var executables = new Dictionary<string, Func<SlashCommandContext, Task>>();
            var appCommands = new List<ApplicationCommandOption>();

            var methods = parent.GetMethods()
                .Where(x => x.GetCustomAttribute<SlashCommandAttribute>() != null)
                .Where(x => x.GetParameters().Length > 0);

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<SlashCommandAttribute>()!;

                appCommands.Add(new ApplicationCommandOption()
                {
                    Name = method.Name.ToLowerInvariant(),
                    Description = attr.Description,
                    Options = loadOptions(method),
                    Type = ApplicationCommandOptionType.Subcommand
                });

                executables.Add(parentName.ToLowerInvariant() + " " + method.Name.ToLowerInvariant(), context => ExecuteCommand(context, method, getTypeInstance(parent)));
            }

            return (executables, appCommands);
        }

        private (Dictionary<string, Func<SlashCommandContext, Task>>, List<ApplicationCommandOption>) loadSubGroups(Type parent, string parentName)
        {
            var executables = new Dictionary<string, Func<SlashCommandContext, Task>>();
            var appCommands = new List<ApplicationCommandOption>();

            var groups = parent.GetNestedTypes()
                .Where(x => x.GetCustomAttribute<SlashCommandAttribute>() is not null);

            foreach (var group in groups)
            {
                var attr = group.GetCustomAttribute<SlashCommandAttribute>()!;

                var subCommands = loadSubCommands(group, group.Name.ToLowerInvariant());

                appCommands.Add(new ApplicationCommandOption()
                {
                    Name = group.Name.ToLowerInvariant(),
                    Description = attr.Description,
                    Options = subCommands.Item2,
                    Type = ApplicationCommandOptionType.SubcommandGroup
                });

                foreach (var executable in subCommands.Item1)
                    executables.Add(parentName.ToLowerInvariant() + " " + executable.Key.ToLowerInvariant(), executable.Value);
            }

            return (executables, appCommands);
        }

        private object getTypeInstance(Type type)
        {
            if (classInstances.ContainsKey(type))
            {
                return classInstances[type];
            }

            // TODO dependency injection
            var newObject = Activator.CreateInstance(type)!;
            classInstances.Add(type, newObject);
            return newObject;
        }

        private List<ApplicationCommandOption> loadOptions(MethodInfo method)
        {
            var options = new List<ApplicationCommandOption>();

            var parameters = method.GetParameters().Skip(1);
            if (parameters.Any(x => x.GetCustomAttribute<OptionAttribute>() is null))
            {
                throw new Exception();
            }
            foreach (var param in parameters)
            {
                var attr = param.GetCustomAttribute<OptionAttribute>();

                options.Add(new ApplicationCommandOption()
                {
                    Name = param.Name.ToLowerInvariant(),
                    Description = attr.Description,
                    Type = attr.Type,
                    Required = param.ParameterType.IsAssignableFrom(typeof(Optional))
                });
            }
            return options;
        }

        private async Task ExecuteCommand(SlashCommandContext context, MethodInfo method, object instance)
        {
            List<object> parameters = [context];

            foreach (var param in method.GetParameters().Skip(1))
            {
                var option = context.OptionValues.FirstOrDefault(x => x.Name == param.Name);
                if(option is null || !option.Value.HasValue)
                {
                    parameters.Add(null);
                }
                else
                {
                    parameters.Add(JsonSerializer.Deserialize(option.Value.Value, param.ParameterType, options: context.ServiceProvider.GetService<JsonSerializerOptions>())!);
                }
            }

            await (method.Invoke(instance, parameters.ToArray()) as Task)!;
        }
    }
}
