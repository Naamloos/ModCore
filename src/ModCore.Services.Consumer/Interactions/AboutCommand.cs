using Microsoft.EntityFrameworkCore;
using ModCore.Common.Cache;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Components;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Utils;
using ModCore.Common.Discord.Rest;
using ModCore.Common.Language;
using ModCore.Services.Consumer.Interactions.Framework;

namespace ModCore.Services.Consumer.Interactions
{
    public class AboutCommand : BaseApplicationCommand
    {
        public override string Name => "about";

        public override string Description => "Information about ModCore";

        private DiscordRest _rest;
        private CacheService _cache;
        private IModCoreLocalizerFactory _localizerFactory;

        public AboutCommand(DiscordRest rest, CacheService cache, IModCoreLocalizerFactory localizerFactory)
        {
            _rest = rest;
            _cache = cache;
            _localizerFactory = localizerFactory;
        }

        [ApplicationCommandHandler]
        public async Task HandleAsync(Interaction interaction)
        {
            User? modcoreSelf;

            var modcoreSelfCached = _cache.TryGet<User, string>("@me");
            if (modcoreSelfCached.Success)
            {
                modcoreSelf = modcoreSelfCached.Value;
            }
            else
            {
                var modcoreSelfRest = await _rest.GetCurrentUserAsync();
                if (!modcoreSelfRest.Success)
                {
                    return; // TODO fallback when we can't get self user, maybe just don't show the avatar?
                }
                modcoreSelf = modcoreSelfRest.Value;
                await _cache.UpdateAsync<User, string>("@me", modcoreSelf);
            }

            var t = _localizerFactory.Get(interaction.Locale.Value);
            var contribs = string.Join(", ", previousContribList.Select(x => $"[{x.Key}]({x.Value})"));

            var message = new MessageBuilder()
                .AddContainer(container =>
                {
                    container.AddSection(section =>
                    {
                        section.AddText(t["aboutWelcome"]);
                        section.AddText(t["aboutMainDeveloper"]);
                        section.AddText(t["aboutContribute"]);
                    }, new Thumbnail()
                    {
                        Media = new UnfurledMediaItem()
                        {
                            Url = $"https://cdn.discordapp.com/avatars/{modcoreSelf!.Id}/{modcoreSelf!.AvatarHash}.png"
                        }
                    })
                    .AddText(t["aboutDonate"])
                    .AddText(t["aboutPreviousContribs", new { contribs }])
                    .AddText("-# ModCore v3 ALPHA")
                    .WithAccentColor(0x5865F2);
                })
                .WithFlags(MessageFlags.ComponentsV2 | MessageFlags.Ephemeral)
                .BuildInteractionResponse();

            await _rest.CreateInteractionResponseAsync(
                interaction.Id,
                interaction.Token,
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
