using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Entities;
using ModCore.Logic;
using System;
using ModCore.Database;
using System.Linq;
using System.Text;

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

            if (settings.GlobalWarn.Enable)
            {
                if (settings.GlobalWarn.WarnLevel == GLobalWarnLevel.Warn)
                {
                    var embed = new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.MidnightBlue)
                    .WithTitle($"@{e.Member.Username}#{e.Member.Discriminator} - ID: {e.Member.Id}");

                    var BanString = new StringBuilder();
                    foreach (DatabaseBan ban in bans) BanString.Append($"[{ban.GuildId} - {ban.BanReason}] ");
                    embed.AddField("Bans", BanString.ToString());

                    await e.Guild.Owner.SendMessageAsync("", embed: embed);
                }
                else if (settings.GlobalWarn.WarnLevel == GLobalWarnLevel.Ban)
                {
                    await e.Guild.BanMemberAsync(e.Member, reason: "ModCore GlobalWarn previously recorded for this user, and GlobalWarnLevel set to **Ban**");
                }
            }
        }
    }
}
