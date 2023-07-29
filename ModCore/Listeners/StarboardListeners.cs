﻿using DSharpPlus.EventArgs;
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

namespace ModCore.Listeners
{
    public class StarboardListeners
    {
        private static ConcurrentDictionary<ulong, SemaphoreSlim> semaphores = new ConcurrentDictionary<ulong, SemaphoreSlim>();

        [AsyncListener(EventType.MessageReactionAdded)]
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

        public static async Task ReactionRemovedAsync(MessageReactionRemoveEventArgs eventargs, DatabaseContextBuilder database,
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

        [AsyncListener(EventType.MessageReactionsCleared)]
        public static async Task ReactionsClearedAsync(MessageReactionsClearEventArgs eventargs, DatabaseContextBuilder database,
            DiscordClient client, IMemoryCache cache)
        {
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

                // fetch User and Message since we can't trust cache nowadays lolol
                DiscordMessage message;
                try
                {
                    message = await eventargs.Channel.GetMessageAsync(eventargs.Message.Id, true);
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
                    var starDataWithMessage = ctx.StarDatas.FirstOrDefault(x => x.StarboardMessageId != 0);
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
                        x.ChannelId == (long)eventargs.Channel.Id &&
                        x.GuildId == (long)eventargs.Guild.Id));
                }
                finally
                {
                    // release semaphore, flush database changes
                    semaphore.Release();
                    await ctx.SaveChangesAsync();
                }
            }
        }

        public static async Task ReactionsEmojiRemoveAsync(MessageReactionRemoveEmojiEventArgs eventargs, DatabaseContextBuilder database,
            DiscordClient client, IMemoryCache cache)
        {
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
                try
                {
                    message = await eventargs.Channel.GetMessageAsync(eventargs.Message.Id, true);
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
                    var starDataWithMessage = ctx.StarDatas.FirstOrDefault(x => x.StarboardMessageId != 0);
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
                        x.ChannelId == (long)eventargs.Channel.Id &&
                        x.GuildId == (long)eventargs.Guild.Id));
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
            GuildSettings settings, DiscordClient client)
        {
            // first, we fetch the existing stars and try to find an existing starboard message ID
            var existingStars = database.StarDatas.Where(x => x.ChannelId == (long)message.Channel.Id && x.MessageId == (long)message.Id);
            var starboardMessageId = 0l;
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
            await existingStars.ForEachAsync(x => x.StarboardMessageId = starboardMessageId);
            try
            {
                // Post the updated starboard message.
                await buildStarboardMessage(message, existingStars.Count(), emoji).ModifyAsync(starboardMessage);
            }
            catch(Exception)
            {
            }
        }

        private static DiscordMessageBuilder buildStarboardMessage(DiscordMessage sourceMessage, long count, DiscordEmoji emoji)
        {
            // TODO cleanup
            var embed = new DiscordEmbedBuilder()
                .WithAuthor($"{sourceMessage.Author.GetDisplayUsername()}",
                iconUrl: (string.IsNullOrEmpty(sourceMessage.Author.AvatarHash) ? sourceMessage.Author.DefaultAvatarUrl : sourceMessage.Author.AvatarUrl))
                .WithDescription(sourceMessage.Content.Truncate(800, "..."))
                .WithFooter($"ID: {sourceMessage.Id}")
                .WithTimestamp(sourceMessage.Id);

            // This is shit code kek
            if (sourceMessage.Attachments.Any(x => x.Url.ToLower().EndsWith(".jpg") || x.Url.ToLower().EndsWith(".png")
             || x.Url.ToLower().EndsWith(".jpeg") || x.Url.ToLower().EndsWith(".gif")))
                embed.WithImageUrl(sourceMessage.Attachments.First(x => x.Url.ToLower().EndsWith(".jpg") || x.Url.ToLower().EndsWith(".png")
            || x.Url.ToLower().EndsWith(".jpeg") || x.Url.ToLower().EndsWith(".gif")).Url);

            var emotename = emoji.GetDiscordName().Replace(":", "");
            emotename = emotename.EndsWith('s') ? emotename : count > 1 ? emotename + "s" : emotename;

            if (sourceMessage.ReferencedMessage != null)
            {
                var refContent = sourceMessage.ReferencedMessage.Content.Truncate(200, "...").Replace(")[", "​)[") + " ";

                embed.Description += $"\n\n**➥** {sourceMessage.ReferencedMessage.Author.Mention}: " +
                    $"{refContent} {(sourceMessage.ReferencedMessage.Attachments.Count() > 0 ? $"_<{sourceMessage.ReferencedMessage.Attachments.Count()} file(s)>_" : "")}";
            }

            var messageBuilder = new DiscordMessageBuilder()
                .AddEmbed(embed)
                .WithContent($"{emoji} {count} {emotename} in {sourceMessage.Channel.Mention}");

            messageBuilder.AddComponents(new DiscordLinkButtonComponent(sourceMessage.JumpLink.ToString(), "Go to message"));
            messageBuilder.WithContent("");

            return messageBuilder;
        }
    }
}