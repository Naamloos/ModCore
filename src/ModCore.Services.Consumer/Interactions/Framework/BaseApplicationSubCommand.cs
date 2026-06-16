using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ModCore.Services.Consumer.Interactions.Framework
{
    public abstract class BaseApplicationSubcommand : BaseCommand, IApplicationSubcommand
    {
        public abstract string ParentName { get; }

        public virtual string? GroupName => null;

        public virtual string? GroupDescription => null;

        public abstract string Name { get; }

        public abstract string Description { get; }

        public ApplicationCommandOption BuildAsSubcommand()
        {
            return new ApplicationCommandOption
            {
                Name = Name,
                Description = Description,
                Type = ApplicationCommandOptionType.Subcommand,
                Options = BuildParameterOptions()
            };
        }

        public Task InvokeAsync(
            Interaction interaction,
            JsonSerializerOptions jsonSerializerOptions)
        {
            return InvokeHandlerAsync(this, interaction, jsonSerializerOptions);
        }
    }
}
