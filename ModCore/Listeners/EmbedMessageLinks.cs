using DSharpPlus.EventArgs;
using DSharpPlus;
using Microsoft.Extensions.Caching.Memory;
using ModCore.Database;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Enums;
using System.Threading.Tasks;
using ModCore.Database.JsonEntities;
using ModCore.Utils.Extensions;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
using DSharpPlus.Entities;
using Humanizer;
using System.Linq;
using System.IO;

namespace ModCore.Listeners
{
    public class EmbedMessageLinks
    {
        public static Regex AlwaysEmbedRegex = new Regex(@"https?:\/\/.*?discord.com\/channels\/([0-9]+)\/([0-9]+)\/([0-9]+)", RegexOptions.Compiled);
        public static Regex PrefixedEmbedRegex = new Regex(@"!https?:\/\/.*?discord.com\/channels\/([0-9]+)\/([0-9]+)\/([0-9]+)", RegexOptions.Compiled);

        [AsyncListener(EventType.MessageCreated)]
        public static async Task ReactionAddedAsync(MessageCreateEventArgs eventargs, DatabaseContextBuilder database,
            DiscordClient client, IMemoryCache cache)
        {
            using (DatabaseContext ctx = database.CreateContext())
            {
                GuildSettings settings;
                settings = eventargs.Guild.GetGuildSettings(ctx);

                if (settings == null)
                    return; // No guild settings so starboard is disabled.

                if (settings.EmbedMessageLinks == EmbedMessageLinksMode.Disabled)
                    return;// embedding is disabled.

                Regex rex = settings.EmbedMessageLinks == EmbedMessageLinksMode.Always? AlwaysEmbedRegex : PrefixedEmbedRegex;
                var matches = rex.Matches(eventargs.Message.Content);

                var embeds = new List<DiscordEmbed>();

                foreach(Match match in matches)
                {
                    var guildId = ulong.Parse(match.Groups[1].Value);
                    var channelId = ulong.Parse(match.Groups[2].Value);
                    var messageId = ulong.Parse(match.Groups[3].Value);

                    if (guildId != eventargs.Guild.Id) return; // not same guild
                    if (!eventargs.Guild.Channels.ContainsKey(channelId)) return; // channel not found

                    var channel = eventargs.Guild.Channels[channelId];
                    try
                    {
                        var message = await channel.GetMessageAsync(messageId);
                        // no exception = exists

                        var truncatedText = message.Content.Length > 250 ? message.Content.Truncate(250) + "..." : message.Content;
                        var embed = new DiscordEmbedBuilder()
                            .WithDescription($"{truncatedText}\n")
                            .WithAuthor(message.Author.GetDisplayUsername(), iconUrl: message.Author.GetAvatarUrl(ImageFormat.Png));

                        var imageFiles = message.Attachments.Where(x =>
                        {
                            var uri = new Uri(x.Url);
                            return StarboardListeners.validFileExts.Contains(Path.GetExtension(uri.AbsolutePath));
                        });

                        if (imageFiles.Any())
                        {
                            embed.WithThumbnail(imageFiles.First().Url);
                            var count = imageFiles.Count();
                            if (count > 1)
                            {
                                embed.WithDescription(embed.Description + $"\n_Contains ({count - 1}) more attachments._");
                            }
                        }
                        embed.WithDescription(embed.Description + $"\n[Jump to message]({match.Value.Replace("!", "")})");

                        embeds.Add(embed.Build());
                    }
                    catch (Exception) { }
                }

                if(embeds.Count > 0)
                    await eventargs.Channel.SendMessageAsync(x => x.AddEmbeds(embeds));
            }
        }
    }
}
