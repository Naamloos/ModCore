using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Humanizer;
using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using ModCore.Database.JsonEntities;
using ModCore.Entities;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Enums;
using ModCore.Utils;
using ModCore.Utils.Extensions;

namespace ModCore.Listeners
{
    public class Reactions
    {
        [AsyncListener(EventType.MessageReactionAdded)]
        public static async Task ReactionAdd(MessageReactionAddEventArgs eventargs, DatabaseContextBuilder database, DiscordClient client)
        {
            GuildSettings config = null;
            using (var db = database.CreateContext())
            {
                config = eventargs.Channel.Guild.GetGuildSettings(db);
                if (config == null)
                    return;

                // Reaction roles
				if (config.ReactionRoles.Any(x => (ulong)x.ChannelId == eventargs.Channel.Id && (ulong)x.MessageId == eventargs.Message.Id 
                    && (ulong)x.Reaction.EmojiId == eventargs.Emoji.Id && x.Reaction.EmojiName == eventargs.Emoji.Name))
				{
					var reactionroleid = (ulong)config.ReactionRoles.First(
						x => (ulong)x.ChannelId == eventargs.Channel.Id && (ulong)x.MessageId == eventargs.Message.Id 
                            && (ulong)x.Reaction.EmojiId == eventargs.Emoji.Id && x.Reaction.EmojiName == eventargs.Emoji.Name).RoleId;
					var reactionrole = eventargs.Channel.Guild.GetRole(reactionroleid);
					var member = await eventargs.Channel.Guild.GetMemberAsync(eventargs.User.Id);
					if(!member.Roles.Any(x => x.Id == reactionroleid))
						await member.GrantRoleAsync(reactionrole);
				}

                // Starboard
                var emoji = config.Starboard.Emoji;
                DiscordEmoji discordemoji = null;
                if (emoji.EmojiId != 0)
                    discordemoji = DiscordEmoji.FromGuildEmote(client, (ulong)emoji.EmojiId);
                else
                    discordemoji = DiscordEmoji.FromUnicode(client, emoji.EmojiName);

                if (config.Starboard.Enable && eventargs.Emoji == discordemoji)
                {
                    long starboardmessageid = 0;
                    var channel = eventargs.Channel.Guild.Channels.First(x => x.Key == (ulong)config.Starboard.ChannelId).Value;

                    if ((!channel.IsNSFW) && eventargs.Channel.IsNSFW)
                        return;

                    if (channel.Id != eventargs.Channel.Id) // star on starboard entry
                    {
                        // fetch REST message (cache sometimes fails)
                        var message = await eventargs.Channel.GetMessageAsync(eventargs.Message.Id);
                        var user = await eventargs.Channel.Guild.GetMemberAsync(eventargs.User.Id);

                        if (message.Author.Id == user.Id || user.IsBot)
                            return;

                        if (db.StarDatas.Any(x => (ulong)x.MessageId == eventargs.Message.Id))
                        {
                            var count = db.StarDatas.Count(x => (ulong)x.MessageId == eventargs.Message.Id);

                            if (db.StarDatas.Any(x => (ulong)x.MessageId == eventargs.Message.Id && x.StarboardMessageId != 0))
                            {
                                var other = db.StarDatas.First(x => (ulong)x.MessageId == message.Id && x.StarboardMessageId != 0);
                                var oldsbmessage = await channel.GetMessageAsync((ulong)other.StarboardMessageId);

                                var starboardmessage = await oldsbmessage.ModifyAsync(buildMessage(message, eventargs.Emoji, count + 1));
                                starboardmessageid = (long)starboardmessage.Id;
                            }
                            else
                            {
                                if(count + 1 >= config.Starboard.Minimum)
                                {
                                    // create msg
                                    var starboardmessage = await channel.SendMessageAsync(buildMessage(message, eventargs.Emoji, count + 1));
                                    starboardmessageid = (long)starboardmessage.Id;
                                }
                            }
                        }
                        else if (config.Starboard.Minimum <= 1)
                        {
                            var starboardmessage = await channel.SendMessageAsync(buildMessage(message, eventargs.Emoji, 1));
                            starboardmessageid = (long)starboardmessage.Id;
                        }

                        await db.StarDatas.AddAsync(new DatabaseStarData
                        {
                            ChannelId = (long)channel.Id,
                            GuildId = (long)channel.Guild.Id,
                            MessageId = (long)message.Id,
                            AuthorId = (long)message.Author.Id,
                            StarboardMessageId = starboardmessageid,
                            StargazerId = (long)eventargs.User.Id,
                        });

                        // somebody once told me...
                        var allstars = db.StarDatas.Where(x => (ulong)x.MessageId == message.Id).ToList();
                        allstars.ForEach(x => x.StarboardMessageId = starboardmessageid);
                        db.StarDatas.UpdateRange(allstars);

                        await db.SaveChangesAsync();
                    }
                }
            }
        }

