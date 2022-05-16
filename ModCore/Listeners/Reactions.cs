using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Humanizer;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;
using ModCore.Logic.Extensions;

namespace ModCore.Listeners
{
    public class Reactions
    {
        [AsyncListener(EventTypes.MessageReactionAdded)]
        public static async Task ReactionAdd(ModCoreShard bot, MessageReactionAddEventArgs eventargs)
        {
            GuildSettings config = null;
            using (var db = bot.Database.CreateContext())
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
                    discordemoji = DiscordEmoji.FromGuildEmote(bot.Client, (ulong)emoji.EmojiId);
                else
                    discordemoji = DiscordEmoji.FromUnicode(bot.Client, emoji.EmojiName);

                if (!config.Starboard.AllowNSFW && eventargs.Channel.IsNSFW)
                    return;

                if (config.Starboard.Enable && eventargs.Emoji == discordemoji)
                {
                    long starboardmessageid = 0;
                    var channel = eventargs.Channel.Guild.Channels.First(x => x.Key == (ulong)config.Starboard.ChannelId).Value;
                    if (channel.Id == eventargs.Channel.Id) // star on starboard entry
                    {
                        /*if (db.StarDatas.Any(x => (ulong)x.StarboardMessageId == e.Message.Id))
                        {
                            var other = db.StarDatas.First(x => (ulong)x.StarboardMessageId == e.Message.Id);
                            var count = db.StarDatas.Count(x => (ulong)x.StarboardMessageId == e.Message.Id);
                            if (!db.StarDatas.Any(x => x.MessageId == other.MessageId && x.StargazerId == (long)e.User.Id))
                            {
                                var chn = await e.Client.GetChannelAsync((ulong)other.ChannelId);
                                var msg = await chn.GetMessageAsync((ulong)other.MessageId);

                                if (msg.Author.Id == e.User.Id || e.User.IsBot)
                                    return;

                                var d = await (await c.GetMessageAsync((ulong)other.StarboardMessageId)).ModifyAsync($"{e.Emoji}: {count + 1} ({msg.Id}) in {msg.Channel.Mention}", embed: BuildMessageEmbed(msg));
                                sbmid = (long)d.Id;
                                await db.StarDatas.AddAsync(new DatabaseStarData
                                {
                                    ChannelId = other.ChannelId,
                                    GuildId = (long)e.Channel.Guild.Id,
                                    MessageId = other.MessageId,
                                    AuthorId = other.AuthorId,
                                    StarboardMessageId = sbmid,
                                    StargazerId = (long)e.User.Id,
                                });
                                await db.SaveChangesAsync();
                            }
                        }*/

                    // Removing this behaviour for jump links.
                    }
                    else // star on actual message
                    {
                        if (eventargs.Message.Author.Id == eventargs.User.Id || eventargs.User.IsBot)
                            return;

                        if (db.StarDatas.Any(x => (ulong)x.MessageId == eventargs.Message.Id))
                        {
                            var count = db.StarDatas.Count(x => (ulong)x.MessageId == eventargs.Message.Id);

                            if (db.StarDatas.Any(x => (ulong)x.MessageId == eventargs.Message.Id && x.StarboardMessageId != 0))
                            {
                                var other = db.StarDatas.First(x => (ulong)x.MessageId == eventargs.Message.Id && x.StarboardMessageId != 0);
                                var message = await channel.GetMessageAsync((ulong)other.StarboardMessageId);

                                var starboardmessage = await message.ModifyAsync($"{eventargs.Emoji}: {count + 1} ({eventargs.Message.Id}) in {eventargs.Message.Channel.Mention}", embed: BuildMessageEmbed(eventargs.Message));
                                starboardmessageid = (long)starboardmessage.Id;
                            }
                            else
                            {
                                if(count + 1 >= config.Starboard.Minimum)
                                {
                                    // create msg
                                    var starboardmessage = await channel.ElevatedMessageAsync($"{eventargs.Emoji}: {count + 1} ({eventargs.Message.Id}) in {eventargs.Message.Channel.Mention}", embed: BuildMessageEmbed(eventargs.Message));
                                    starboardmessageid = (long)starboardmessage.Id;
                                }
                            }
                        }
                        else if (config.Starboard.Minimum <= 1)
                        {
                            var starboardmessage = await channel.ElevatedMessageAsync($"{eventargs.Emoji}: 1 ({eventargs.Message.Id}) in {eventargs.Message.Channel.Mention}", embed: BuildMessageEmbed(eventargs.Message));
                            starboardmessageid = (long)starboardmessage.Id;
                        }

                        await db.StarDatas.AddAsync(new DatabaseStarData
                        {
                            ChannelId = (long)eventargs.Channel.Id,
                            GuildId = (long)eventargs.Channel.Guild.Id,
                            MessageId = (long)eventargs.Message.Id,
                            AuthorId = (long)eventargs.Message.Author.Id,
                            StarboardMessageId = starboardmessageid,
                            StargazerId = (long)eventargs.User.Id,
                        });

                        // somebody once told me...
                        var allstars = db.StarDatas.Where(x => (ulong)x.MessageId == eventargs.Message.Id).ToList();
                        allstars.ForEach(x => x.StarboardMessageId = starboardmessageid);
                        db.StarDatas.UpdateRange(allstars);

                        await db.SaveChangesAsync();
                    }
                }
            }
        }

