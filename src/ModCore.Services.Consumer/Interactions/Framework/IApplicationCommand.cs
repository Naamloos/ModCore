using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using ModCore.Common.Discord.Entities.Interactions;

namespace ModCore.Services.Consumer.Interactions.Framework
{
    public interface IApplicationCommand
    {
        string Name { get; }

        ApplicationCommand BuildAsCommand();

        Task InvokeAsync(Interaction interaction, JsonSerializerOptions jsonSerializerOptions);
    }
}
