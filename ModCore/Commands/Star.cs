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
    [Group("star"), Aliases("s"), Description("star commands")]
    public class Star
    {
        public SharedData Shared { get; }
        public InteractivityExtension Interactivity { get; }
        public DatabaseContextBuilder Database { get; }

        public Star(DatabaseContextBuilder db, SharedData shared, InteractivityExtension interactive)
        {
            this.Shared = shared;
            this.Interactivity = interactive;
        }

        [Command("debug"), Aliases("d"), Description("Returns information about a specific user")]
        public async Task DebugAsync(CommandContext ctx)
        {
            using(var db = Database.CreateContext())
            {
                await ctx.RespondAsync($"Stars: {db.StarDatas.Count()}");
            }
        }
    }
}
