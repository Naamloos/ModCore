using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Entities.Enums;
using ModCore.Common.Discord.Entities.Interactions;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Rest;
using ModCore.Common.Utils;
using ModCore.Services.Shard.Interactions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Services.Shard.Interactions.Commands
{
    public class GeneralCommands : BaseInteractionContainer
    {
        private readonly DiscordRest _rest;
        private readonly ILogger _logger;

        public GeneralCommands(DiscordRest rest, ILogger<GeneralCommands> logger) 
        {
            _rest = rest;
            _logger = logger;
        }

        [Command("about", "General information about ModCore", true, false)]
        public async Task AboutAsync(InteractionCreate data)
        {
            _logger.LogInformation($"Succesfully interpreted command interaction- 'about' as a command, and reached command handler method.");
            var modcoreSelf = await _rest.GetCurrentUserAsync();
            if (!modcoreSelf.Success)
                return;

            var resp = await _rest.CreateInteractionResponseAsync(data.Id, data.Token, InteractionResponseType.ChannelMessageWithSource, new InteractionMessageResponse()
            {
                Content = $"{data.Member.Value.User.Value.Mention}, Welcome to ModCore v3. " +
                        $"This is an early ALPHA version of ModCore, not yet available to the general public.",
                Flags = MessageFlags.Ephemeral,
                Embeds = new[]
                    {
                        new Embed()
                        {
                            Description = $"ModCore is a Discord bot focused on moderation and server management, written from scratch in C#.",
                            Color = ColorConverter.FromHex("#089FDF"),
                            Author = new EmbedAuthor()
                            {
                                Name = "ModCore",
                                IconUrl = $"https://cdn.discordapp.com/avatars/{modcoreSelf.Value.Id}/{modcoreSelf.Value.AvatarHash}.png",
                                Url = "https://github.com/Naamloos/ModCore"
                            },
                            Thumbnail = new EmbedThumbnail()
                            {
                                Url = $"https://cdn.discordapp.com/avatars/{modcoreSelf.Value.Id}/{modcoreSelf.Value.AvatarHash}.png"
                            },
                            Fields = new List<EmbedField>()
                            {
                                new()
                                {
                                    Name = "Main Developer",
                                    Value = "[Naamloos](https://github.com/Naamloos)"
                                },
                                new()
                                {
                                    Name = "Special thanks to all of these wonderful contributors:",
                                    Value = "[uwx](https://github.com/uwx), " +
                                        "[jcryer](https://github.com/jcryer), " +
                                        "[Emzi0767](https://github.com/Emzi0767), " +
                                        "[YourAverageBlackGuy](https://github.com/YourAverageBlackGuy), " +
                                        "[DrCreo](https://github.com/DrCreo), " +
                                        "[aexolate](https://github.com/aexolate), " +
                                        "[Drake103](https://github.com/Drake103), " +
                                        "[Izumemori](https://github.com/Izumemori), " +
                                        "[OoLunar](https://github.com/OoLunar) and " +
                                        "[InFTord](https://github.com/InFTord)"
                                },
                                new()
                                {
                                    Name = "Want to contribute?",
                                    Value = "Contributions are always welcome at our [GitHub repo.](https://github.com/Naamloos/ModCore)"
                                },
                                new()
                                {
                                    Name = "Donate?",
                                    Value = "Currently, ModCore is hosted off my (Naamloos's) own money. Donations are always welcome over at [Ko-Fi](https://ko-fi.com/Naamloos)!"
                                }
                            },
                            Footer = new EmbedFooter()
                            {
                                Text = "v3.0.0-alpha (early access)"
                            }
                        }
                    }
            });
        }
    }
}
