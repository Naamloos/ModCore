using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Database;
using ModCore.Extensions.AsyncListeners.Attributes;
using ModCore.Extensions.AsyncListeners.Enums;
using ModCore.Utils;
using ModCore.Utils.Extensions;
using System.Threading.Tasks;

namespace ModCore.Listeners
{
    public class Logging
    {
        [AsyncListener(EventType.GuildMemberUpdated)]
        public static async Task LogNickames(GuildMemberUpdateEventArgs eventargs, DatabaseContextBuilder database)
        {
            using var db = database.CreateContext();
            var cfg = eventargs.Guild.GetGuildSettings(db);
            if (cfg == null)
                return;
            DiscordChannel channel = eventargs.Guild.GetChannel(cfg.Logging.ChannelId);
            if (channel == null)
                return;

            if (cfg.Logging.EditLog_Enable)
            {
                if(eventargs.NicknameBefore != eventargs.NicknameAfter)
                {
                    var embed = new DiscordEmbedBuilder()
                    .WithTitle("Member Nickname Changed")
                    .WithDescription($"<@{eventargs.Member.Id}>")
                    .WithAuthor($"{eventargs.Member.DisplayName}",
                        iconUrl: eventargs.Member.GetAvatarUrl(DSharpPlus.ImageFormat.Auto))
                    .AddField("Original Nickname", eventargs.NicknameBefore ?? eventargs.Member.Username)
                    .AddField("New Nickname", eventargs.NicknameAfter ?? eventargs.Member.Username)
                    .WithColor(DiscordColor.Sienna);
                    await channel.ElevatedMessageAsync(embed);
                }
                return;
            }
            if(cfg.Logging.AvatarLog_Enable)
            {
                if (!string.IsNullOrEmpty(eventargs.AvatarHashAfter))
                {
                    var embed = new DiscordEmbedBuilder()
                        .WithTitle("Member Avatar Updated")
                        .WithDescription(eventargs.Member.Mention)
                        .WithAuthor($"{eventargs.Member.DisplayName}",
                            iconUrl: eventargs.Member.GetAvatarUrl(DSharpPlus.ImageFormat.Auto))
                        .WithImageUrl(eventargs.Member.GetAvatarUrl(DSharpPlus.ImageFormat.Auto))
                        .WithColor(DiscordColor.Sienna);
                    await channel.ElevatedMessageAsync(embed);
                }
            }
        }

        [AsyncListener(EventType.InviteCreate)]
        public static async Task InviteUpdates(InviteCreateEventArgs eventargs, DatabaseContextBuilder database)
        {
            using var db = database.CreateContext();
            var cfg = eventargs.Guild.GetGuildSettings(db);
            DiscordChannel channel = eventargs.Guild.GetChannel(cfg.Logging.ChannelId);
            if (channel == null)
                return;

            if (cfg.Logging.InviteLog_Enable)
            {
                var embed = new DiscordEmbedBuilder()
                .WithTitle("Invite Created")
                .WithAuthor($"{eventargs.Invite.Inviter.Username}",
                    iconUrl: eventargs.Invite.Inviter.GetAvatarUrl(DSharpPlus.ImageFormat.Auto))
                .AddField("Invite", eventargs.Invite.ToString())
                .AddField("Channel", eventargs.Channel.Mention)
                .WithColor(DiscordColor.Goldenrod);
                await channel.ElevatedMessageAsync(embed);
            }
        }
    }
}
