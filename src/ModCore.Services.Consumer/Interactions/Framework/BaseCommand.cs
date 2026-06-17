using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Channels;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Guilds;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ModCore.Services.Consumer.Interactions.Framework
{
    public abstract class BaseCommand
    {
        protected List<ApplicationCommandOption> BuildParameterOptions()
        {
            var handler = TryFindHandler();

            if (handler is null)
            {
                return [];
            }

            var parameters = handler.GetParameters().Skip(1);
            var options = new List<ApplicationCommandOption>();

            foreach (var parameter in parameters)
            {
                var name = ToDiscordOptionName(parameter.GetCustomAttribute<NameAttribute>()?.Name ?? parameter.Name!);
                var description = parameter.GetCustomAttribute<DescriptionAttribute>()?.Description
                    ?? "No description provided";
                var type = ToDiscordOptionType(parameter.ParameterType);
                var underlyingType = Nullable.GetUnderlyingType(parameter.ParameterType) ?? parameter.ParameterType;
                int? minimum = parameter.GetCustomAttribute<MinAttribute>()?.Min;
                int? maximum = parameter.GetCustomAttribute<MaxAttribute>()?.Max;
                ChannelType[]? channelTypes = parameter.GetCustomAttribute<AllowedChannelTypesAttribute>()?.AllowedTypes;

                var option = new ApplicationCommandOption
                {
                    Name = name,
                    Description = description,
                    Type = type,
                    Required = !(parameter.HasDefaultValue || parameter.IsOptional),
                    Choices = underlyingType.IsEnum
                        ? BuildEnumChoices(underlyingType)
                        : Optional.None,
                    MinValue = minimum != null && (type == ApplicationCommandOptionType.Integer || type == ApplicationCommandOptionType.Number)
                        ? minimum.Value
                        : Optional.None,
                    MaxValue = maximum != null && (type == ApplicationCommandOptionType.Integer || type == ApplicationCommandOptionType.Number)
                        ? maximum.Value
                        : Optional.None,
                    MaxLength = maximum != null && type == ApplicationCommandOptionType.String
                        ? maximum.Value
                        : Optional.None,
                    MinLength = minimum != null && type == ApplicationCommandOptionType.String
                        ? minimum.Value
                        : Optional.None,
                    ChannelTypes = channelTypes != null && type == ApplicationCommandOptionType.Channel
                        ? channelTypes
                        : Optional.None
                };

                options.Add(option);
            }

            return options;
        }

        protected async Task InvokeHandlerAsync(
            object instance,
            Interaction interaction,
            JsonSerializerOptions jsonSerializerOptions)
        {
            var handler = TryFindHandler();

            if (handler is null)
            {
                throw new InvalidOperationException(
                    $"Command '{GetType().FullName}' does not have a handler.");
            }

            var handlerParameters = handler.GetParameters();

            if (!interaction.Data.HasValue)
            {
                throw new InvalidOperationException(
                    $"Interaction does not contain command data for handler '{handler.Name}'.");
            }

            var interactionData = interaction.Data.Value;

            var interactionOptions = interactionData.Options.HasValue
                ? FlattenOptions(interactionData.Options.Value)
                    .Where(x => x.Value.HasValue)
                    .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, ApplicationCommandInteractionDataOption>(
                    StringComparer.OrdinalIgnoreCase);

            object?[] parameters = new object?[handlerParameters.Length];
            parameters[0] = interaction;

            for (var i = 1; i < handlerParameters.Length; i++)
            {
                var parameter = handlerParameters[i];
                var optionName = ToDiscordOptionName(parameter.GetCustomAttribute<NameAttribute>()?.Name ?? parameter.Name!);

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

            var result = handler.Invoke(instance, parameters);

            if (result is Task task)
            {
                await task;
            }
        }

        protected MethodInfo? TryFindHandler()
        {
            var handlers = GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<ApplicationCommandHandlerAttribute>() is not null)
                .ToArray();

            if (handlers.Length == 0)
            {
                return null;
            }

            if (handlers.Length != 1)
            {
                throw new ArgumentException(
                    $"Exactly one method must be marked with {nameof(ApplicationCommandHandlerAttribute)} in {GetType().FullName}.");
            }

            var handler = handlers[0];
            var parameters = handler.GetParameters();

            if (parameters.Length < 1 || parameters[0].ParameterType != typeof(Interaction))
            {
                throw new ArgumentException(
                    $"The method marked with {nameof(ApplicationCommandHandlerAttribute)} must have Interaction as its first parameter.");
            }

            return handler;
        }

        protected bool HasHandler()
        {
            return TryFindHandler() is not null;
        }

        protected static string ToDiscordOptionName(string name)
        {
            var result = new StringBuilder();

            for (var i = 0; i < name.Length; i++)
            {
                var c = name[i];

                if (char.IsLetterOrDigit(c))
                {
                    if (ShouldInsertSeparatorBefore(name, i, result))
                    {
                        result.Append('_');
                    }

                    result.Append(char.ToLowerInvariant(c));
                }
                else if (c == '_' || c == '-')
                {
                    AppendSeparator(result, c);
                }
                else
                {
                    AppendSeparator(result, '_');
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

            if (string.IsNullOrWhiteSpace(final))
            {
                throw new InvalidOperationException(
                    $"Parameter name '{name}' cannot be converted to a valid Discord option name.");
            }

            return final;
        }

        private static bool ShouldInsertSeparatorBefore(
            string name,
            int index,
            StringBuilder result)
        {
            var c = name[index];

            if (!char.IsUpper(c) || result.Length == 0)
            {
                return false;
            }

            var previousResult = result[result.Length - 1];

            if (previousResult == '_' || previousResult == '-')
            {
                return false;
            }

            var previous = name[index - 1];

            if (previous == '_' || previous == '-')
            {
                return false;
            }

            if (char.IsLower(previous) || char.IsDigit(previous))
            {
                return true;
            }

            return char.IsUpper(previous) &&
                index + 1 < name.Length &&
                char.IsLower(name[index + 1]);
        }

        private static void AppendSeparator(StringBuilder result, char separator)
        {
            if (result.Length == 0)
            {
                return;
            }

            var previous = result[result.Length - 1];

            if (previous == '_' || previous == '-')
            {
                return;
            }

            result.Append(separator);
        }

        private static ApplicationCommandOptionType ToDiscordOptionType(Type parameterType)
        {
            var underlyingType = Nullable.GetUnderlyingType(parameterType) ?? parameterType;

            if (underlyingType.IsEnum)
            {
                return ApplicationCommandOptionType.String;
            }

            if (underlyingType == typeof(string))
            {
                return ApplicationCommandOptionType.String;
            }

            if (underlyingType == typeof(int) || underlyingType == typeof(long))
            {
                return ApplicationCommandOptionType.Integer;
            }

            if (underlyingType == typeof(bool))
            {
                return ApplicationCommandOptionType.Boolean;
            }

            if (underlyingType == typeof(double) || underlyingType == typeof(float) || underlyingType == typeof(decimal))
            {
                return ApplicationCommandOptionType.Number;
            }

            if (underlyingType == typeof(User) || underlyingType == typeof(Member))
            {
                return ApplicationCommandOptionType.User;
            }

            if (underlyingType == typeof(Channel))
            {
                return ApplicationCommandOptionType.Channel;
            }

            if (underlyingType == typeof(Role))
            {
                return ApplicationCommandOptionType.Role;
            }

            if (underlyingType == typeof(Mentionable))
            {
                return ApplicationCommandOptionType.Mentionable;
            }

            if (underlyingType == typeof(Attachment))
            {
                return ApplicationCommandOptionType.Attachment;
            }

            throw new NotImplementedException(
                $"Parameter type '{parameterType.FullName}' is not implemented.");
        }

        private static List<ApplicationCommandOptionChoice> BuildEnumChoices(Type enumType)
        {
            return Enum.GetValues(enumType)
                .Cast<object>()
                .Select(value =>
                {
                    var enumName = Enum.GetName(enumType, value);

                    if (enumName is null)
                    {
                        throw new InvalidOperationException(
                            $"Could not get enum name for value '{value}' in enum '{enumType.FullName}'.");
                    }

                    var field = enumType.GetField(enumName);
                    var name = field?.GetCustomAttribute<NameAttribute>()?.Name ?? enumName;

                    return new ApplicationCommandOptionChoice
                    {
                        Name = name,
                        Value = enumName
                    };
                })
                .ToList();
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

        private static object? ConvertOptionValue(
            Interaction interaction,
            ApplicationCommandInteractionDataOption option,
            Type targetType,
            JsonSerializerOptions jsonSerializerOptions)
        {
            if (!option.Value.HasValue)
            {
                return null;
            }

            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;
            JsonNode rawValue = option.Value.Value;

            if (underlyingType.IsEnum)
            {
                var value = DeserializeOptionValue<string>(rawValue, jsonSerializerOptions);

                if (Enum.TryParse(underlyingType, value, true, out var enumValue))
                {
                    return enumValue;
                }

                throw new InvalidOperationException(
                    $"Cannot convert option '{option.Name}' to {targetType.FullName}.");
            }

            if (underlyingType == typeof(string))
            {
                return DeserializeOptionValue<string>(rawValue, jsonSerializerOptions);
            }

            if (underlyingType == typeof(int))
            {
                return DeserializeOptionValue<int>(rawValue, jsonSerializerOptions);
            }

            if (underlyingType == typeof(long))
            {
                return DeserializeOptionValue<long>(rawValue, jsonSerializerOptions);
            }

            if (underlyingType == typeof(bool))
            {
                return DeserializeOptionValue<bool>(rawValue, jsonSerializerOptions);
            }

            if (underlyingType == typeof(double))
            {
                return DeserializeOptionValue<double>(rawValue, jsonSerializerOptions);
            }

            if (underlyingType == typeof(float))
            {
                return DeserializeOptionValue<float>(rawValue, jsonSerializerOptions);
            }

            if (underlyingType == typeof(decimal))
            {
                return DeserializeOptionValue<decimal>(rawValue, jsonSerializerOptions);
            }

            if (underlyingType == typeof(Snowflake))
            {
                return DeserializeOptionValue<Snowflake>(rawValue, jsonSerializerOptions);
            }

            if (underlyingType == typeof(User))
            {
                return ResolveUser(
                    interaction,
                    DeserializeOptionValue<Snowflake>(rawValue, jsonSerializerOptions));
            }

            if (underlyingType == typeof(Member))
            {
                return ResolveMember(
                    interaction,
                    DeserializeOptionValue<Snowflake>(rawValue, jsonSerializerOptions));
            }

            if (underlyingType == typeof(Channel))
            {
                return ResolveChannel(
                    interaction,
                    DeserializeOptionValue<Snowflake>(rawValue, jsonSerializerOptions));
            }

            if (underlyingType == typeof(Role))
            {
                return ResolveRole(
                    interaction,
                    DeserializeOptionValue<Snowflake>(rawValue, jsonSerializerOptions));
            }

            if (underlyingType == typeof(Mentionable))
            {
                return ResolveMentionable(
                    interaction,
                    DeserializeOptionValue<Snowflake>(rawValue, jsonSerializerOptions));
            }

            if (underlyingType == typeof(Attachment))
            {
                return ResolveAttachment(
                    interaction,
                    DeserializeOptionValue<Snowflake>(rawValue, jsonSerializerOptions));
            }

            throw new NotImplementedException(
                $"Cannot convert option '{option.Name}' to {targetType.FullName}.");
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

        private static Mentionable ResolveMentionable(Interaction interaction, Snowflake id)
        {
            var resolved = GetResolvedData(interaction);

            if (resolved.Users.HasValue &&
                resolved.Users.Value.TryGetValue(id, out var user))
            {
                Member? member = null;

                if (resolved.Members.HasValue)
                {
                    resolved.Members.Value.TryGetValue(id, out member);
                }

                return Mentionable.FromUser(id, user, member);
            }

            if (resolved.Roles.HasValue &&
                resolved.Roles.Value.TryGetValue(id, out var role))
            {
                return Mentionable.FromRole(id, role);
            }

            throw new InvalidOperationException($"Could not resolve mentionable '{id}'.");
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