        [AsyncListener(EventTypes.MessageReactionRemoved)]
        public static async Task ReactionRemove(ModCoreShard bot, MessageReactionRemoveEventArgs eventargs)
        {
            GuildSettings config = null;
            using (var db = bot.Database.CreateContext())
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
                    discordemoji = DiscordEmoji.FromGuildEmote(bot.Client, (ulong)emoji.EmojiId);
                else
                    discordemoji = DiscordEmoji.FromUnicode(bot.Client, emoji.EmojiName);

                if (config.Starboard.Enable && eventargs.Emoji == discordemoji)
                {
                    var channel = eventargs.Channel.Guild.Channels.First(x => x.Key == (ulong)config.Starboard.ChannelId).Value;
                    if (channel.Id == eventargs.Channel.Id)
                    {
                        /*if (db.StarDatas.Any(x => (ulong)x.StarboardMessageId == e.Message.Id && (ulong)x.StargazerId == e.User.Id))
                        {
                            var star = db.StarDatas.First(x => (ulong)x.StarboardMessageId == e.Message.Id && (ulong)x.StargazerId == e.User.Id);
                            var count = db.StarDatas.Count(x => (ulong)x.StarboardMessageId == e.Message.Id);
                            var m = await c.GetMessageAsync((ulong)star.StarboardMessageId);
                            var chn = await e.Client.GetChannelAsync((ulong)star.ChannelId);
                            var msg = await chn.GetMessageAsync((ulong)star.MessageId);
                            if (count - 1 >= cfg.Starboard.Minimum)
                                await m.ModifyAsync($"{e.Emoji}: {count - 1} ({msg.Id}) in {msg.Channel.Mention}", embed: BuildMessageEmbed(msg));
                            else
                                await m.DeleteAsync();
                            db.StarDatas.Remove(star);
                            await db.SaveChangesAsync();
                        }*/
                        // Removing behaviour due to jump links
                    }
                    else
                    {
                        long newstarboardmessageid = 0;
                        if (db.StarDatas.Any(x => (ulong)x.MessageId == eventargs.Message.Id && (ulong)x.StargazerId == eventargs.User.Id && x.StarboardMessageId != 0))
                        {
                            var star = db.StarDatas.First(x => (ulong)x.MessageId == eventargs.Message.Id && (ulong)x.StargazerId == eventargs.User.Id);
                            var count = db.StarDatas.Count(x => (ulong)x.MessageId == eventargs.Message.Id);

                            if (db.StarDatas.Any(x => (ulong)x.MessageId == eventargs.Message.Id && (ulong)x.StargazerId == eventargs.User.Id && x.StarboardMessageId != 0))
                            {
                                var starboardmessageid = db.StarDatas.First(x => (ulong)x.MessageId == eventargs.Message.Id && (ulong)x.StargazerId == eventargs.User.Id && x.StarboardMessageId != 0)
                                    .StarboardMessageId;

                                var m = await channel.GetMessageAsync((ulong)starboardmessageid);
                                if (count - 1 >= config.Starboard.Minimum)
                                {
                                    await m.ModifyAsync($"{eventargs.Emoji}: {count - 1} ({eventargs.Message.Id}) in {eventargs.Message.Channel.Mention}", embed: BuildMessageEmbed(eventargs.Message));
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
                            var allstars = db.StarDatas.Where(x => (ulong)x.MessageId == eventargs.Message.Id).ToList();
                            allstars.ForEach(x => x.StarboardMessageId = newstarboardmessageid);
                            db.StarDatas.UpdateRange(allstars);

                            await db.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        [AsyncListener(EventTypes.MessageReactionsCleared)]
        public static async Task ReactionClear(ModCoreShard bot, MessageReactionsClearEventArgs eventargs)
        {
            GuildSettings config = null;
            using (var db = bot.Database.CreateContext())
            {
                config = eventargs.Channel.Guild.GetGuildSettings(db);
                if (config == null)
                    return;

                var emoji = config.Starboard.Emoji;
                DiscordEmoji discordemoji = null;
                if (emoji.EmojiId != 0)
                    discordemoji = DiscordEmoji.FromGuildEmote(bot.Client, (ulong)emoji.EmojiId);
                else
                    discordemoji = DiscordEmoji.FromUnicode(bot.Client, emoji.EmojiName);

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

        public static DiscordEmbed BuildMessageEmbed(DiscordMessage message)
        {
            var embed = new DiscordEmbedBuilder()
                .WithAuthor($"{message.Author.Username}#{message.Author.Discriminator}",
				iconUrl: (string.IsNullOrEmpty(message.Author.AvatarHash) ? message.Author.DefaultAvatarUrl : message.Author.AvatarUrl))
                .WithDescription(message.Content.Truncate(1000) + $"\n\n[Jump to message]({message.JumpLink})");

            // This is shit code kek
            if (message.Attachments.Any(x => x.Url.ToLower().EndsWith(".jpg") || x.Url.ToLower().EndsWith(".png")
             || x.Url.ToLower().EndsWith(".jpeg") || x.Url.ToLower().EndsWith(".gif")))
                return embed.WithImageUrl(message.Attachments.First(x => x.Url.ToLower().EndsWith(".jpg") || x.Url.ToLower().EndsWith(".png")
            || x.Url.ToLower().EndsWith(".jpeg") || x.Url.ToLower().EndsWith(".gif")).Url).Build();

            return embed.Build();
        }
    }
}
