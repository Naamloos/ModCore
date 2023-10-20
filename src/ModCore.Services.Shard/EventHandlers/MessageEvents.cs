using Microsoft.Extensions.Logging;
using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Messages;
using ModCore.Common.Discord.Gateway.EventData.Incoming;
using ModCore.Common.Discord.Gateway.Events;
using ModCore.Common.Discord.Rest;
using ModCore.Common.Utils;

namespace ModCore.Services.Shard.EventHandlers
{
    public class MessageEvents : ISubscriber<MessageCreate>, ISubscriber<InteractionCreate>
    {
        private readonly ILogger _logger;
        private readonly DiscordRest _rest;

        public MessageEvents(ILogger<StartupEvents> logger, DiscordRest rest)
        {
            _logger = logger;
            _rest = rest;
        }

        public async Task HandleEvent(MessageCreate data)
        {
            _logger.LogInformation("@{0}: {1}", data.Author.Username, data.Content);

            if(data.Mentions.Any(x => x.Id == 811197813043494942))
            {
                var modcoreSelf = await _rest.GetCurrentUserAsync();
                if(!modcoreSelf.Success)
                    return;
                
                var responseMessage = new CreateMessage()
                {
                    Content = $"{data.Author.Mention}, Welcome to ModCore v3. " +
                        $"This is an early ALPHA version of ModCore, not yet available to the general public.",
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
                };

                var resp = await _rest.CreateMessageAsync(data.ChannelId, responseMessage);
                if(resp.Success)
                {
                    var createdMessage = resp.Value;
                    _logger.LogInformation("Created message with new ID: {0} {1}", createdMessage.Id, createdMessage.GetJumpLink(data.GuildId));
                }
            }
        }

        public async Task HandleEvent(InteractionCreate data)
        {
            _logger.LogDebug("Incoming interaction");
        }
    }
}
