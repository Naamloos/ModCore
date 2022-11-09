using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Database;
using ModCore.Database.JsonEntities;
using ModCore.Entities;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Enums;
using ModCore.Utils;
using ModCore.Utils.Extensions;

namespace ModCore.Listeners
{
    public static class Linkfilter
    {
        public static Regex InviteRegex { get; } =
            new Regex(@"discord(?:\.gg|app\.com\/invite)\/([\w\-]+)", RegexOptions.Compiled);

        public static ConcurrentDictionary<string, DiscordInvite> InviteCache { get; } =
            new ConcurrentDictionary<string, DiscordInvite>();

        [AsyncListener(EventType.MessageCreated)]
        public static async Task RemoveSuspiciousLinks(MessageCreateEventArgs eventargs, DatabaseContextBuilder database, DiscordClient client)
        {
            if (eventargs.Author == null || eventargs.Channel == null)
                return;

            if (eventargs.Message.WebhookMessage)
                return;

            if ((eventargs.Channel.PermissionsFor(eventargs.Author as DiscordMember) & Permissions.ManageMessages) != 0) return;

            if (eventargs.Channel.Guild == null)
                return;

            GuildSettings config;
            using (var db = database.CreateContext())
                config = eventargs.Guild.GetGuildSettings(db);
            if (config == null)
                return;

            var lfSettings = config.Linkfilter;

            if (lfSettings.ExemptUserIds.Contains(eventargs.Message.Author.Id))
                return;

            if (eventargs.Message.Author is DiscordMember member &&
                member.Roles.Select(xr => xr.Id).Intersect(lfSettings.ExemptRoleIds).Any())
                return;

            if (lfSettings.BlockInviteLinks)
                await FindAndPurgeInvites(client, eventargs, lfSettings);
        }

        private static async Task FindAndPurgeInvites(DiscordClient client, MessageCreateEventArgs eventargs,
            GuildLinkfilterSettings linkfilter)
        {
            var matches = InviteRegex.Matches(eventargs.Message.Content);
            if (!matches.Any())
                return;

            foreach (Match match in matches)
            {
                var invk = match.Groups[1].Value;
                if (!InviteCache.TryGetValue(invk, out var inv))
                {
                    inv = await client.GetInviteByCodeAsync(invk);
                    InviteCache.TryAdd(invk, inv);
                }

                if (linkfilter.ExemptInviteGuildIds.Contains(inv.Guild.Id))
                    continue;

                await eventargs.Message.DeleteAsync($"Discovered invite <{inv.Code}> and deleted message");
                break;
            }
        }
    }
}