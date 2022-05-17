using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Logic;
using ModCore.Logic.Extensions;
using System.Threading.Tasks;

namespace ModCore.Listeners
{
    public class Logging
    {
        [AsyncListener(EventTypes.GuildMemberUpdated)]
        public static async Task CheckLevelUpdates(ModCoreShard bot, GuildMemberUpdateEventArgs eventargs)
        {
            using var db = bot.Database.CreateContext();
            var cfg = eventargs.Guild.GetGuildSettings(db);
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
            }
            if(cfg.Logging.AvatarLog_Enable)
            {
                if (eventargs.AvatarHashBefore != eventargs.AvatarHashAfter)
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

        [AsyncListener(EventTypes.InviteCreate)]
        public static async Task InviteUpdates(ModCoreShard bot, InviteCreateEventArgs eventargs)
        {
            using var db = bot.Database.CreateContext();
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
