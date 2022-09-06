using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Humanizer;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Extensions.AsyncListeners.Attributes;
using ModCore.Extensions.AsyncListeners.Enums;
using ModCore.Utils;
using ModCore.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Listeners
{
    public static class MessageSnipe
    {
        [AsyncListener(EventType.MessageDeleted)]
        public static async Task MessageSniped(MessageDeleteEventArgs eventargs, SharedData sharedData, DatabaseContextBuilder database)
        {
            await Task.Yield();

            if((!string.IsNullOrEmpty(eventargs.Message?.Content) || eventargs.Message.Embeds.Count > 0) && !eventargs.Message.Author.IsBot)
            {
                if (sharedData.DeletedMessages.ContainsKey(eventargs.Channel.Id))
                {
                    sharedData.DeletedMessages[eventargs.Channel.Id] = eventargs.Message;
                }
                else
                {
                    sharedData.DeletedMessages.TryAdd(eventargs.Channel.Id, eventargs.Message);
                }
            }

            if (eventargs.Message == null)
                return;

            using var db = database.CreateContext();
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
                        .WithColor(DiscordColor.Orange)
                        .AddField("IDs", $"```ini\nUser = {eventargs.Message.Author.Id}\nChannel = {eventargs.Channel.Id}\nMessage = {eventargs.Message.Id}```");
                await channel.ElevatedMessageAsync(embed);
            }
        }

        [AsyncListener(EventType.MessageUpdated)]
        public static async Task MessageEdited(MessageUpdateEventArgs eventargs, SharedData sharedData, DatabaseContextBuilder database)
        {
            await Task.Yield();

            if ((!string.IsNullOrEmpty(eventargs.MessageBefore?.Content) || eventargs.Message.Embeds.Count > 0) && !eventargs.Message.Author.IsBot)
            {
                if (sharedData.EditedMessages.ContainsKey(eventargs.Channel.Id))
                {
                    sharedData.EditedMessages[eventargs.Channel.Id] = eventargs.MessageBefore;
                }
                else
                {
                    sharedData.EditedMessages.TryAdd(eventargs.Channel.Id, eventargs.MessageBefore);
                }
            }

            using var db = database.CreateContext();
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
                        .WithColor(DiscordColor.Orange)
                        .AddField("IDs", $"```ini\nUser = {eventargs.Message.Author.Id}\nChannel = {eventargs.Channel.Id}\nMessage = {eventargs.Message.Id}```");

                var msg = new DiscordMessageBuilder()
                    .WithEmbed(embed)
                    .AddComponents(new DiscordLinkButtonComponent(eventargs.Message.JumpLink.ToString(), "Go to message"));
                await channel.ElevatedMessageAsync(embed);
            }
        }
    }
}
