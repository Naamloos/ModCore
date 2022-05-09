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
        public static async Task CheckLevelUpdates(ModCoreShard bot, MessageCreateEventArgs eventargs)
        {
            // Storing vars for quick reference
            var server = eventargs.Guild;
            var message = eventargs.Message;
            var user = eventargs.Message.Author as DiscordMember;

            if (user.IsBot)
                return;

            GuildSettings config;
            DatabaseUserData userdata = null;
            // holder for returned datetimeoffsets
            DateTimeOffset lastGrant = DateTimeOffset.MinValue;

            using (var db = bot.Database.CreateContext())
            {
                config = eventargs.Guild.GetGuildSettings(db);
                if (config == null)
                    return;
                if (!config.Levels.Enabled)
                    return;
                if (eventargs.Message.Content.StartsWith(string.IsNullOrEmpty(config.Prefix) ? bot.Settings.DefaultPrefix : config.Prefix))
                    return;

                // Get user data and assign new when null
                if (db.UserDatas.Any(x => x.UserId == (long)user.Id))
                {
                    userdata = db.UserDatas.First(x => x.UserId == (long)user.Id);
                }
                userdata ??= new DatabaseUserData() { UserId = (long)user.Id };

                // do level stuff
                if(LastXpGrants.ContainsKey((server.Id, user.Id)))
                {
                    // Less than 1 hour since last message in server
                    if (LastXpGrants.TryGetValue((server.Id, user.Id), out lastGrant))
                    {
                        if (DateTimeOffset.Now.Subtract(lastGrant).TotalMinutes < 30)
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
                LastXpGrants.TryRemove((server.Id, user.Id), out lastGrant);
                LastXpGrants.TryAdd((server.Id, user.Id), DateTimeOffset.Now);

                // Getting user data and setting xp value if not yet set.
                var jsonuserdata = userdata.GetData();
                jsonuserdata ??= new UserData();
                if (!jsonuserdata.ServerExperience.ContainsKey(server.Id))
                {
                    jsonuserdata.ServerExperience.Add(server.Id, 0);
                }

                // Getting old XP value and new XP value
                var previousxp = jsonuserdata.ServerExperience[server.Id];
                jsonuserdata.ServerExperience[server.Id] += new Random().Next(10, 50);
                var newxp = jsonuserdata.ServerExperience[server.Id];

                // Do checks for message
                if (config.Levels.MessagesEnabled)
                {
                    var msgchannel = eventargs.Channel;
                    if (config.Levels.RedirectMessages)
                    {
                        // Check whether channel exists and then change channel
                        if (server.Channels.ContainsKey(config.Levels.ChannelId))
                        {
                            msgchannel = server.Channels[config.Levels.ChannelId];
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
                userdata.SetData(jsonuserdata);
                db.UserDatas.Update(userdata);
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
