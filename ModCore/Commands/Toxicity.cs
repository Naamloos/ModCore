using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using ModCore.Entities;
using DSharpPlus.Entities;
using System.Linq;
using ModCore.Database;
using DSharpPlus;

namespace ModCore.Commands
{
    [Group("toxicity"), Aliases("toxic", "tox", "t"), RequireUserPermissions(Permissions.ManageMessages)]
    public class Toxicity
    {
        public SharedData Shared { get; }
        public DatabaseContextBuilder Database { get; }
        public InteractivityExtension Interactivity { get; }
        public StartTimes StartTimes { get; }

        public Toxicity(SharedData shared, DatabaseContextBuilder db, InteractivityExtension interactive, StartTimes starttimes)
        {
            this.Database = db;
            this.Shared = shared;
            this.Interactivity = interactive;
            this.StartTimes = starttimes;
        }

        [Command("analyze"), Description("Analyze a member's toxicity level from a channel's message history. Note " +
                                         "that this feature is still in heavy development and subject to change!")]
        public async Task AnalyzeAsync(CommandContext ctx, [Description("Member to calculate toxicity from")]DiscordMember member, 
            [Description("Channel to calculate in")]DiscordChannel channel)
        {
            var msg = await channel.GetMessagesBeforeAsync(await channel.GetMessageAsync(channel.LastMessageId), 100);
            var msgstr = msg.Where(x => x.Author.Id == member.Id).Select(x => x.Content);
            var str = string.Join('\n', msgstr);
            var a = await Shared.Perspective.RequestAnalysis(str);
            await ctx.RespondAsync($"Toxicity: {a.AttributeScores.First().Value.SummaryScore.Value}");
        }
    }
}
