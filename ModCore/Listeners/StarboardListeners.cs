using DSharpPlus.EventArgs;
using DSharpPlus;
using ModCore.Database;
using System.Threading.Tasks;
using ModCore.Database.JsonEntities;
using ModCore.Utils.Extensions;
using DSharpPlus.Entities;
using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;

namespace ModCore.Listeners
{
    public class StarboardListeners
    {
        private static ConcurrentDictionary<ulong, SemaphoreSlim> semaphores = new ConcurrentDictionary<ulong, SemaphoreSlim>();

        public static async Task ReactionAddedAsync(MessageReactionAddEventArgs eventargs, DatabaseContextBuilder database, 
            DiscordClient client, IMemoryCache cache)
        {
            if (eventargs.User.Id == eventargs.Message.Author.Id)
                return; // same author, return
            if (eventargs.User.IsBot)
                return; // no bots allowed pshhk pshhk

            using (DatabaseContext ctx = database.CreateContext())
            {
                GuildSettings settings;
                settings = eventargs.Guild.GetGuildSettings(ctx);

                if (settings == null)
                    return; // No guild settings so starboard is disabled.

                if (!settings.Starboard.Enable)
                    return;// starboard is disabled.

                GuildEmoji starEmoji = settings.Starboard.Emoji;
                DiscordEmoji resolvedStarEmoji;

                if (starEmoji is null)
                    return; // emoji is null so we're ignoring.

                if (starEmoji.EmojiId == 0)
                {
                    // emoji is a guildemote
                    if (!DiscordEmoji.TryFromGuildEmote(client, starEmoji.EmojiId, out resolvedStarEmoji))
                        return; // return when invalid
                }
                else
                {
                    // emoji is unicode
                    if (!DiscordEmoji.TryFromUnicode(starEmoji.EmojiName, out resolvedStarEmoji))
                        return; // return when invalid
                }

                if (resolvedStarEmoji != eventargs.Emoji)
                    return; // This is not the emoji we resolved so we can quit

                DiscordChannel starboardChannel;
                try
                {
                    starboardChannel = await client.GetChannelAsync(settings.Starboard.ChannelId);
                }
                catch (Exception)
                {
                    return;// Unable to fetch the starboard channel so we ignore.
                }

                if (starboardChannel.GuildId != eventargs.Guild.Id)
                    return; // somehow Guild ID mismatch, return

                if (eventargs.Channel.IsNSFW && !starboardChannel.IsNSFW)
                    return; // We're not allowing NSFW stars in SFW starboard channels.

                if (eventargs.Channel.Id == starboardChannel.Id)
                    return; // disallow starring on starboard messages.

                // fetch User and Message since we can't trust cache nowadays lolol
                DiscordMessage message;
                DiscordMember member;
                try
                {
                    message = await eventargs.Channel.GetMessageAsync(eventargs.Message.Id, true);
                    member = await eventargs.Guild.GetMemberAsync(eventargs.User.Id, false);
                }
                catch(Exception)
                {
                    return;
                }

                SemaphoreSlim semaphore;
                if(!cache.TryGetValue($"starboard_semaphore_{message.Id}", out semaphore))
                {
                    semaphore = new SemaphoreSlim(1);
                    cache.Set($"starboard_semaphore_{message.Id}", semaphore);
                }

                await semaphore.WaitAsync();
                try
                {

                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        private static DiscordMessageBuilder buildStarboardMessage()
        {
            var builder = new DiscordMessageBuilder();
            // TODO
            return builder;
        }
    }
}
