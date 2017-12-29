using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using ModCore.Database;
using ModCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Commands
{
    [Group("star"), Aliases("s"), Description("Star commands.")]
    public class Star
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

        [Command("debug"), Aliases("d"), Description("Returns amount of stars in database.")]
        public async Task DebugAsync(CommandContext ctx)
        {
            using (var db = Database.CreateContext())
            {
                await ctx.RespondAsync($"Stars: {db.StarDatas.Count()}");
            }
        }

        [Command("info"), Aliases("i", "data", "information"), Description("Returns stardata for a specified user.")]
        public async Task ListGivenAsync(CommandContext ctx, DiscordMember m)
        {
            var embed = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.MidnightBlue)
                .WithTitle($"@{m.Username}#{m.Discriminator} - ID: {m.Id}");

            using (var db = Database.CreateContext())
            {
                var guildStars = db.StarDatas.Where(x => (ulong)x.GuildId == ctx.Guild.Id);
                var givenStars = guildStars.Where(x => (ulong)x.StargazerId == m.Id);
                var gotStars = guildStars.Where(x => (ulong)x.AuthorId == m.Id);

                embed.Description =
                    $"You have given **{givenStars.Count()}** stars to other users.\n" +
                    $"You have been given **{gotStars.Count()}** stars by **{gotStars.Select(x => x.MessageId).Distinct().Count()}** different users.";

                var memberNames = new Dictionary<string, int>();
                foreach (DatabaseStarData star in gotStars)
                {
                    string memberName = "Unknown User";
                    try 
                    {
                        var member = await ctx.Client.GetUserAsync((ulong)star.StargazerId);
                        memberName = $"{member.Username}#{member.Discriminator}";
                        if (memberNames.ContainsKey(memberName))
                        {
                            memberNames[memberName] += 1;
                        }
                        else
                        {
                            memberNames.Add(memberName, 1);
                        }
                    }
                    catch 
                    {
                        if (memberNames.ContainsKey(memberName))
                        {
                            memberNames[memberName] += 1;
                        }
                        else
                        {
                            memberNames.Add(memberName, 1);
                        }
                    }
                }
                var ordered = memberNames.OrderBy(x => x.Value);

                var memberLists = memberNames.Select(x => x.Key + " - " + x.Value);
                embed.AddField("Users who gave you stars", string.Join("\n", memberLists.Take(10)), false);
                if (memberLists.Count() > 10)
                    embed.Fields.Last().Value += $"\n and {memberLists.Count() - 10} more...";

                await ctx.RespondAsync(embed: embed);
            }
        }
    }
}
