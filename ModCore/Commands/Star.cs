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
#warning TODO: fix jcryer's terrible star info commands - amount of stars a user has given or received. 

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

        [Command("listgiven"), Aliases("listgive", "lgive", "listgi"), Description("Returns amount of stars the user has ever given.")]
        public async Task ListGivenAsync(CommandContext ctx, DiscordMember m)
        {
            using (var db = Database.CreateContext())
            {
                await ctx.RespondAsync($"You have given: "
                    + db.StarDatas.Count(x => (ulong)x.StargazerId == m.Id)
                    + " stars in total.");
            }
        }

        [Command("listgot"), Aliases("listg", "lgot"), Description("Returns amount of stars the user has ever been given.")]
        public async Task ListGotAsync(CommandContext ctx, DiscordMember m)
        {
            using (var db = Database.CreateContext())
            {
                var messages = db.StarDatas.Where(x => (ulong)x.AuthorId == m.Id);
                if (!messages.Any())
                {
                    await ctx.RespondAsync("You have never been given a star.");
                    return;
                }
                var unique = messages.Select(x => x.Id).Distinct().Count();

                await ctx.RespondAsync($"You have been given: "
                    + messages.Count()
                    + " stars in total, over: "
                    + unique
                    + " different messages.");
            }
        }
    }
}
