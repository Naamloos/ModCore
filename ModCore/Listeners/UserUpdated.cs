using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Entities;
using ModCore.Logic;
using ModCore.Logic.Extensions;
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
        public static async Task<bool> IsInGuild(DiscordUser u, DiscordGuild g)
        {
            try
            {
                var m = await g.GetMemberAsync(u.Id);
                return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        [AsyncListener(EventTypes.PresenceUpdated)]
        public static async Task PresenceUpdated(ModCoreShard bot, PresenceUpdateEventArgs e)
        {
            // TODO fix me
            var db = bot.SharedData.ModCore.CreateGlobalContext();
            var loggers = db.GuildConfig.Where(x => x.GetSettings().LogUpdates).Select(x => x.GuildId);

            var guilds = bot?.Parent?.Shards?.SelectMany(x => x.Client?.Guilds?.Where(y => y.Value.GetGuildSettings(db)?.LogUpdates ?? false));
            if (guilds != null)
                foreach (var g in guilds.Select(x => x.Value))
                {
                    if (await IsInGuild(e.UserAfter, g))
                    {
                        var gst = g.GetGuildSettings(db);
                        var log = g.GetChannel(gst.UpdateChannel);

                        var embed = new DiscordEmbedBuilder()
                            .WithTitle("User Updated:")
                            .WithDescription($"{e.UserAfter.Username}#{e.UserAfter.Discriminator}");

                        if (e.UserAfter.Username != e.UserBefore.Username)
                            embed.AddField("Changed username", $"{e.UserBefore.Username} to {e.UserAfter.Username}");

                        if (e.UserAfter.Discriminator != e.UserBefore.Discriminator)
                            embed.AddField("Changed discriminator", $"{e.UserBefore.Discriminator} to {e.UserAfter.Discriminator}");

                        if (e.UserAfter.AvatarUrl != e.UserBefore.AvatarUrl)
                            embed.AddField("Changed avatar", $"[Old Avatar]({e.UserBefore.AvatarUrl})" +
                                $"\nNote: this link may 404 later due to cache invalidation");

                        if (e.UserAfter.IsBot != e.UserBefore.IsBot)
                            embed.AddField("Magically transformed between bot form and human form",
                                $"Wait, what the fuck this isn't possible");

                        // TODO ModCore color scheme
                        embed.WithColor(new DiscordColor("#089FDF"));

                        embed.WithThumbnail(e.UserAfter.AvatarUrl);
                        if (embed.Fields.Count > 0)
                            await log.SendMessageAsync(embed: embed);
                    }
                }
        }

        [AsyncListener(EventTypes.GuildMemberUpdated)]
        public static async Task MemberUpdated(ModCoreShard bot, GuildMemberUpdateEventArgs e)
        {
            var db = bot.SharedData.ModCore.CreateGlobalContext();
            var gst = e.Guild.GetGuildSettings(db);

            if(gst == null)
            {
                return;
            }

            if (gst.LogUpdates)
            {
                var log = e.Guild.GetChannel(gst.UpdateChannel);

                var embed = new DiscordEmbedBuilder()
                    .WithTitle("Member Updated:")
                    .WithDescription($"{e.Member.Username}#{e.Member.Discriminator}");

                // TODO ModCore color scheme
                embed.WithColor(new DiscordColor("#089FDF"));

                embed.WithThumbnail(e.Member.AvatarUrl);

                if (e.NicknameAfter != e.NicknameBefore)
                {
                    var after = e.Member.Username;
                    var before = e.Member.Username;

                    if (!string.IsNullOrEmpty(e.NicknameAfter))
                        after = e.NicknameAfter;
                    if (!string.IsNullOrEmpty(e.NicknameBefore))
                        before = e.NicknameBefore;

                    embed.AddField("Nickname update", $"{before} to {after}");
                }

                if (e.RolesAfter != e.RolesBefore)
                {
                    var added = e.RolesAfter.Where(x => !e.RolesBefore.Contains(x));
                    var removed = e.RolesBefore.Where(x => !e.RolesAfter.Contains(x));

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