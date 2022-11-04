using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Enums;
using ModCore.Utils;
using ModCore.Utils.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Listeners
{
    public class UserUpdate
    {
        public static async Task<bool> IsInGuild(DiscordUser user, DiscordGuild guild)
        {
            try
            {
                var member = await guild.GetMemberAsync(user.Id);
                return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        [AsyncListener(EventType.PresenceUpdated)]
        public static async Task PresenceUpdated(PresenceUpdateEventArgs eventargs, DatabaseContextBuilder database, SharedData sharedData)
        {
            // TODO fix me
            var db = database.CreateContext();
            var loggers = db.GuildConfig.Where(x => x.GetSettings().LogUpdates).Select(x => x.GuildId);

            var guilds = sharedData?.ModCore?.Shards?.SelectMany(x => x.Client?.Guilds?.Where(y => y.Value.GetGuildSettings(db)?.LogUpdates ?? false));
            if (guilds != null)
                foreach (var g in guilds.Select(x => x.Value))
                {
                    if (await IsInGuild(eventargs.UserAfter, g))
                    {
                        var guildsettings = g.GetGuildSettings(db);
                        var log = g.GetChannel(guildsettings.UpdateChannel);

                        var embed = new DiscordEmbedBuilder()
                            .WithTitle("User Updated:")
                            .WithDescription($"{eventargs.UserAfter.Username}#{eventargs.UserAfter.Discriminator}");

                        if (eventargs.UserAfter.Username != eventargs.UserBefore.Username)
                            embed.AddField("Changed username", $"{eventargs.UserBefore.Username} to {eventargs.UserAfter.Username}");

                        if (eventargs.UserAfter.Discriminator != eventargs.UserBefore.Discriminator)
                            embed.AddField("Changed discriminator", $"{eventargs.UserBefore.Discriminator} to {eventargs.UserAfter.Discriminator}");

                        if (eventargs.UserAfter.AvatarUrl != eventargs.UserBefore.AvatarUrl)
                            embed.AddField("Changed avatar", $"[Old Avatar]({eventargs.UserBefore.AvatarUrl})" +
                                $"\nNote: this link may 404 later due to cache invalidation");

                        if (eventargs.UserAfter.IsBot != eventargs.UserBefore.IsBot)
                            embed.AddField("Magically transformed between bot form and human form",
                                $"Wait, what the fuck this isn't possible");

                        // TODO ModCore color scheme
                        embed.WithColor(new DiscordColor("#089FDF"));

                        embed.WithThumbnail(eventargs.UserAfter.AvatarUrl);
                        if (embed.Fields.Count > 0)
                            await log.SendMessageAsync(embed: embed);
                    }
                }
        }

        [AsyncListener(EventType.GuildMemberUpdated)]
        public static async Task MemberUpdated(GuildMemberUpdateEventArgs eventargs, DatabaseContextBuilder database)
        {
            var db = database.CreateContext();
            var guildsettings = eventargs.Guild.GetGuildSettings(db);

            if(guildsettings == null)
            {
                return;
            }

            if (guildsettings.LogUpdates)
            {
                var log = eventargs.Guild.GetChannel(guildsettings.UpdateChannel);

                var embed = new DiscordEmbedBuilder()
                    .WithTitle("Member Updated:")
                    .WithDescription($"{eventargs.Member.Username}#{eventargs.Member.Discriminator}");

                // TODO ModCore color scheme
                embed.WithColor(new DiscordColor("#089FDF"));

                embed.WithThumbnail(eventargs.Member.AvatarUrl);

                if (eventargs.NicknameAfter != eventargs.NicknameBefore)
                {
                    var after = eventargs.Member.Username;
                    var before = eventargs.Member.Username;

                    if (!string.IsNullOrEmpty(eventargs.NicknameAfter))
                        after = eventargs.NicknameAfter;
                    if (!string.IsNullOrEmpty(eventargs.NicknameBefore))
                        before = eventargs.NicknameBefore;

                    embed.AddField("Nickname update", $"{before} to {after}");
                }

                if (eventargs.RolesAfter != eventargs.RolesBefore)
                {
                    var added = eventargs.RolesAfter.Where(x => !eventargs.RolesBefore.Contains(x));
                    var removed = eventargs.RolesBefore.Where(x => !eventargs.RolesAfter.Contains(x));

                    if (added.Count() > 0)
                        embed.AddField("Added roles", string.Join(", ", added.Select(x => x.Name)));

                    if (removed.Count() > 0)
                        embed.AddField("Removed roles", string.Join(", ", removed.Select(x => x.Name)));
                }

                await log.SendMessageAsync(embed: embed);
            }
        }
    }
}