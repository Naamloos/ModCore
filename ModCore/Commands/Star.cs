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
                .WithTitle($"{m.DisplayName} - {m.Username}#{m.Discriminator}");

            using (var db = Database.CreateContext())
            {
                var guildStars = db.StarDatas.Where(x => (ulong)x.GuildId == ctx.Guild.Id);
                var givenStars = guildStars.Where(x => (ulong)x.StargazerId == m.Id);
                var gotStars = guildStars.Where(x => (ulong)x.AuthorId == m.Id);

                embed.Description =
                    $"You have given **{givenStars.Count()}** stars to other users.\n\n" +
                    $"You have been given **{gotStars.Count()}** stars by **{gotStars.Select(x => x.StargazerId).Distinct().Count()}** different users, over **{gotStars.Select(x => x.MessageId).Distinct().Count()}** different messages.";
                
                var givenMemberNames = new Dictionary<string, int>();                
                foreach (DatabaseStarData star in givenStars)
                {
                    string memberName = "Unknown User";
                    try 
                    {
                        memberName = (await ctx.Client.GetUserAsync((ulong)star.AuthorId)).Mention;
                        if (givenMemberNames.ContainsKey(memberName))
                        {
                            givenMemberNames[memberName] += 1;
                        }
                        else
                        {
                            givenMemberNames.Add(memberName, 1);
                        }
                    }
                    catch 
                    {
                        if (givenMemberNames.ContainsKey(memberName))
                        {
                            givenMemberNames[memberName] += 1;
                        }
                        else
                        {
                            givenMemberNames.Add(memberName, 1);
                        }
                    }
                }
                var orderGivenmemberNames = givenMemberNames.OrderByDescending(x => x.Value).Select(x => x.Key + " - " + x.Value);
                embed.AddField("Users who you gave stars", string.Join("\n", orderGivenmemberNames.Take(10)), false);
                
                if (orderGivenmemberNames.Count() > 10)
                    embed.Fields.Last().Value += $"\nAnd {orderGivenmemberNames.Count() - 10} more...";
                
                var gotMemberNames = new Dictionary<string, int>();
                foreach (DatabaseStarData star in gotStars)
                {
                    string memberName = "Unknown User";
                    try 
                    {
                        memberName = (await ctx.Client.GetUserAsync((ulong)star.StargazerId)).Mention;
                        if (gotMemberNames.ContainsKey(memberName))
                        {
                            gotMemberNames[memberName] += 1;
                        }
                        else
                        {
                            gotMemberNames.Add(memberName, 1);
                        }
                    }
                    catch 
                    {
                        if (gotMemberNames.ContainsKey(memberName))
                        {
                            gotMemberNames[memberName] += 1;
                        }
                        else
                        {
                            gotMemberNames.Add(memberName, 1);
                        }
                    }
                }
                var orderedGotMemberNames = gotMemberNames.OrderByDescending(x => x.Value).Select(x => x.Key + " - " + x.Value);
                embed.AddField("Users who gave you stars", string.Join("\n", orderedGotMemberNames.Take(10)), false);
                
                if (orderedGotMemberNames.Count() > 10)
                    embed.Fields.Last().Value += $"\nAnd {orderedGotMemberNames.Count() - 10} more...";

                await ctx.RespondAsync(embed: embed);
            }
        }
    }
}
