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
                        var truncatedText = message.Content.Length > 250? message.Content.Truncate(250) + "..." : message.Content;
                        embeds.Add(new DiscordEmbedBuilder()
                            .WithDescription($"{truncatedText}\n\n[Jump to message]({match.Value.Replace("!", "")})")
                            .WithAuthor(message.Author.GetDisplayUsername(), iconUrl: message.Author.GetAvatarUrl(ImageFormat.Png))
                            .Build());
                    }
                    catch (Exception) { }
                }

                if(embeds.Count > 0)
                    await eventargs.Channel.SendMessageAsync(x => x.AddEmbeds(embeds));
            }
        }
    }
}
