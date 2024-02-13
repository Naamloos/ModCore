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
using System.Linq;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Enums;
using ModCore.Database.DatabaseEntities;
using ModCore.Entities;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace ModCore.Listeners
{
    public class StarboardListeners
    {
        private static ConcurrentDictionary<ulong, SemaphoreSlim> semaphores = new ConcurrentDictionary<ulong, SemaphoreSlim>();

        [AsyncListener(EventType.MessageReactionAdded)]
        public static async Task ReactionAddedAsync(MessageReactionAddEventArgs eventargs, DatabaseContextBuilder database, 
            DiscordClient client, IMemoryCache cache)
        {
            DiscordMessage message = await eventargs.Channel.GetMessageAsync(eventargs.Message.Id, true);
            DiscordUser user = await eventargs.Guild.GetMemberAsync(eventargs.User.Id);

            if (user.Id == message.Author.Id)
                return; // same author, return
            if (user.IsBot)
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

                if (starEmoji.EmojiId != 0)
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
                DiscordMember member;
                try
                {
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
                    if(!ctx.StarDatas.Any(x => x.ChannelId == (long)message.Channel.Id 
                        && x.MessageId == (long)message.Id 
                        && x.StargazerId == (long)eventargs.User.Id))
                    {
                        // Add stardata if it not yet exists.
                        ctx.StarDatas.Add(new DatabaseStarData()
                        {
                            StarboardMessageId = 0,
                            StargazerId = (long)eventargs.User.Id,
                            AuthorId = (long)message.Author.Id,
                            ChannelId = (long)message.Channel.Id,
                            GuildId = (long)message.Channel.GuildId,
                            MessageId = (long)message.Id
                        });
                        await ctx.SaveChangesAsync();
                    }
                    await updateStarboardMessage(ctx, message, resolvedStarEmoji, settings, client);
                }
                finally
                {
                    // release semaphore, flush database changes
                    semaphore.Release();
                    await ctx.SaveChangesAsync();
                }
            }
        }

        [AsyncListener(EventType.MessageReactionRemoved)]
        public static async Task ReactionRemovedAsync(MessageReactionRemoveEventArgs eventargs, DatabaseContextBuilder database,
            DiscordClient client, IMemoryCache cache)
        {
            DiscordMessage message = await eventargs.Channel.GetMessageAsync(eventargs.Message.Id, true);
            DiscordUser user = await eventargs.Guild.GetMemberAsync(eventargs.User.Id);

            if (user.Id == message.Author.Id)
                return; // same author, return
            if (user.IsBot)
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

                if (starEmoji.EmojiId != 0)
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
                DiscordMember member;
                try
                {
                    member = await eventargs.Guild.GetMemberAsync(eventargs.User.Id, false);
                }
                catch (Exception)
                {
                    return;
                }

                SemaphoreSlim semaphore;
                if (!cache.TryGetValue($"starboard_semaphore_{message.Id}", out semaphore))
                {
                    semaphore = new SemaphoreSlim(1);
                    cache.Set($"starboard_semaphore_{message.Id}", semaphore);
                }

                await semaphore.WaitAsync();
                try
                {
                    var data = ctx.StarDatas.FirstOrDefault(x => x.ChannelId == (long)message.Channel.Id
                        && x.MessageId == (long)message.Id
                        && x.StargazerId == (long)eventargs.User.Id);
                    if(data != default(DatabaseStarData))
                    {
                        ctx.StarDatas.Remove(data);
                        await ctx.SaveChangesAsync();
                    }

                    await updateStarboardMessage(ctx, message, resolvedStarEmoji, settings, client, data.StarboardMessageId);
                }
                finally
                {
                    // release semaphore, flush database changes
                    semaphore.Release();
                    await ctx.SaveChangesAsync();
                }
            }
        }

        [AsyncListener(EventType.MessageReactionsCleared)]
        public static async Task ReactionsClearedAsync(MessageReactionsClearEventArgs eventargs, DatabaseContextBuilder database,
            DiscordClient client, IMemoryCache cache)
        {
            DiscordMessage message = await eventargs.Channel.GetMessageAsync(eventargs.Message.Id, true);

            using (DatabaseContext ctx = database.CreateContext())
            {
                GuildSettings settings;
                settings = eventargs.Guild.GetGuildSettings(ctx);

                if (settings == null)
                    return; // No guild settings so starboard is disabled.

                if (!settings.Starboard.Enable)
                    return;// starboard is disabled.

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

                SemaphoreSlim semaphore;
                if (!cache.TryGetValue($"starboard_semaphore_{message.Id}", out semaphore))
                {
                    semaphore = new SemaphoreSlim(1);
                    cache.Set($"starboard_semaphore_{message.Id}", semaphore);
                }

                await semaphore.WaitAsync();
                try
                {
                    var starDataWithMessage = ctx.StarDatas.FirstOrDefault(x => x.MessageId == (long)eventargs.Message.Id &&
                        x.ChannelId == (long)eventargs.Channel.Id &&
                        x.StarboardMessageId != 0);

                    if (starDataWithMessage != default(DatabaseStarData))
                    {
                        try
                        {
                            await starboardChannel.DeleteMessageAsync(await starboardChannel.GetMessageAsync((ulong)starDataWithMessage.StarboardMessageId));
                        }
                        catch(Exception)
                        {
                            // message doesn't exist, bummer, don't care
                        }
                    }
                    // delete all star datas for this message.
                    ctx.StarDatas.RemoveRange(ctx.StarDatas.Where(x =>
                        x.MessageId == (long)eventargs.Message.Id &&
                        x.ChannelId == (long)eventargs.Channel.Id));
                }
                finally
                {
                    // release semaphore, flush database changes
                    semaphore.Release();
                    await ctx.SaveChangesAsync();
                }
            }
        }

        [AsyncListener(EventType.MessageReactionEmojiRemoved)]
        public static async Task ReactionsEmojiRemoveAsync(MessageReactionRemoveEmojiEventArgs eventargs, DatabaseContextBuilder database,
            DiscordClient client, IMemoryCache cache)
        {
            DiscordMessage message = await eventargs.Channel.GetMessageAsync(eventargs.Message.Id, true);

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

                if (starEmoji.EmojiId != 0)
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

                SemaphoreSlim semaphore;
                if (!cache.TryGetValue($"starboard_semaphore_{message.Id}", out semaphore))
                {
                    semaphore = new SemaphoreSlim(1);
                    cache.Set($"starboard_semaphore_{message.Id}", semaphore);
                }

                await semaphore.WaitAsync();
                try
                {
                    var starDataWithMessage = ctx.StarDatas.FirstOrDefault(x => x.MessageId == (long)eventargs.Message.Id &&
                        x.ChannelId == (long)eventargs.Channel.Id &&
                        x.StarboardMessageId != 0);

                    if (starDataWithMessage != default(DatabaseStarData))
                    {
                        try
                        {
                            await starboardChannel.DeleteMessageAsync(await starboardChannel.GetMessageAsync((ulong)starDataWithMessage.StarboardMessageId));
                        }
                        catch (Exception)
                        {
                            // message doesn't exist, bummer, don't care
                        }
                    }
                    // delete all star datas for this message.
                    ctx.StarDatas.RemoveRange(ctx.StarDatas.Where(x =>
                        x.MessageId == (long)eventargs.Message.Id &&
                        x.ChannelId == (long)eventargs.Channel.Id));
                }
                finally
                {
                    // release semaphore, flush database changes
                    semaphore.Release();
                    await ctx.SaveChangesAsync();
                }
            }
        }

        private static async Task updateStarboardMessage(DatabaseContext database, DiscordMessage message, DiscordEmoji emoji,
            GuildSettings settings, DiscordClient client, long predefinedStarboardMessageId = 0l)
        {
            // first, we fetch the existing stars and try to find an existing starboard message ID
            var existingStars = database.StarDatas.Where(x => x.ChannelId == (long)message.Channel.Id && x.MessageId == (long)message.Id);
            var starboardMessageId = predefinedStarboardMessageId;
            if(existingStars.Any(x => x.StarboardMessageId != 0))
            {
                starboardMessageId = existingStars.First(x => x.StarboardMessageId != 0).StarboardMessageId;
            }

            // fetch the starboard channel
            DiscordChannel channel;
            try
            {
                channel = await client.GetChannelAsync(settings.Starboard.ChannelId);
            }
            catch(Exception ex)
            {
                // channel doesn't exist so we just. ignore I guess?
                return;
            }

            // Not enough stars, delete message
            if(existingStars.Count() < settings.Starboard.Minimum)
            {
                try
                {
                    // delete the message if there even is any
                    await channel.DeleteMessageAsync(await channel.GetMessageAsync((ulong)starboardMessageId));
                }
                catch(Exception)
                {
                }

                await existingStars.ForEachAsync(x => x.StarboardMessageId = 0);
                return;
            }

            // Try to find or create a qualified starboard message
            DiscordMessage starboardMessage;
            if(starboardMessageId != 0)
            {
                try
                {
                    starboardMessage = await channel.GetMessageAsync((ulong)starboardMessageId, true);
                }catch(Exception)
                {
                    // message not found. creating new.
                    try
                    {
                        starboardMessage = await channel.SendMessageAsync("Preparing starboard message...");
                    }
                    catch(Exception)
                    {
                        // ok never mind.
                        return;
                    }
                }
            }
            else
            {
                try
                {
                    starboardMessage = await channel.SendMessageAsync("Preparing starboard message...");
                }
                catch (Exception)
                {
                    // failed posting, return
                    return;
                }
            }

            // Update the starboard message ID for all existing stars
            starboardMessageId = (long)starboardMessage.Id;
            await existingStars.ForEachAsync(x => x.StarboardMessageId = (long)starboardMessage.Id);
            database.UpdateRange(existingStars);
            await database.SaveChangesAsync();
            try
            {
                // Post the updated starboard message.
                await buildStarboardMessage(message, existingStars.Count(), emoji).ModifyAsync(starboardMessage);
            }
            catch(Exception)
            {
            }
        }

        public static readonly string[] validFileExts = { ".jpg", ".gif", ".png", ".jpeg", ".webp" };
        public static DiscordMessageBuilder buildStarboardMessage(DiscordMessage sourceMessage, long count, DiscordEmoji emoji)
        {
            // TODO cleanup
            var embed = new DiscordEmbedBuilder()
                .WithAuthor($"{sourceMessage.Author.GetDisplayUsername()}",
                iconUrl: (string.IsNullOrEmpty(sourceMessage.Author.AvatarHash) ? sourceMessage.Author.DefaultAvatarUrl : sourceMessage.Author.AvatarUrl))
                .WithDescription(sourceMessage.Content.Truncate(800, "..."))
                .WithFooter($"ID: {sourceMessage.Id}")
                .WithTimestamp(sourceMessage.Id);

            var emotename = emoji.GetDiscordName().Replace(":", "");
            emotename = emotename.EndsWith('s') ? emotename : count > 1 ? emotename + "s" : emotename;

            if (sourceMessage.ReferencedMessage != null)
            {
                var refContent = sourceMessage.ReferencedMessage.Content.Truncate(200, "...").Replace(")[", "​)[") + " ";

                embed.Description += $"\n\n**➥** {sourceMessage.ReferencedMessage.Author.Mention}: " +
                    $"{refContent} {(sourceMessage.ReferencedMessage.Attachments.Count() > 0 ? $"_<{sourceMessage.ReferencedMessage.Attachments.Count()} file(s)>_" : "")}";
            }

            var embeds = new List<DiscordEmbed>();
            var imageFiles = sourceMessage.Attachments.Where(x =>
            {
                var uri = new Uri(x.Url);
                return validFileExts.Contains(Path.GetExtension(uri.AbsolutePath));
            });

            if (imageFiles.Any()) 
            {
                foreach (var img in imageFiles)
                {
                    embeds.Add(new DiscordEmbedBuilder(embed).WithUrl("https://github.com/Naamloos/ModCore").WithImageUrl(img.Url));
                }
            }
            else
            {
                embeds.Add(embed);
            }

            var messageBuilder = new DiscordMessageBuilder()
                .AddEmbeds(embeds)
                .WithContent($"{emoji} {count} {emotename} in {sourceMessage.Channel.Mention}");

            messageBuilder.AddComponents(new DiscordLinkButtonComponent(sourceMessage.JumpLink.ToString(), "Go to message"));
            return messageBuilder;
        }
    }
}
