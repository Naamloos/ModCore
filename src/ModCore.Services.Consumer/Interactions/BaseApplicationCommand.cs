using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Messages;
using Npgsql.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ModCore.Services.Consumer.Interactions
{
    public interface IApplicationCommand
    {
        public string Name { get; }
        public ApplicationCommand BuildAsCommand();
        public ApplicationCommandOption BuildAsSubcommand();
        internal Task InvokeAsync(Interaction interaction, JsonSerializerOptions jsonSerializerOptions);
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ApplicationCommandHandlerAttribute : Attribute
    {
        public ApplicationCommandHandlerAttribute() { }
    }

    public abstract class BaseApplicationCommand : IApplicationCommand
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public virtual ApplicationCommandType Type { get => ApplicationCommandType.ChatInput; }
        public virtual Permissions DefaultMemberPermissions { get => Permissions.None; }
        public virtual bool NSFW { get => false; }

        public ApplicationCommand BuildAsCommand()
        {
            return new ApplicationCommand()
            {
                Name = Name,
                Description = Description,
                Type = Type,
                DefaultMemberPermissions = DefaultMemberPermissions,
                NSFW = NSFW,
                Handler = Type == ApplicationCommandType.ActivityEntryPoint ? 1 : Optional.None,
                Options = Type == ApplicationCommandType.ChatInput ? BuildOptions() : Optional.None,
            };
        }

        public ApplicationCommandOption BuildAsSubcommand()
        {
            return new ApplicationCommandOption()
            {
                Name = Name,
                Description = Description,
                Type = ApplicationCommandOptionType.Subcommand,
            };
        }

        private List<ApplicationCommandOption> BuildOptions()
        {
            var handler = findHandler();
            var parameters = handler.GetParameters().Skip(1);

            var options = new List<ApplicationCommandOption>();

            foreach (var parameter in parameters)
            {
                var description = parameter.GetCustomAttribute<DescriptionAttribute>()?.Description
                    ?? "No description provided";

                var option = new ApplicationCommandOption()
                {
                    Name = ToDiscordOptionName(parameter.Name!),
                    Description = description,
                };

                switch (parameter.ParameterType)
                {
                    case Type t when t == typeof(string):
                        option.Type = ApplicationCommandOptionType.String;
                        break;

                    case Type t when t == typeof(int):
                    case Type t2 when t2 == typeof(long):
                        option.Type = ApplicationCommandOptionType.Integer;
                        break;

                    case Type t when t == typeof(bool):
                        option.Type = ApplicationCommandOptionType.Boolean;
                        break;

                    case Type t when t == typeof(double):
                        option.Type = ApplicationCommandOptionType.Number;
                        break;

                    case Type t when t == typeof(User):
                    case Type t2 when t2 == typeof(Member):
                        option.Type = ApplicationCommandOptionType.User;
                        break;

                    case Type t when t == typeof(Channel):
                        option.Type = ApplicationCommandOptionType.Channel;
                        break;

                    case Type t when t == typeof(Role):
                        option.Type = ApplicationCommandOptionType.Role;
                        break;

                    case Type t when t == typeof(Attachment):
                        option.Type = ApplicationCommandOptionType.Attachment;
                        break;

                    default:
                        throw new NotImplementedException(string.Format(
                            "Parameter {0} has an unimplemented data type: {1}! in {2}",
                            parameter.Name,
                            parameter.ParameterType.FullName,
                            this.GetType().FullName
                        ));
                }

                option.Required = !(parameter.HasDefaultValue || parameter.IsOptional);
                options.Add(option);
            }

            return options;
        }

        public async Task InvokeAsync(Interaction interaction, JsonSerializerOptions jsonSerializerOptions)
        {
            var handler = findHandler();
            var handlerParameters = handler.GetParameters();

            if (!interaction.Data.HasValue)
            {
                throw new InvalidOperationException(
                    $"Interaction does not contain command data for handler '{handler.Name}'.");
            }

            var interactionData = interaction.Data.Value;

            var interactionOptions = interactionData.Options.HasValue
                ? FlattenOptions(interactionData.Options.Value)
                    .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, ApplicationCommandInteractionDataOption>(
                    StringComparer.OrdinalIgnoreCase);

            object?[] parameters = new object?[handlerParameters.Length];

            parameters[0] = interaction;

            for (var i = 1; i < handlerParameters.Length; i++)
            {
                var parameter = handlerParameters[i];
                var optionName = ToDiscordOptionName(parameter.Name!);

                if (!interactionOptions.TryGetValue(optionName, out var option))
                {
                    if (parameter.HasDefaultValue || parameter.IsOptional)
                    {
                        parameters[i] = parameter.DefaultValue;
                        continue;
                    }

                    throw new InvalidOperationException(
                        $"Missing required option '{optionName}' for command '{interactionData.Name}'.");
                }

                parameters[i] = ConvertOptionValue(
                    interaction,
                    option,
                    parameter.ParameterType,
                    jsonSerializerOptions);
            }

            var result = handler.Invoke(this, parameters);

            if (result is Task task)
            {
                await task;
            }
        }

        private static IEnumerable<ApplicationCommandInteractionDataOption> FlattenOptions(
            IEnumerable<ApplicationCommandInteractionDataOption> options)
        {
            foreach (var option in options)
            {
                if (option.Options.HasValue && option.Options.Value.Count > 0)
                {
                    foreach (var child in FlattenOptions(option.Options.Value))
                    {
                        yield return child;
                    }
                }
                else
                {
                    yield return option;
                }
            }
        }

        private object? ConvertOptionValue(
    Interaction interaction,
    ApplicationCommandInteractionDataOption option,
    Type targetType,
    JsonSerializerOptions jsonSerializerOptions)
        {
            if (!option.Value.HasValue)
            {
                return null;
            }

            JsonNode rawValue = option.Value.Value;

            return targetType switch
            {
                Type t when t == typeof(string) =>
                    DeserializeOptionValue<string>(rawValue, jsonSerializerOptions),

                Type t when t == typeof(int) =>
                    DeserializeOptionValue<int>(rawValue, jsonSerializerOptions),

                Type t when t == typeof(long) =>
                    DeserializeOptionValue<long>(rawValue, jsonSerializerOptions),

                Type t when t == typeof(bool) =>
                    DeserializeOptionValue<bool>(rawValue, jsonSerializerOptions),

                Type t when t == typeof(double) =>
                    DeserializeOptionValue<double>(rawValue, jsonSerializerOptions),

                Type t when t == typeof(Snowflake) =>
                    DeserializeOptionValue<Snowflake>(rawValue, jsonSerializerOptions),

                Type t when t == typeof(User) =>
                    ResolveUser(interaction, DeserializeOptionValue<Snowflake>(rawValue, jsonSerializerOptions)),

                Type t when t == typeof(Member) =>
                    ResolveMember(interaction, DeserializeOptionValue<Snowflake>(rawValue, jsonSerializerOptions)),

                Type t when t == typeof(Channel) =>
                    ResolveChannel(interaction, DeserializeOptionValue<Snowflake>(rawValue, jsonSerializerOptions)),

                Type t when t == typeof(Role) =>
                    ResolveRole(interaction, DeserializeOptionValue<Snowflake>(rawValue, jsonSerializerOptions)),

                Type t when t == typeof(Attachment) =>
                    ResolveAttachment(interaction, DeserializeOptionValue<Snowflake>(rawValue, jsonSerializerOptions)),

                _ => throw new NotImplementedException(
                    $"Cannot convert option '{option.Name}' to {targetType.FullName}.")
            };
        }

        private MethodInfo findHandler()
        {
            var handlers = this.GetType().GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(ApplicationCommandHandlerAttribute), false).Length > 0);
            if (handlers.Count() != 1)
            {
                throw new ArgumentException("Exactly one method must be marked with the ApplicationCommandHandlerAttribute.");
            }

            var handler = handlers.First();
            var parameters = handler.GetParameters();
            if (parameters.Length < 1 || parameters[0].ParameterType != typeof(Interaction))
            {
                throw new ArgumentException("The method marked with the ApplicationCommandHandlerAttribute must have at least one parameter of type Interaction.");
            }

            return handler;
        }

        private static string ToDiscordOptionName(string name)
        {
            var result = new StringBuilder();

            foreach (var c in name)
            {
                if (char.IsLetterOrDigit(c))
                {
                    result.Append(char.ToLowerInvariant(c));
                }
                else if (c == '_' || c == '-')
                {
                    result.Append(c);
                }
            }

            var final = result.ToString().Trim('_', '-');

            if (string.IsNullOrWhiteSpace(final))
            {
                throw new InvalidOperationException(
                    $"Parameter name '{name}' cannot be converted to a valid Discord option name.");
            }

            if (final.Length > 32)
            {
                final = final[..32].Trim('_', '-');
            }

            return final;
        }

        private static ResolvedDataStructure GetResolvedData(Interaction interaction)
        {
            if (!interaction.Data.HasValue)
            {
                throw new InvalidOperationException("Interaction data is missing.");
            }

            var data = interaction.Data.Value;

            if (!data.Resolved.HasValue)
            {
                throw new InvalidOperationException("Interaction resolved data is missing.");
            }

            return data.Resolved.Value;
        }

        private static User ResolveUser(Interaction interaction, Snowflake id)
        {
            var resolved = GetResolvedData(interaction);

            if (resolved.Users.HasValue &&
                resolved.Users.Value.TryGetValue(id, out var user))
            {
                return user;
            }

            throw new InvalidOperationException($"Could not resolve user '{id}'.");
        }

        private static Member ResolveMember(Interaction interaction, Snowflake id)
        {
            var resolved = GetResolvedData(interaction);

            if (resolved.Members.HasValue &&
                resolved.Members.Value.TryGetValue(id, out var member))
            {
                return member;
            }

            throw new InvalidOperationException($"Could not resolve member '{id}'.");
        }

        private static Channel ResolveChannel(Interaction interaction, Snowflake id)
        {
            var resolved = GetResolvedData(interaction);

            if (resolved.Channels.HasValue &&
                resolved.Channels.Value.TryGetValue(id, out var channel))
            {
                return channel;
            }

            throw new InvalidOperationException($"Could not resolve channel '{id}'.");
        }

        private static Role ResolveRole(Interaction interaction, Snowflake id)
        {
            var resolved = GetResolvedData(interaction);

            if (resolved.Roles.HasValue &&
                resolved.Roles.Value.TryGetValue(id, out var role))
            {
                return role;
            }

            throw new InvalidOperationException($"Could not resolve role '{id}'.");
        }

        private static Attachment ResolveAttachment(Interaction interaction, Snowflake id)
        {
            var resolved = GetResolvedData(interaction);

            if (resolved.Attachments.HasValue &&
                resolved.Attachments.Value.TryGetValue(id, out var attachment))
            {
                return attachment;
            }

            throw new InvalidOperationException($"Could not resolve attachment '{id}'.");
        }

        private static T DeserializeOptionValue<T>(
            JsonNode rawValue,
            JsonSerializerOptions jsonSerializerOptions)
        {
            var value = rawValue.Deserialize<T>(jsonSerializerOptions);

            if (value is null)
            {
                throw new InvalidOperationException(
                    $"Could not deserialize option value to {typeof(T).FullName}.");
            }

            return value;
        }
    }
}
