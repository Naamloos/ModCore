using Microsoft.Extensions.Logging;
using ModCore.Common.Database;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.InteractionFramework;
using ModCore.Common.InteractionFramework.Attributes;
using ModCore.Common.Utils;
using ModCore.Common.Xaml;
using ModCore.Services.Shard.Modules.About.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModCore.Services.Shard.Modules.About
{
    public class AboutCommands : BaseCommandHandler
    {
        private readonly ILogger _logger;
        private readonly DiscordXaml _xamlCompiler;

        public AboutCommands(ILogger<AboutCommands> logger, DiscordXaml xamlCompiler)
        {
            _logger = logger;
            _xamlCompiler = xamlCompiler;
        }

        private const string ABOUT_VIEW_XAML = "ModCore.Services.Shard.Modules.About.Views.AboutView.xaml";
        [SlashCommand("about", "Shows information about this bot.", dm_permission: true)]
        public async ValueTask AboutAsync(SlashCommandContext context)
        {
            var data = context.EventData;

            // Fetch current user info to grab avatar
            var modcoreSelf = await context.RestClient.GetCurrentUserAsync();
            if (!modcoreSelf.Success)
                return;

            // Create viewmodel and compile XAML with Bindings
            var viewModel = new AboutViewModel(modcoreSelf.Value);
            var compiledComponentList = await _xamlCompiler.CompileXamlAsync(ABOUT_VIEW_XAML, viewModel);

            await context.RestClient.CreateInteractionResponseAsync(data.Id, data.Token, InteractionResponseType.ChannelMessageWithSource,
                new InteractionMessageResponse()
                {
                    Flags = MessageFlags.ComponentsV2 | MessageFlags.Ephemeral,
                    Components = compiledComponentList.ToArray()
                });
        }
    }
}
