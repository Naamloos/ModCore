using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using ModCore.Utils.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.SlashCommands
{
    [SlashCommandGroup("starboard", "Starboard-related commands")]
    public class Starboard : ApplicationCommandModule
    {
        public DatabaseContextBuilder Database { private get; set; }

        [SlashCommand("info", "Starboard information for a specific user.")]
        public async Task InfoAsync(InteractionContext ctx, [Option("user", "User to display info about.")]DiscordUser user = null)
        {
            await ctx.DeferAsync(true);
            
            var selectedUser = user ?? ctx.User;
            var member = await ctx.Guild.GetMemberAsync(selectedUser.Id);

            var embed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#089FDF"))
                .WithAuthor($"{member.DisplayName} - {member.Username}#{member.Discriminator}", iconUrl: member.GetGuildAvatarUrl(ImageFormat.Png));

            using (var db = Database.CreateContext())
            {
                var guildStars = db.StarDatas.Where(x => (ulong)x.GuildId == ctx.Guild.Id);

                var givenStars = guildStars.Where(x => (ulong)x.StargazerId == member.Id);
                var gotStars = guildStars.Where(x => (ulong)x.AuthorId == member.Id);

                embed.Description =
                    $"This user has given **{givenStars.Count()}** stars to other users.\n" +
                    $"This user has been given **{gotStars.Count()}** stars by **{gotStars.Select(x => x.StargazerId).Distinct().Count()}** different users, " +
                    $"over **{gotStars.Select(x => x.MessageId).Distinct().Count()}** different messages.";

                var given = givenStars.GroupBy(x => x.AuthorId).OrderByDescending(x => x.Count()).Select(x => $"<@{x.Key}>").ToList();
                if (given.Any())
                {
                    embed.AddField("💫 Users who have been given stars by this user", string.Join("\n", given.Take(10)));

                    if (given.Count() > 10)
                        embed.Fields.Last().Value += $"\nAnd {given.Count() - 10} others...";
                }
                else
                {
                    embed.AddField("💫 Users who have been given stars by this user", "You haven't given out any stars yet.");
                }

                var gotten = gotStars.GroupBy(x => x.StargazerId).OrderByDescending(x => x.Count()).Select(x => $"<@{x.Key}>").ToList();
                if (gotten.Any())
                {
                    embed.AddField("💫 Users who have given this user stars", string.Join("\n", gotten.Take(10)));

                    if (gotten.Count() > 10)
                        embed.Fields.Last().Value += $"\nAnd {gotten.Count() - 10} others...";
                }
                else
                {
                    embed.AddField("💫 Users who have given this user stars", "You haven't received any stars yet.");
                }

                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed).AsEphemeral());
            }
        }

        [SlashCommand("leaderboard", "Shows star leaderboard for this server.")]
        public async Task LeaderboardAsync(InteractionContext ctx)
        {
            await ctx.DeferAsync(true);

            using (var db = Database.CreateContext())
            {
                var guildStars = db.StarDatas.Where(x => (ulong)x.GuildId == ctx.Guild.Id).ToList();
                var groups = guildStars.GroupBy(x => x.AuthorId);
                var top10 = groups.OrderByDescending(x => x.Count()).Take(10);

                if(!top10.Any())
                {
                    await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("⚠️ No stars have been given out on this server yet!").AsEphemeral());
                    return;
                }

                var top10string = "";
                int index = 1;
                foreach (var data in top10)
                {
                    top10string += $"{index}. <@{data.Key}>: " +
                        $"{data.Count()} stars\n";
                    index++;
                }

                var embed = new DiscordEmbedBuilder()
                    .WithTitle($"{ctx.Guild.Name} Star Leaderboard")
                    .WithDescription($"These are the users with the most stars!")
                    .WithColor(new DiscordColor())
                    .AddField("Top 10", top10string);

                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed).AsEphemeral());
            }
        }
    }
}
