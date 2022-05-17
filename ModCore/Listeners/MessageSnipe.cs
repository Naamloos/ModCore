using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Humanizer;
using ModCore.Logic;
using ModCore.Logic.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Listeners
{
    public static class MessageSnipe
    {
        [AsyncListener(EventTypes.MessageDeleted)]
        public static async Task MessageSniped(ModCoreShard bot, MessageDeleteEventArgs eventargs)
        {
            await Task.Yield();

            if((!string.IsNullOrEmpty(eventargs.Message?.Content) || eventargs.Message.Embeds.Count > 0) && !eventargs.Message.Author.IsBot)
            {
                if (bot.SharedData.DeletedMessages.ContainsKey(eventargs.Channel.Id))
                {
                    bot.SharedData.DeletedMessages[eventargs.Channel.Id] = eventargs.Message;
                }
                else
                {
                    bot.SharedData.DeletedMessages.TryAdd(eventargs.Channel.Id, eventargs.Message);
                }
            }

            if (eventargs.Message == null)
                return;

            using var db = bot.Database.CreateContext();
            var cfg = eventargs.Guild.GetGuildSettings(db);
            if (cfg.Logging.EditLog_Enable)
            {
                var channel = eventargs.Guild.GetChannel(cfg.Logging.ChannelId);
                if (channel == null)
                    return;
                var embed = new DiscordEmbedBuilder()
                        .WithTitle("Message Deleted")
                        .WithAuthor($"{eventargs.Message.Author.Username}",
                            iconUrl: eventargs.Message.Author.GetAvatarUrl(DSharpPlus.ImageFormat.Auto))
                        .AddField("Content", eventargs.Message != null ? eventargs.Message.Content.Truncate(1000) : "Original Content Unknown.")
                        .AddField("Channel", eventargs.Message.Channel.Mention)
                        .WithColor(DiscordColor.Orange);
                await channel.ElevatedMessageAsync(embed);
            }
        }

        [AsyncListener(EventTypes.MessageUpdated)]
        public static async Task MessageEdited(ModCoreShard bot, MessageUpdateEventArgs eventargs)
        {
            await Task.Yield();

            if ((!string.IsNullOrEmpty(eventargs.MessageBefore?.Content) || eventargs.Message.Embeds.Count > 0) && !eventargs.Message.Author.IsBot)
            {
                if (bot.SharedData.EditedMessages.ContainsKey(eventargs.Channel.Id))
                {
                    bot.SharedData.EditedMessages[eventargs.Channel.Id] = eventargs.MessageBefore;
                }
                else
                {
                    bot.SharedData.EditedMessages.TryAdd(eventargs.Channel.Id, eventargs.MessageBefore);
                }
            }

            using var db = bot.Database.CreateContext();
            var cfg = eventargs.Guild.GetGuildSettings(db);
            if(cfg.Logging.EditLog_Enable)
            {
                var channel = eventargs.Guild.GetChannel(cfg.Logging.ChannelId);
                if (channel == null)
                    return;
                var embed = new DiscordEmbedBuilder()
                        .WithTitle("Message Edited")
                        .WithAuthor($"{eventargs.Message.Author.Username}",
                            iconUrl: eventargs.Author.GetAvatarUrl(DSharpPlus.ImageFormat.Auto))
                        .AddField("Original Message", eventargs.MessageBefore != null ? eventargs.MessageBefore.Content.Truncate(1000) : "Original Content Unknown.")
                        .AddField("Edited Message", eventargs.Message.Content.Truncate(1000))
                        .AddField("Channel", eventargs.Message.Channel.Mention)
                        .WithColor(DiscordColor.Orange);
                await channel.ElevatedMessageAsync(embed);
            }
        }
    }
}
