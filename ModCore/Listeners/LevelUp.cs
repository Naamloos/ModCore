using System;
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
using ModCore.Entities;
using ModCore.Logic;
using ModCore.Logic.Extensions;

namespace ModCore.Listeners
{
    public static class LevelUp
    {
        private static ConcurrentDictionary<(ulong server, ulong user), DateTimeOffset> LastXpGrants 
            = new ConcurrentDictionary<(ulong server, ulong user), DateTimeOffset>();

        [AsyncListener(EventTypes.MessageCreated)]
        public static async Task CheckLevelUpdates(ModCoreShard bot, MessageCreateEventArgs e)
        {
            // Storing vars for quick reference
            var server = e.Guild;
            var message = e.Message;
            var user = e.Message.Author as DiscordMember;

            if (user.IsBot)
                return;

            GuildSettings cfg;
            DatabaseUserData ud = null;
            // holder for returned datetimeoffsets
            DateTimeOffset p = DateTimeOffset.MinValue;

            using (var db = bot.Database.CreateContext())
            {
                cfg = e.Guild.GetGuildSettings(db);
                if (cfg == null)
                    return;
                if (!cfg.Levels.Enabled)
                    return;
                if (e.Message.Content.StartsWith(string.IsNullOrEmpty(cfg.Prefix) ? bot.Settings.DefaultPrefix : cfg.Prefix))
                    return;

                // Get user data and assign new when null
                if (db.UserDatas.Any(x => x.UserId == (long)user.Id))
                {
                    ud = db.UserDatas.First(x => x.UserId == (long)user.Id);
                }
                ud ??= new DatabaseUserData() { UserId = (long)user.Id };

                // do level stuff
                if(LastXpGrants.ContainsKey((server.Id, user.Id)))
                {
                    // Less than 1 hour since last message in server
                    if (LastXpGrants.TryGetValue((server.Id, user.Id), out p))
                    {
                        if (DateTimeOffset.Now.Subtract(p).TotalMinutes < 30)
                        {
                            // When debugging we want to grant xp for every message.
                            #if !DEBUG
                            return;
                            #endif
                        }
                    }
                }
                else
                {
                    LastXpGrants.TryAdd((server.Id, user.Id), DateTimeOffset.MinValue);
                }

                // Setting new last xp grant timestamp
                LastXpGrants.TryRemove((server.Id, user.Id), out p);
                LastXpGrants.TryAdd((server.Id, user.Id), DateTimeOffset.Now);

                // Getting user data and setting xp value if not yet set.
                var udata = ud.GetData();
                udata ??= new UserData();
                if (!udata.ServerExperience.ContainsKey(server.Id))
                {
                    udata.ServerExperience.Add(server.Id, 0);
                }

                // Getting old XP value and new XP value
                var previousxp = udata.ServerExperience[server.Id];
                udata.ServerExperience[server.Id] += new Random().Next(10, 50);
                var newxp = udata.ServerExperience[server.Id];

                // Do checks for message
                if (cfg.Levels.MessagesEnabled)
                {
                    var msgchannel = e.Channel;
                    if (cfg.Levels.RedirectMessages)
                    {
                        // Check whether channel exists and then change channel
                        if (server.Channels.ContainsKey(cfg.Levels.ChannelId))
                        {
                            msgchannel = server.Channels[cfg.Levels.ChannelId];
                        }
                    }

                    // Calculate old and new level from XPs
                    var oldlevel = CalculateLevel(previousxp);
                    var newlevel = CalculateLevel(newxp);

                    if (newlevel > oldlevel)
                    {
                        // Leveled up! congratulate user.
                        await msgchannel.SendMessageAsync($"Congratulations, {user.Nickname ?? user.Username}! You've leveled up!\n_Level {oldlevel} -> Level {newlevel}_");
                    }
                }

                // setting new data and updating
                ud.SetData(udata);
                db.UserDatas.Update(ud);
                await db.SaveChangesAsync();
            }
        }

        public static int CalculateLevel(int xp)
        {
            // XP formula: 150 * (x^2)
            // Level formula: sqrt(x / 150)
            // (and ofc floor to get round integer levels)
            return (int)Math.Floor(Math.Sqrt(xp / 150));
        }

        public static int CalculateRequiredXp(int level)
        {
            return 150 * (int)Math.Pow(level, 2);
        }
    }
}
