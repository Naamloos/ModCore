using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.ModernEmbedBuilder;
using ModCore.Entities;
using ModCore.Logic;

namespace ModCore.Listeners
{
    public static class MessageLog
    {
        [AsyncListener(EventTypes.MessageDeleted)]
        public static async Task LogMessageDelete(ModCoreShard bot, MessageDeleteEventArgs e)
        {
            GuildSettings cfg = null;
            using (var db = bot.Database.CreateContext())
                cfg = e.Guild.GetGuildSettings(db);
            if (cfg.MessageLog.Enable && cfg.MessageLog.LogLevel >= MessageLogLevel.Delete && !e.Message.Author.IsBot)
            {
                if (!string.IsNullOrEmpty(cfg.Prefix))
                    if (e.Message.Content.StartsWith(cfg.Prefix))
                        return;
                var m = e.Message.Author;
                var c = (DiscordChannel)null;
                try
                {
                    c = e.Guild.GetChannel((ulong)cfg.MessageLog.ChannelId);
                }
                catch (Exception)
                {
                    return;
                }
                await new ModernEmbedBuilder
                {
                    Title = "Message Deleted",
                    Author = new DuckAuthor
                    {
                        Name = $"{m.Username}#{m.Discriminator}",
                        IconUrl = string.IsNullOrEmpty(m.AvatarHash) ? m.DefaultAvatarUrl : m.AvatarUrl
                    },
                    Fields =
                    {
                        new DuckField("Member", $"{e.Message.Author.Username}#{e.Message.Author.Discriminator} ({e.Message.Author.Id})", true),
                        new DuckField("Creation Timestamp", e.Message.CreationTimestamp.ToString(), true),
                        new DuckField("Deletion Timestamp", DateTime.Now.ToString(), true),
                        new DuckField("Content", !string.IsNullOrWhiteSpace(e.Message.Content) ? e.Message.Content : "-"),
                        new DuckField("Attachments", e.Message.Attachments.Any() ? string.Join("\n", e.Message.Attachments.Select(x => x.Url)) : "-")
                    },
                    Color = DiscordColor.DarkBlue
                }.Send(c);
            }
        }

        [AsyncListener(EventTypes.MessageUpdated)]
        public static async Task LogMessageEdit(ModCoreShard bot, MessageUpdateEventArgs e)
        {
            GuildSettings cfg = null;
            using (var db = bot.Database.CreateContext())
                cfg = e.Guild.GetGuildSettings(db) != null ? e.Guild.GetGuildSettings(db) : new GuildSettings();
            if (cfg.MessageLog.Enable && cfg.MessageLog.LogLevel >= MessageLogLevel.Edit && !e.Author.IsBot)
            {
                if (!string.IsNullOrEmpty(cfg.Prefix))
                    if (e.Message.Content.StartsWith(cfg.Prefix))
                        return;

                var m = e.Message.Author;
                var c = (DiscordChannel)null;
                try
                {
                    c = e.Guild.GetChannel((ulong)cfg.MessageLog.ChannelId);
                }
                catch (Exception)
                {
                    return;
                }
                await new ModernEmbedBuilder
                {
                    Title = "Message Edited",
                    Author = new DuckAuthor
                    {
                        Name = $"{m.Username}#{m.Discriminator}",
                        IconUrl = string.IsNullOrEmpty(m.AvatarHash) ? m.DefaultAvatarUrl : m.AvatarUrl
                    },
                    Fields =
                    {
                        new DuckField("Member", $"{e.Message.Author.Username}#{e.Message.Author.Discriminator} ({e.Message.Author.Id})", true),
                        new DuckField("Creation Timestamp", e.Message.CreationTimestamp.ToString(), true),
                        new DuckField("Edit Timestamp", DateTime.Now.ToString(), true),
                        new DuckField("Old Content", "WIP"),
                        new DuckField("New Content", !string.IsNullOrWhiteSpace(e.Message.Content) ? e.Message.Content : "-")
                    },
                    Color = DiscordColor.Blue
                }.Send(c);
            }
        }
    }
}
