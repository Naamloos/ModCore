using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Entities;
using ModCore.Logic;
using System;

namespace ModCore.Listeners
{
    public static class JoinLog
    {
        [AsyncListener(EventTypes.GuildMemberAdded)]
        public static async Task LogNewMember(ModCoreShard bot, GuildMemberAddEventArgs e)
        {
            GuildSettings cfg = null;
            using (var db = bot.Database.CreateContext())
                cfg = e.Guild.GetGuildSettings(db);
            if (cfg.JoinLog.Enable)
            {
                var m = e.Member;
                var c = (DiscordChannel)null;
                try
                {
                    c = e.Guild.GetChannel((ulong)cfg.JoinLog.ChannelId);
                }
                catch (Exception)
                {
                    return;
                }
                var embed = new DiscordEmbedBuilder()
                    .WithTitle("New member joined")
                    .WithDescription($"ID: ({m.Id})")
                    .WithAuthor($"{m.Username}#{m.Discriminator}", icon_url: string.IsNullOrEmpty(m.AvatarHash) ? m.DefaultAvatarUrl : m.AvatarUrl)
                    .AddField("Join Date", $"{m.JoinedAt.DateTime.ToString()}")
                    .AddField("Register Date", $"{m.CreationTimestamp.DateTime.ToString()}")
                    .WithColor(DiscordColor.Green);
                await c.SendMessageAsync(embed: embed);
            }
        }

        [AsyncListener(EventTypes.GuildMemberRemoved)]
        public static async Task LogLeaveMember(ModCoreShard bot, GuildMemberRemoveEventArgs e)
        {
            GuildSettings cfg = null;
            using (var db = bot.Database.CreateContext())
                cfg = e.Guild.GetGuildSettings(db);
            if (cfg.JoinLog.Enable)
            {
                var m = e.Member;
                var c = (DiscordChannel)null;
                try
                {
                    c = e.Guild.GetChannel((ulong)cfg.JoinLog.ChannelId);
                }
                catch (Exception)
                {
                    return;
                }
                var embed = new DiscordEmbedBuilder()
                    .WithTitle("Member left")
                    .WithDescription($"ID: ({m.Id})")
                    .WithAuthor($"{m.Username}#{m.Discriminator}", icon_url: string.IsNullOrEmpty(m.AvatarHash) ? m.DefaultAvatarUrl : m.AvatarUrl)
                    .AddField("Join Date", $"{m.JoinedAt.DateTime.ToString()}")
                    .AddField("Register Date", $"{m.CreationTimestamp.DateTime.ToString()}")
                    .WithColor(DiscordColor.Red);
                await c.SendMessageAsync(embed: embed);
            }
        }
    }
}
