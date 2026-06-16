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

namespace ModCore.Services.Consumer.Interactions.Framework
{
    public abstract class BaseApplicationCommand : BaseCommand, IApplicationCommand
    {
        public abstract string Name { get; }

        public abstract string Description { get; }

        public virtual ApplicationCommandType Type => ApplicationCommandType.ChatInput;

        public virtual Permissions DefaultMemberPermissions => Permissions.None;

        public virtual bool NSFW => false;

        public virtual ApplicationCommand BuildAsCommand()
        {
            return new ApplicationCommand
            {
                Name = Name,
                Description = Description,
                Type = Type,
                DefaultMemberPermissions = DefaultMemberPermissions,
                NSFW = NSFW,
                Handler = Type == ApplicationCommandType.ActivityEntryPoint ? 1 : Optional.None,
                Options = Type == ApplicationCommandType.ChatInput
                    ? BuildParameterOptions()
                    : Optional.None,
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