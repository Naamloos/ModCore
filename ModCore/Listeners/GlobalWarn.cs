using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Entities;
using ModCore.Logic;
using System;
using ModCore.Database;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace ModCore.Listeners
{
    public static class GlobalWarn
    {
        [AsyncListener(EventTypes.GuildMemberAdded)]
        public static async Task CheckNewMember(ModCoreShard bot, GuildMemberAddEventArgs e)
        {
            GuildSettings settings;
            DatabaseBan[] bans;
            using (var db = bot.Database.CreateContext())
            {
                if (!db.Bans.Any(x => x.UserId == (long)e.Member.Id))
                {
                    return;
                }
                bans = db.Bans.Where(x => x.UserId == (long)e.Member.Id).ToArray();
                settings = e.Guild.GetGuildSettings(db) ?? new GuildSettings();
            }

            var prevowns = new List<ulong>();
            int count = 0;
            var guilds = ModCore.Shards.SelectMany(x => x.Client.Guilds.Values);
            foreach (var b in bans)
            {
                var g = guilds.First(x => x.Id == (ulong)b.GuildId);
                if (prevowns.Contains(g.Owner.Id))
                    continue;
                count++;
                prevowns.Add(g.Owner.Id);
            }
            if (settings.GlobalWarn.Enable && count > 2)
            {
                var embed = new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.MidnightBlue)
                    .WithTitle($"@{e.Member.Username}#{e.Member.Discriminator} - ID: {e.Member.Id}");

                var BanString = new StringBuilder();
                foreach (DatabaseBan ban in bans) BanString.Append($"[{ban.GuildId} - {ban.BanReason}] ");
                embed.AddField("Bans", BanString.ToString());

                if (settings.GlobalWarn.WarnLevel == GlobalWarnLevel.Owner)
                {
                    await e.Guild.Owner.SendMessageAsync("", embed: embed);
                }
                else if (settings.GlobalWarn.WarnLevel == GlobalWarnLevel.JoinLog)
                {
                    await e.Guild.Channels.First(x => x.Id == (ulong)settings.JoinLog.ChannelId).SendMessageAsync(embed: embed);
                }
            }
        }
    }
}
