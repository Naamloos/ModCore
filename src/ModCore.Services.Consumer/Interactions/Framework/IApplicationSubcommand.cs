using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using ModCore.Common.Discord.Entities.Interactions;

namespace ModCore.Services.Consumer.Interactions.Framework
{
    public interface IApplicationSubcommand
    {
        string ParentName { get; }

        string? GroupName { get; }

        string? GroupDescription { get; }

        string Name { get; }

        string Description { get; }

        ApplicationCommandOption BuildAsSubcommand();

        Task InvokeAsync(Interaction interaction, JsonSerializerOptions jsonSerializerOptions);
    }
}
