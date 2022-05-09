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

                if (db.UserDatas.Any(x => x.UserId == (long)localmember.Id))
                {
                    var data = db.UserDatas.First(x => x.UserId == (long)localmember.Id).GetData();

                    if (data.ServerExperience.ContainsKey(context.Guild.Id))
                    {
                        experience = data.ServerExperience[context.Guild.Id];
                        level = Listeners.LevelUp.CalculateLevel(experience);
                    }
                }
            }

            var levelupxp = Listeners.LevelUp.CalculateRequiredXp(level + 1) - experience;

            await context.RespondAsync($"💫 **Currently, {(member == null ? "you are" : $"{member.DisplayName} is")} Level {level} with {experience} xp.**" +
                $"\n{levelupxp} more xp is required to reach level {level + 1}.");
        }

        [Command("leaderboard"), Aliases("top", "lb", "board")]
        public async Task LeaderboardAsync(CommandContext context)
        {
            await context.RespondAsync("😔 This feature has not yet been implemented. Please try again later!");
        }
    }
}