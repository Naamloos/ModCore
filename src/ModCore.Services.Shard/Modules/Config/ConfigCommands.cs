using Microsoft.Extensions.Logging;
using ModCore.Common.Database;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.InteractionFramework;
using ModCore.Common.InteractionFramework.Attributes;
using ModCore.Common.Language;
using ModCore.Common.Utils;
using ModCore.Common.Views;
using ModCore.Common.Xaml;
using ModCore.Services.Shard.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModCore.Services.Shard.Modules.About
{
    public class ConfigCommands : BaseCommandHandler
    {
        public ConfigCommands() { }

        [SlashCommand("config", "Launches the ModCore Configuration Utility.", dm_permission: false)]
        public async ValueTask ConfigAsync(SlashCommandContext context)
        {
            var data = context.EventData;

            await context.RestClient.CreateInteractionResponseAsync(data.Id, data.Token, InteractionResponseType.LaunchActivity,
                new InteractionMessageResponse());
        }
    }
}
