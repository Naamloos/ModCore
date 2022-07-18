using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ModCore.Database;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.SlashCommands
{
    [SlashCommandGroup("level", "Commands relating to the level system.")]
    public class Level : ApplicationCommandModule
    {
        public DatabaseContextBuilder Database { private get; set; }

        [SlashCommand("info", "Information about you or someone else's level in this server.")]
        public async Task InfoAsync(InteractionContext ctx, [Option("user", "User to get level data from")]DiscordUser user = null)
        {
            var targetUser = user ?? ctx.User;
            var targetMember = await ctx.Guild.GetMemberAsync(targetUser.Id);

            var experience = 0;
            var level = 0;

            using (var db = Database.CreateContext())
            {
                var data = db.Levels.FirstOrDefault(x => x.UserId == (long)targetUser.Id && x.GuildId == (long)ctx.Guild.Id);
                if (data != null)
                {
                    level = Listeners.LevelUp.CalculateLevel(data.Experience);
                    experience = data.Experience;
                }
            }

            var levelupxp = Listeners.LevelUp.CalculateRequiredXp(level + 1) - experience;

            await ctx.CreateResponseAsync($"💫 **Currently, {(user == null ? "you are" : $"{targetMember.DisplayName} is")} Level {level} with {experience} xp.**" +
                $"\n{levelupxp} more xp is required to reach level {level + 1}.", true);
        }

        [SlashCommand("leaderboard", "Shows this server's level leaderboard.")]
        public async Task LeaderboardAsync(InteractionContext ctx)
        {
            using (var db = Database.CreateContext())
            {
                var top10 = db.Levels.Where(x => x.GuildId == (long)ctx.Guild.Id)
                    .OrderByDescending(x => x.Experience)
                    .Take(10)
                    .ToList();

                if(top10.Count == 0)
                {
                    await ctx.CreateResponseAsync("⚠️ No level data was found for this server!", true);
                    return;
                }

                var top10string = "";
                int index = 1;
                foreach (var leveldata in top10)
                {
                    top10string += $"{index}. <@{leveldata.UserId}>: Level " +
                        $"{Listeners.LevelUp.CalculateLevel(leveldata.Experience)} ({leveldata.Experience} xp)";
                    index++;
                }

                var embed = new DiscordEmbedBuilder()
                    .WithTitle($"{ctx.Guild.Name} Level Leaderboard")
                    .WithDescription($"These are the users with the most activity!")
                    .WithColor(new DiscordColor())
                    .AddField("Top 10", top10string);

                await ctx.CreateResponseAsync(embed, true);
            }
        }
    }
}
