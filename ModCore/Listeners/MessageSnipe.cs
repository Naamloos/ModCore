using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Humanizer;
using Microsoft.Extensions.Caching.Memory;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Enums;
using ModCore.Utils.Extensions;
using System;
using System.Threading.Tasks;

namespace ModCore.Listeners
{
    public static class MessageSnipe
    {
        [AsyncListener(EventType.MessageDeleted)]
        public static async Task MessageSniped(MessageDeleteEventArgs eventargs, SharedData sharedData, DatabaseContextBuilder database, IMemoryCache cache)
        {
            await Task.Yield();

            if (eventargs.Message == null)
                return;
            if (eventargs.Message.WebhookMessage)
                return;

            if (((!string.IsNullOrEmpty(eventargs.Message?.Content)) || eventargs.Message.Attachments.Count > 0) && !eventargs.Message.Author.IsBot)
            {
                cache.Set($"snipe_{eventargs.Channel.Id}", eventargs.Message, TimeSpan.FromHours(12));
            }

            await using var db = database.CreateContext();
            var cfg = eventargs.Guild.GetGuildSettings(db);
            if (cfg.Logging.EditLog_Enable)
            {
                var channel = eventargs.Guild.GetChannel(cfg.Logging.ChannelId);
                if (channel == null)
                    return;
                if (eventargs.Message != null && eventargs.Message.Author != null)
                {
                    var embed = new DiscordEmbedBuilder()
                            .WithTitle("Message Deleted")
                            .WithAuthor($"{eventargs.Message.Author.Username}",
                                iconUrl: eventargs.Message.Author.GetAvatarUrl(DSharpPlus.ImageFormat.Auto))
                            .AddField("Content", string.IsNullOrEmpty(eventargs.Message?.Content) ? eventargs.Message.Content.Truncate(1000) : "Original Content Unknown.")
                            .AddField("Channel", eventargs.Message.Channel.Mention)
                            .WithColor(DiscordColor.Orange)
                            .AddField("IDs", $"```ini\nUser = {eventargs.Message.Author.Id}\nChannel = {eventargs.Channel.Id}\nMessage = {eventargs.Message.Id}```");
                    await channel.ElevatedMessageAsync(embed);
                }
            }
        }

        [AsyncListener(EventType.MessageUpdated)]
        public static async Task MessageEdited(MessageUpdateEventArgs eventargs, SharedData sharedData, DatabaseContextBuilder database, IMemoryCache cache)
        {
            if (eventargs.Message == null)
                return;
            if (eventargs.Message.WebhookMessage)
                return;

            await Task.Yield();

            if (((!string.IsNullOrEmpty(eventargs.MessageBefore?.Content)) || eventargs.MessageBefore.Attachments.Count > 0) && !eventargs.Message.Author.IsBot)
            {
                cache.Set($"esnipe_{eventargs.Channel.Id}", eventargs.MessageBefore, TimeSpan.FromHours(12));
            }

            await using var db = database.CreateContext();
            var cfg = eventargs.Guild.GetGuildSettings(db);
            if(cfg != null && cfg.Logging.EditLog_Enable)
            {
                var channel = eventargs.Guild.GetChannel(cfg.Logging.ChannelId);
                if (channel == null)
                    return;

                if (eventargs.Message.Content != eventargs.MessageBefore.Content)
                {
                    var embed = new DiscordEmbedBuilder()
                            .WithTitle("Message Edited")
                            .WithAuthor($"{eventargs.Message.Author.Username}",
                                iconUrl: eventargs.Author.GetAvatarUrl(DSharpPlus.ImageFormat.Auto))
                            .AddField("Original Message", string.IsNullOrEmpty(eventargs.Message?.Content) ? eventargs.MessageBefore.Content.Truncate(1000) : "Original Content Unknown.")
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
}
