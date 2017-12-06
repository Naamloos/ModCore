using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Listeners
{
    public class Reactions
    {
        [AsyncListener(EventTypes.MessageReactionAdded)]
        public static async Task ReactionAdd(ModCoreShard bot, MessageReactionAddEventArgs e)
        {
            GuildSettings cfg = null;
            using (var db = bot.Database.CreateContext())
            {
                cfg = e.Channel.Guild.GetGuildSettings(db);
                if (cfg == null)
                    return;

                var emoji = cfg.Starboard.Emoji;
                DiscordEmoji em = null;
                if (emoji.EmojiId != 0)
                    em = DiscordEmoji.FromGuildEmote(e.Client, (ulong)emoji.EmojiId);
                else
                    em = DiscordEmoji.FromUnicode(e.Client, emoji.EmojiName);

                if (!cfg.Starboard.AllowNSFW && e.Channel.IsNSFW)
                    return;

                if (cfg.Starboard.Enable && e.Emoji == em)
                {
                    long sbmid = 0;
                    var c = e.Channel.Guild.Channels.First(x => x.Id == (ulong)cfg.Starboard.ChannelId);
                    if (c.Id == e.Channel.Id) // star on starboard entry
                    {
                        if (db.StarDatas.Any(x => (ulong)x.StarboardMessageId == e.Message.Id))
                        {
                            var other = db.StarDatas.First(x => (ulong)x.StarboardMessageId == e.Message.Id);
                            var count = db.StarDatas.Count(x => (ulong)x.StarboardMessageId == e.Message.Id);
                            if (!db.StarDatas.Any(x => x.MessageId == other.MessageId && x.StargazerId == (long)e.User.Id))
                            {
                                var chn = await e.Client.GetChannelAsync((ulong)other.ChannelId);
                                var msg = await chn.GetMessageAsync((ulong)other.MessageId);

                                if (msg.Author.Id == e.User.Id || e.User.IsBot)
                                    return;

                                var d = await (await c.GetMessageAsync((ulong)other.StarboardMessageId)).ModifyAsync($"{e.Emoji.ToString()}: {count + 1} ({msg.Id}) in {msg.Channel.Mention}", embed: BuildMessageEmbed(msg));
                                sbmid = (long)d.Id;
                                await db.StarDatas.AddAsync(new DatabaseStarData()
                                {
                                    ChannelId = other.ChannelId,
                                    GuildId = (long)e.Channel.Guild.Id,
                                    MessageId = other.MessageId,
                                    StarboardMessageId = sbmid,
                                    StargazerId = (long)e.User.Id,
                                });
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                    else // star on actual message
                    {
                        if (db.StarDatas.Any(x => (ulong)x.MessageId == e.Message.Id))
                        {
                            var other = db.StarDatas.First(x => (ulong)x.MessageId == e.Message.Id);
                            var count = db.StarDatas.Count(x => (ulong)x.MessageId == e.Message.Id);
                            var msg = await c.GetMessageAsync((ulong)other.StarboardMessageId);
                            if (msg.Author.Id == e.User.Id || e.User.IsBot)
                                return;

                            var d = await msg.ModifyAsync($"{e.Emoji.ToString()}: {count + 1} ({e.Message.Id}) in {e.Message.Channel.Mention}", embed: BuildMessageEmbed(e.Message));
                            sbmid = (long)d.Id;
                        }
                        else
                        {
                            var d = await c.SendMessageAsync($"{e.Emoji.ToString()}: 1 ({e.Message.Id}) in {e.Message.Channel.Mention}", embed: BuildMessageEmbed(e.Message));
                            sbmid = (long)d.Id;
                        }
                        await db.StarDatas.AddAsync(new DatabaseStarData()
                        {
                            ChannelId = (long)e.Channel.Id,
                            GuildId = (long)e.Channel.Guild.Id,
                            MessageId = (long)e.Message.Id,
                            StarboardMessageId = sbmid,
                            StargazerId = (long)e.User.Id,
                        });
                        await db.SaveChangesAsync();
                    }
                }
            }
        }

        [AsyncListener(EventTypes.MessageReactionRemoved)]
        public static async Task ReactionRemove(ModCoreShard bot, MessageReactionRemoveEventArgs e)
        {
            GuildSettings cfg = null;
            using (var db = bot.Database.CreateContext())
            {
                cfg = e.Channel.Guild.GetGuildSettings(db);
                if (cfg == null)
                    return;

                var emoji = cfg.Starboard.Emoji;
                DiscordEmoji em = null;
                if (emoji.EmojiId != 0)
                    em = DiscordEmoji.FromGuildEmote(e.Client, (ulong)emoji.EmojiId);
                else
                    em = DiscordEmoji.FromUnicode(e.Client, emoji.EmojiName);

                if (cfg.Starboard.Enable && e.Emoji == em)
                {
                    var c = e.Channel.Guild.Channels.First(x => x.Id == (ulong)cfg.Starboard.ChannelId);
                    if (c.Id == e.Channel.Id)
                    {
                        if (db.StarDatas.Any(x => (ulong)x.StarboardMessageId == e.Message.Id && (ulong)x.StargazerId == e.User.Id))
                        {
                            var star = db.StarDatas.First(x => (ulong)x.StarboardMessageId == e.Message.Id && (ulong)x.StargazerId == e.User.Id);
                            var count = db.StarDatas.Count(x => (ulong)x.StarboardMessageId == e.Message.Id);
                            var m = await c.GetMessageAsync((ulong)star.StarboardMessageId);
                            var chn = await e.Client.GetChannelAsync((ulong)star.ChannelId);
                            var msg = await chn.GetMessageAsync((ulong)star.MessageId);
                            if (count - 1 > 0)
                                await m.ModifyAsync($"{e.Emoji.ToString()}: {count - 1} ({msg.Id}) in {msg.Channel.Mention}", embed: BuildMessageEmbed(msg));
                            else
                                await m.DeleteAsync();
                            db.StarDatas.Remove(star);
                            await db.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        if (db.StarDatas.Any(x => (ulong)x.MessageId == e.Message.Id && (ulong)x.StargazerId == e.User.Id))
                        {
                            var star = db.StarDatas.First(x => (ulong)x.MessageId == e.Message.Id && (ulong)x.StargazerId == e.User.Id);
                            var count = db.StarDatas.Count(x => (ulong)x.MessageId == e.Message.Id);
                            var m = await c.GetMessageAsync((ulong)star.StarboardMessageId);
                            if (count - 1 > 0)
                                await m.ModifyAsync($"{e.Emoji.ToString()}: {count - 1} ({e.Message.Id}) in {e.Message.Channel.Mention}", embed: BuildMessageEmbed(e.Message));
                            else
                                await m.DeleteAsync();
                            db.StarDatas.Remove(star);
                            await db.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        [AsyncListener(EventTypes.MessageReactionsCleared)]
        public static async Task ReactionClear(ModCoreShard bot, MessageReactionsClearEventArgs e)
        {
            GuildSettings cfg = null;
            using (var db = bot.Database.CreateContext())
            {
                cfg = e.Channel.Guild.GetGuildSettings(db);
                if (cfg == null)
                    return;

                var emoji = cfg.Starboard.Emoji;
                DiscordEmoji em = null;
                if (emoji.EmojiId != 0)
                    em = DiscordEmoji.FromGuildEmote(e.Client, (ulong)emoji.EmojiId);
                else
                    em = DiscordEmoji.FromUnicode(e.Client, emoji.EmojiName);

                if (cfg.Starboard.Enable)
                {
                    var c = e.Channel.Guild.Channels.First(x => x.Id == (ulong)cfg.Starboard.ChannelId);
                    if (db.StarDatas.Any(x => (ulong)x.MessageId == e.Message.Id))
                    {
                        await (await c.GetMessageAsync((ulong)db.StarDatas.First(x => (ulong)x.MessageId == e.Message.Id).StarboardMessageId)).DeleteAsync();
                        db.StarDatas.RemoveRange(db.StarDatas.Where(x => (ulong)x.MessageId == e.Message.Id));
                        await db.SaveChangesAsync();
                    }
                }
            }
        }

        public static DiscordEmbed BuildMessageEmbed(DiscordMessage m)
        {
            var e = new DiscordEmbedBuilder()
                .WithAuthor($"{m.Author.Username}#{m.Author.Discriminator}",
                icon_url: (string.IsNullOrEmpty(m.Author.AvatarHash) ? m.Author.DefaultAvatarUrl : m.Author.AvatarUrl))
                .WithDescription(m.Content);

            // This is shit code kek
            if (m.Attachments.Any(x => x.Url.ToLower().EndsWith(".jpg") || x.Url.ToLower().EndsWith(".png")
             || x.Url.ToLower().EndsWith(".jpeg") || x.Url.ToLower().EndsWith(".gif")))
                return e.WithImageUrl(m.Attachments.First(x => x.Url.ToLower().EndsWith(".jpg") || x.Url.ToLower().EndsWith(".png")
            || x.Url.ToLower().EndsWith(".jpeg") || x.Url.ToLower().EndsWith(".gif")).Url).Build();

            return e.Build();
        }
    }
}