        [AsyncListener(EventType.MessageReactionRemoved)]
        public static async Task ReactionRemove(MessageReactionRemoveEventArgs eventargs, DatabaseContextBuilder database, DiscordClient client)
        {
            GuildSettings config = null;
            using (var db = database.CreateContext())
            {
                config = eventargs.Channel.Guild.GetGuildSettings(db);
                if (config == null)
                    return;

				if (config.ReactionRoles.Any(x => (ulong)x.ChannelId == eventargs.Channel.Id && (ulong)x.MessageId == eventargs.Message.Id && (ulong)x.Reaction.EmojiId == eventargs.Emoji.Id && x.Reaction.EmojiName == eventargs.Emoji.Name))
				{
					var reactionroleid = (ulong)config.ReactionRoles.First(
						x => (ulong)x.ChannelId == eventargs.Channel.Id && (ulong)x.MessageId == eventargs.Message.Id && (ulong)x.Reaction.EmojiId == eventargs.Emoji.Id && x.Reaction.EmojiName == eventargs.Emoji.Name).RoleId;
					var reactionrole = eventargs.Channel.Guild.GetRole(reactionroleid);
					var member = await eventargs.Channel.Guild.GetMemberAsync(eventargs.User.Id);
					if (member.Roles.Any(x => x.Id == reactionroleid))
						await member.RevokeRoleAsync(reactionrole);
				}

				var emoji = config.Starboard.Emoji;
                DiscordEmoji discordemoji = null;
                if (emoji.EmojiId != 0)
                    discordemoji = DiscordEmoji.FromGuildEmote(client, (ulong)emoji.EmojiId);
                else
                    discordemoji = DiscordEmoji.FromUnicode(client, emoji.EmojiName);

                if (config.Starboard.Enable && eventargs.Emoji == discordemoji)
                {
                    var channel = eventargs.Channel.Guild.Channels.First(x => x.Key == (ulong)config.Starboard.ChannelId).Value;
                    if (channel.Id != eventargs.Channel.Id)
                    {
                        // fetch REST message (cache sometimes fails)
                        var message = await eventargs.Channel.GetMessageAsync(eventargs.Message.Id);
                        var user = await channel.Guild.GetMemberAsync(eventargs.User.Id);

                        long newstarboardmessageid = 0;
                        if (db.StarDatas.Any(x => (ulong)x.MessageId == message.Id && (ulong)x.StargazerId == user.Id && x.StarboardMessageId != 0))
                        {
                            var star = db.StarDatas.First(x => (ulong)x.MessageId == message.Id && (ulong)x.StargazerId == user.Id);
                            var count = db.StarDatas.Count(x => (ulong)x.MessageId == message.Id);

                            if (db.StarDatas.Any(x => (ulong)x.MessageId == message.Id && (ulong)x.StargazerId == user.Id && x.StarboardMessageId != 0))
                            {
                                var starboardmessageid = db.StarDatas.First(x => (ulong)x.MessageId == message.Id && (ulong)x.StargazerId == user.Id && x.StarboardMessageId != 0)
                                    .StarboardMessageId;

                                var m = await channel.GetMessageAsync((ulong)starboardmessageid);
                                if (count - 1 >= config.Starboard.Minimum)
                                {
                                    await m.ModifyAsync(buildMessage(message, eventargs.Emoji, count - 1));
                                    newstarboardmessageid = starboardmessageid;
                                }
                                else
                                {
                                    await m.DeleteAsync();
                                }
                            }

                            db.StarDatas.Remove(star);
                            await db.SaveChangesAsync();

                            // somebody once told me...
                            var allstars = db.StarDatas.Where(x => (ulong)x.MessageId == message.Id).ToList();
                            allstars.ForEach(x => x.StarboardMessageId = newstarboardmessageid);
                            db.StarDatas.UpdateRange(allstars);

                            await db.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        [AsyncListener(EventType.MessageReactionsCleared)]
        public static async Task ReactionClear(MessageReactionsClearEventArgs eventargs, DiscordClient client, DatabaseContextBuilder database)
        {
            GuildSettings config = null;
            using (var db = database.CreateContext())
            {
                config = eventargs.Channel.Guild.GetGuildSettings(db);
                if (config == null)
                    return;

                var emoji = config.Starboard.Emoji;
                DiscordEmoji discordemoji = null;
                if (emoji.EmojiId != 0)
                    discordemoji = DiscordEmoji.FromGuildEmote(client, (ulong)emoji.EmojiId);
                else
                    discordemoji = DiscordEmoji.FromUnicode(client, emoji.EmojiName);

                if (config.Starboard.Enable)
                {
                    var channel = eventargs.Channel.Guild.Channels.First(x => x.Key == (ulong)config.Starboard.ChannelId).Value;
                    if (db.StarDatas.Any(x => (ulong)x.MessageId == eventargs.Message.Id))
                    {
                        await (await channel.GetMessageAsync((ulong)db.StarDatas.First(x => (ulong)x.MessageId == eventargs.Message.Id).StarboardMessageId)).DeleteAsync();
                        db.StarDatas.RemoveRange(db.StarDatas.Where(x => (ulong)x.MessageId == eventargs.Message.Id));
                        await db.SaveChangesAsync();
                    }
                }
            }
        }

        private static DiscordMessageBuilder buildMessage(DiscordMessage message, DiscordEmoji emoji, int count)
        {
            var embed = new DiscordEmbedBuilder()
                .WithAuthor($"{message.Author.Username}#{message.Author.Discriminator}",
                iconUrl: (string.IsNullOrEmpty(message.Author.AvatarHash) ? message.Author.DefaultAvatarUrl : message.Author.AvatarUrl))
                .WithDescription(message.Content.Truncate(800, "..."))
                .WithFooter($"ID: {message.Id}")
                .WithTimestamp(message.Id);

            // This is shit code kek
            if (message.Attachments.Any(x => x.Url.ToLower().EndsWith(".jpg") || x.Url.ToLower().EndsWith(".png")
             || x.Url.ToLower().EndsWith(".jpeg") || x.Url.ToLower().EndsWith(".gif")))
                embed.WithImageUrl(message.Attachments.First(x => x.Url.ToLower().EndsWith(".jpg") || x.Url.ToLower().EndsWith(".png")
            || x.Url.ToLower().EndsWith(".jpeg") || x.Url.ToLower().EndsWith(".gif")).Url);

            var emotename = emoji.GetDiscordName().Replace(":", "");
            emotename = emotename.EndsWith('s') ? emotename : count > 1 ? emotename + "s" : emotename;

            if(message.ReferencedMessage != null)
            {
                var refContent = message.ReferencedMessage.Content.Truncate(200, "...").Replace(")[", "​)[");

                if(string.IsNullOrEmpty(refContent))
                {
                    refContent = "_< no text >_";
                }

                embed.AddField("↳ Reply to", $"{(message.ReferencedMessage.Attachments.Count() > 0? "🖼 " : "")}" +
                    $"{message.ReferencedMessage.Author.Username}: {refContent} ([Jump]({message.ReferencedMessage.JumpLink}))");
            }

            var messageBuilder = new DiscordMessageBuilder()
                .AddEmbed(embed)
                .WithContent($"{emoji} {count} {emotename} in {message.Channel.Mention}");

            messageBuilder.AddComponents(new DiscordLinkButtonComponent(message.JumpLink.ToString(), "Go to message"));

            return messageBuilder;
        }
    }
}
