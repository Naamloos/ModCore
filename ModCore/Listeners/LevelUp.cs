using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using ModCore.Database.JsonEntities;
using ModCore.Entities;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Enums;
using ModCore.Utils.Extensions;

namespace ModCore.Listeners
{
    public static class LevelUp
    {
        [AsyncListener(EventType.MessageCreated)]
        public static async Task CheckLevelUpdates(MessageCreateEventArgs eventargs, DatabaseContextBuilder database, Settings settings)
        {
            // Storing vars for quick reference
            var server = eventargs.Guild;
            var message = eventargs.Message;
            var user = eventargs.Message.Author as DiscordMember;

            if (user == null || user.IsBot)
                return;

            GuildSettings config;
            DatabaseLevel leveldata = null;

            await using var db = database.CreateContext();
            config = eventargs.Guild.GetGuildSettings(db);
            if (config == null)
                return;
            if (!config.Levels.Enabled)
                return;

            // Get level xp data and assign new when null
            if (db.Levels.Any(x => x.UserId == (long)user.Id && x.GuildId == (long)server.Id))
            {
                leveldata = db.Levels.First(x => x.UserId == (long)user.Id && x.GuildId == (long)server.Id);
                // do level stuff
#if !DEBUG
                    if (DateTime.Now.Subtract(leveldata.LastXpGrant).TotalMinutes < 5)
                        return;
#endif
            }

            leveldata ??= new DatabaseLevel() 
            { 
                UserId = (long)user.Id, 
                GuildId = (long)server.Id, 
                Experience = 0,
                LastXpGrant = DateTime.Now
            };

            // Set new last xp datetime
            leveldata.LastXpGrant = DateTime.Now;

            // Getting old XP value and new XP value
            var previousxp = leveldata.Experience;
            leveldata.Experience += new Random().Next(10, 50);

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
                var newlevel = CalculateLevel(leveldata.Experience);

                if (newlevel > oldlevel)
                {
                    // Leveled up! congratulate user.
                    var embed = new DiscordEmbedBuilder()
                        .WithTitle($"🏆 Congratulations, {user.Nickname ?? user.Username}! You've leveled up!")
                        .WithDescription($"Level {oldlevel} ➡ Level {newlevel}")
                        .AddField("Experience", $"You currently have {leveldata.Experience} xp. " +
                                                $"{CalculateRequiredXp(newlevel + 1) - leveldata.Experience} needed to level up.");
                    await msgchannel.SendMessageAsync(embed);
                }
            }

            // setting new data and updating
            db.Levels.Update(leveldata);
            await db.SaveChangesAsync();
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
