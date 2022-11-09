using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Database;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Enums;
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

            if (cfg.Logging.NickameLog_Enable)
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
                    .AddField("IDs", $"```ini\nUser = {eventargs.Member.Id}\n```")
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
                        .WithColor(DiscordColor.Sienna)
                        .AddField("IDs", $"```ini\nUser = {eventargs.Member.Id}\n```");
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
                var inv = eventargs.Invite;
                var embed = new DiscordEmbedBuilder()
                .WithTitle("Invite Created")
                .WithDescription($"Max Uses: {inv.MaxUses}\nMax Age: {inv.MaxAge}\nTemporary membership: {inv.IsTemporary}\nExpires At:{inv.ExpiresAt}\nCreated At:{inv.CreatedAt}")
                .WithAuthor($"{eventargs.Invite.Inviter.Username}",
                    iconUrl: eventargs.Invite.Inviter.GetAvatarUrl(DSharpPlus.ImageFormat.Auto))
                .AddField("Invite", eventargs.Invite.ToString())
                .AddField("Channel", eventargs.Channel.Mention)
                .WithColor(DiscordColor.Goldenrod)
                .AddField("IDs", $"```ini\nUser = {eventargs.Invite.Inviter.Id}\nChannel = {eventargs.Channel.Id}```");
                await channel.ElevatedMessageAsync(embed);
            }
        }
    }
}
