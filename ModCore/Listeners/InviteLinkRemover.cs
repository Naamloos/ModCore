using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Logic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModCore.Listeners
{
    public static class InviteLinkRemover
    {
        public static Regex InviteRegex { get; } = new Regex(@"discord(?:\.gg|app\.com\/invite)\/([\w\-]+)", RegexOptions.Compiled);
        public static ConcurrentDictionary<string, DiscordInvite> InviteCache { get; } = new ConcurrentDictionary<string, DiscordInvite>();

        [AsyncListener(EventTypes.MessageCreated)]
        public static async Task RemoveInviteLinks(Bot bot, MessageCreateEventArgs e)
        {
            if (e.Channel.Guild == null)
                return;

            var db = bot.Database.CreateContext();

            var cfg = e.Guild.GetGuildSettings(db);
            if (cfg == null)
                return;

            var ib = cfg.InviteBlocker;
            if (!ib.Enable)
                return;

            var ms = InviteRegex.Matches(e.Message.Content);
            if (!ms.Any())
                return;

            if (ib.ExemptUserIds.Contains(e.Message.Author.Id))
                return;

            var mbr = e.Message.Author as DiscordMember;
            if (mbr != null && mbr.Roles.Select(xr => xr.Id).Intersect(ib.ExemptRoleIds).Any())
                return;

            foreach (Match m in ms)
            {
                var invk = m.Groups[1].Value;
                if (!InviteCache.TryGetValue(invk, out var inv))
                {
                    inv = await bot.Client.GetInviteByCodeAsync(invk);
                    InviteCache.TryAdd(invk, inv);
                }

                if (ib.ExemptInviteGuildIds.Contains(inv.Guild.Id))
                    continue;

                #warning INCREMENT BAN THRESHOLD
                await e.Message.DeleteAsync("Discovered invite and deleted message");
                break;
            }
        }
    }
}