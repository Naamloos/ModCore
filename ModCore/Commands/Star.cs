using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using ModCore.Entities;
using ModCore.Logic;
using ModCore.Logic.Extensions;

namespace ModCore.Commands
{
    [Group("star"), Aliases("s"), Description("Star commands.")]
    public class Star : BaseCommandModule
	{
        public SharedData Shared { get; }
        public InteractivityExtension Interactivity { get; }
        public DatabaseContextBuilder Database { get; }

        public Star(DatabaseContextBuilder db, SharedData shared, InteractivityExtension interactive)
        {
            this.Shared = shared;
            this.Interactivity = interactive;
            this.Database = db;
        }

        [Command("info"), Aliases("i", "data", "information"), Description("Returns stardata for a specified user.")]
        public async Task ListGivenAsync(CommandContext context, [Description("User to show stardata information about.")] DiscordMember member)
        {
            var embed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#089FDF"))
                .WithTitle($"{member.DisplayName} - {member.Username}#{member.Discriminator}");

            using (var db = Database.CreateContext())
            {
                var guildStars = db.StarDatas.Where(x => (ulong)x.GuildId == context.Guild.Id);
                var givenStars = guildStars.Where(x => (ulong)x.StargazerId == member.Id);
                var gotStars = guildStars.Where(x => (ulong)x.AuthorId == member.Id);

                embed.Description =
                    $"You have given **{givenStars.Count()}** stars to other users.\n\n" +
                    $"You have been given **{gotStars.Count()}** stars by **{gotStars.Select(x => x.StargazerId).Distinct().Count()}** different users, over **{gotStars.Select(x => x.MessageId).Distinct().Count()}** different messages.";

                var allMembers = await context.Guild.GetAllMembersAsync();

                var givenMemberNames = new Dictionary<string, int>();                
                foreach (DatabaseStarData star in givenStars)
                {
                    string memberName = "Removed User";
                    if (allMembers.Any(x => x.Id == (ulong)star.AuthorId))
                    {
                        memberName = allMembers.First(x => x.Id == (ulong)star.AuthorId).Mention;
                    }
                    if (givenMemberNames.ContainsKey(memberName))
                    {
                        givenMemberNames[memberName] += 1;
                    }
                    else
                    {
                        givenMemberNames.Add(memberName, 1);
                    }
                }

                var orderGivenmemberNames = givenMemberNames.OrderByDescending(x => x.Value).Select(x => x.Key + " - " + x.Value).ToArray();
                embed.AddField("Users who have been given stars by you", string.Join("\n", orderGivenmemberNames.Take(10)));
                
                if (orderGivenmemberNames.Length > 10)
                    embed.Fields.Last().Value += $"\nAnd {orderGivenmemberNames.Length - 10} more...";
                
                var gotMemberNames = new Dictionary<string, int>();
                foreach (DatabaseStarData star in gotStars)
                {
                    string memberName = "Removed User";
                    if (allMembers.Any(x => x.Id == (ulong)star.StargazerId))
                    {
                        memberName = allMembers.First(x => x.Id == (ulong)star.StargazerId).Mention;
                    }
                    if (gotMemberNames.ContainsKey(memberName))
                    {
                        gotMemberNames[memberName] += 1;
                    }
                    else
                    {
                        gotMemberNames.Add(memberName, 1);
                    }
                }
                var orderedGotMemberNames = gotMemberNames.OrderByDescending(x => x.Value).Select(x => x.Key + " - " + x.Value).ToArray();
                embed.AddField("Users who have given you stars", string.Join("\n", orderedGotMemberNames.Take(10)));
                
                if (orderedGotMemberNames.Length > 10)
                    embed.Fields.Last().Value += $"\nAnd {orderedGotMemberNames.Length - 10} more...";

                await context.ElevatedRespondAsync(embed: embed);
            }
        }

        [Command("leaderboard"), Aliases("top", "lb", "board")]
        public async Task LeaderboardAsync(CommandContext context)
        {
            using (var db = Database.CreateContext())
            {
                var guildStars = db.StarDatas.Where(x => (ulong)x.GuildId == context.Guild.Id).ToList();
                var groups = guildStars.GroupBy(x => x.AuthorId);
                var top10 = groups.OrderByDescending(x => x.Count()).Take(10);

                var top10string = "";
                int index = 1;
                foreach (var data in top10)
                {
                    top10string += $"{index}. <@{data.Key}>: " +
                        $"{data.Count()} stars\n";
                    index++;
                }

                var embed = new DiscordEmbedBuilder()
                    .WithTitle($"{context.Guild.Name} Star Leaderboard")
                    .WithDescription($"These are the users with the most stars!")
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
