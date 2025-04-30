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
        private readonly I18n _i18n;

        public AboutCommands(ILogger<AboutCommands> logger, DiscordXaml xamlCompiler, I18n i18n)
        {
            _logger = logger;
            _xamlCompiler = xamlCompiler;
            _i18n = i18n;
        }

        // TODO figure out command/argument i18n
        private const string ABOUT_VIEW_XAML = "ModCore.Services.Shard.Modules.About.Views.AboutView.xaml";
        [SlashCommand("about", "Shows information about this bot.", dm_permission: true)]
        public async ValueTask AboutAsync(SlashCommandContext context,
            [Option("language", "Language to receive about info in", ApplicationCommandOptionType.String)] Optional<string> language)
        {
            var data = context.EventData;

            // Fetch current user info to grab avatar
            var modcoreSelf = await context.RestClient.GetCurrentUserAsync();
            if (!modcoreSelf.Success)
                return;

            // Create viewmodel and compile XAML with Bindings
            var viewModel = new AboutViewModel(modcoreSelf.Value, _i18n, language.HasValue? language : "en");
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
