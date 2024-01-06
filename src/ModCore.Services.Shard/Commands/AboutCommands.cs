using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.InteractionFramework;
using ModCore.Common.InteractionFramework.Attributes;
using ModCore.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Shard.Commands
{
    public class AboutCommands : BaseCommandHandler
    {
        private readonly ILogger _logger;

        public AboutCommands(ILogger<AboutCommands> logger)
        {
            _logger = logger;
        }

        [SlashCommand("Shows information about this bot.", dm_permission: true)]
        public async ValueTask About(SlashCommandContext context)
        {
            if (context.EventData.Member.HasValue)
            {
                _logger.LogInformation(context.EventData.Member.Value.User.Value.Username + " ran about!");
            }
            else if(context.EventData.User.HasValue)
            {
                _logger.LogInformation(context.EventData.User.Value.Username + " ran about!");
            }

            var data = context.EventData;
            var modcoreSelf = await context.RestClient.GetCurrentUserAsync();
            if (!modcoreSelf.Success)
                return;
            var avatar = $"https://cdn.discordapp.com/avatars/{modcoreSelf.Value.Id}/{modcoreSelf.Value.AvatarHash}.png";
            var resp = await context.RestClient.CreateInteractionResponseAsync(data.Id, data.Token, InteractionResponseType.ChannelMessageWithSource, new InteractionMessageResponse()
            {
                Content = $"Welcome to ModCore v3. " +
                        $"This is an early ALPHA version of ModCore, not yet available to the general public.",
                Flags = MessageFlags.Ephemeral,
                Embeds = new[]
                    {
                        new Embed()
                            .WithDescription($"ModCore is a Discord bot focused on moderation and server management, written from scratch in C#.")
                            .WithColor(ColorConverter.FromHex("#089FDF"))
                            .WithAuthor("ModCore", "https://github.com/Naamloos/ModCore", avatar)
                            .WithThumbnail(avatar)
                            .WithField("Main Developer", "[Naamloos](https://github.com/Naamloos)")
                            .WithField("Special thanks to all of these wonderful contributors:",
                                "[uwx](https://github.com/uwx), " +
                                "[jcryer](https://github.com/jcryer), " +
                                "[Emzi0767](https://github.com/Emzi0767), " +
                                "[YourAverageBlackGuy](https://github.com/YourAverageBlackGuy), " +
                                "[DrCreo](https://github.com/DrCreo), " +
                                "[aexolate](https://github.com/aexolate), " +
                                "[Drake103](https://github.com/Drake103), " +
                                "[Izumemori](https://github.com/Izumemori), " +
                                "[OoLunar](https://github.com/OoLunar) and " +
                                "[InFTord](https://github.com/InFTord)"
                            )
                            .WithField("Want to contribute?", "Contributions are always welcome at our [GitHub repo.](https://github.com/Naamloos/ModCore)")
                            .WithField("Donate?", "Currently, ModCore is hosted off my (Naamloos's) own money. Donations are always welcome over at [Ko-Fi](https://ko-fi.com/Naamloos)!")
                            .WithFooter("v3.0.0-alpha (early access)")
                    }
            });

            if(!resp.Success)
            {
                _logger.LogError("Failed sending about info on request! Status {0}", resp.HttpResponse.StatusCode);
            }
        }
    }
}
