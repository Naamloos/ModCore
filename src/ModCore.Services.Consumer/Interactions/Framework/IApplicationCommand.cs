using ModCore.Common.Discord.Entities.Interactions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ModCore.Services.Consumer.Interactions.Framework
{
    public interface IApplicationCommand
    {
        string Name { get; }

        ApplicationCommand BuildAsCommand();

        Task InvokeAsync(
            Interaction interaction,
            JsonSerializerOptions jsonSerializerOptions);
    }
}
