using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.EventHandling;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;
using ModCore.Logic.Extensions;

namespace ModCore.Commands
{
    [Group("level"), CheckDisable]
    public class Level : BaseCommandModule
    {
        public SharedData Shared { get; }
        public DatabaseContextBuilder Database { get; }
        public InteractivityExtension Interactivity { get; }
        public StartTimes StartTimes { get; }

        public Level(SharedData shared, DatabaseContextBuilder db, InteractivityExtension interactive,
            StartTimes starttimes)
        {
            this.Database = db;
            this.Shared = shared;
            this.Interactivity = interactive;
            this.StartTimes = starttimes;
        }

        [GroupCommand]
        public async Task ExecuteGroupAsync(CommandContext context,
            [RemainingText, Description("User to get level data from")] DiscordMember member = null)
        {
            DiscordMember localmember = member;
            if (localmember == null)
            {
                localmember = context.Member;
            }

            var experience = 0;
            var level = 0;

            using (var db = Database.CreateContext())
            {
                var data = db.Levels.FirstOrDefault(x => x.UserId == (long)localmember.Id && x.GuildId == (long)context.Guild.Id);
                if (data != null)
                {
                    level = Listeners.LevelUp.CalculateLevel(data.Experience);
                    experience = data.Experience;
                }
            }

            var levelupxp = Listeners.LevelUp.CalculateRequiredXp(level + 1) - experience;

            await context.RespondAsync($"💫 **Currently, {(member == null ? "you are" : $"{member.DisplayName} is")} Level {level} with {experience} xp.**" +
                $"\n{levelupxp} more xp is required to reach level {level + 1}.");
        }

        [Command("leaderboard"), Aliases("top", "lb", "board")]
        public async Task LeaderboardAsync(CommandContext context)
        {
            using (var db = Database.CreateContext()) 
            {
                var top10 = db.Levels.Where(x => x.GuildId == (long)context.Guild.Id)
                    .OrderByDescending(x => x.Experience)
                    .Take(10)
                    .ToList();

                var top10string = "";
                int index = 1;
                foreach(var leveldata in top10)
                {
                    top10string += $"{index}. <@{leveldata.UserId}>: Level " +
                        $"{Listeners.LevelUp.CalculateLevel(leveldata.Experience)} ({leveldata.Experience} xp)";
                    index++;
                }

                var embed = new DiscordEmbedBuilder()
                    .WithTitle($"{context.Guild.Name} Level Leaderboard")
                    .WithDescription($"These are the users with the most activity!")
                    .WithColor(new DiscordColor())
                    .AddField("Top 10", top10string);

                var message = new DiscordMessageBuilder()
                    .AddEmbed(embed)
                    .WithReply(context.Message.Id, true, false);

                await context.RespondAsync(message);
            }
        }
    }
}