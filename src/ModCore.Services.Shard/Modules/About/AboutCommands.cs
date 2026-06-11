using Microsoft.Extensions.Logging;
using ModCore.Common.Cache;
using ModCore.Common.Database;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Components;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Entities.Utils;
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
    public class AboutCommands : BaseCommandHandler
    {
        private readonly ILogger _logger;
        private readonly I18n _i18n;
        private readonly CacheService _cache;

        public AboutCommands(ILogger<AboutCommands> logger, I18n i18n, CacheService cache)
        {
            _logger = logger;
            _i18n = i18n;
            _cache = cache;
        }

        // TODO figure out command/argument i18n
        [SlashCommand("about", "Shows information about this bot.", dm_permission: true)]
        public async ValueTask AboutAsync(SlashCommandContext context)
        {
            var data = context.EventData;

            User? modcoreSelf;

            var modcoreSelfCached = _cache.TryGet<User, string>("@me");
            if (modcoreSelfCached.Success)
            {
                modcoreSelf = modcoreSelfCached.Value;
            }
            else
            {
                var modcoreSelfRest = await context.RestClient.GetCurrentUserAsync();
                if (!modcoreSelfRest.Success)
                {
                    return; // TODO fallback when we can't get self user, maybe just don't show the avatar?
                }
                modcoreSelf = modcoreSelfRest.Value;
                await _cache.UpdateAsync<User, string>("@me", modcoreSelf);
            }

            var message = new MessageBuilder()
                .AddContainer(container =>
                {
                    container.AddSection(section =>
                    {
                        section.AddText(this._i18n.t("about.welcome"));
                        section.AddText(this._i18n.t("about.main_developer"));
                        section.AddText(this._i18n.t("about.contribute"));
                    }, new Thumbnail()
                    {
                        Media = new UnfurledMediaItem()
                        {
                            Url = $"https://cdn.discordapp.com/avatars/{modcoreSelf!.Id}/{modcoreSelf!.AvatarHash}.png"
                        }
                    })
                    .AddText(this._i18n.t("about.donate"))
                    .AddText(this._i18n.t("about.previous_contribs", data: new()
                        {
                            { "contribs", string.Join(", ", previousContribList.Select(x => $"[{x.Key}]({x.Value})")) }
                        }))
                    .AddText("-# ModCore v3 ALPHA")
                    .WithAccentColor(0x5865F2);
                })
                .WithFlags(MessageFlags.ComponentsV2 | MessageFlags.Ephemeral)
                .BuildInteractionResponse();

            await context.RestClient.CreateInteractionResponseAsync(
                data.Id,
                data.Token,
                InteractionResponseType.ChannelMessageWithSource,
                message
            );
        }

        private static Dictionary<string, string> previousContribList = new()
        {
            { "uwx", "https://github.com/uwx" },
            { "jcryer", "https://github.com/jcryer" },
            { "Emzi0767", "https://github.com/Emzi0767" },
            { "YourAverageBlackGuy", "https://github.com/YourAverageBlackGuy" },
            { "DrCreo", "https://github.com/DrCreo" },
            { "aexolate", "https://github.com/aexolate" },
            { "Drake103", "https://github.com/Drake103" },
            { "Izumemori", "https://github.com/Izumemori" },
            { "OoLunar", "https://github.com/OoLunar" },
            { "InFTord", "https://github.com/InFTord" }
        };
    }
}
