using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.VisualBasic;
using ModCore.Buttons;
using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using ModCore.Database.JsonEntities;
using ModCore.Extensions;
using ModCore.Modals;
using ModCore.Utils.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.SlashCommands
{
    public class Config : ApplicationCommandModule
    {
        public DatabaseContextBuilder Database { private get; set; }

        [SlashCommand("config", "Launches the ModCore Configuration Utility.")]
        [SlashCommandPermissions(Permissions.ManageGuild)]
        public async Task ConfigAsync(InteractionContext ctx)
        {
            var buttons = ctx.Client.GetInteractionExtension();

            var databaseContext = Database.CreateContext();
            var settings = ctx.Guild.GetGuildSettings(databaseContext);
            if(settings == null)
            {
                // This seems like a great moment to generate a new config
                var dbConfig = new DatabaseGuildConfig()
                {
                    GuildId = (long)ctx.Guild.Id
                };
                dbConfig.SetSettings(new GuildSettings());

                databaseContext.GuildConfig.Add(dbConfig);
                await databaseContext.SaveChangesAsync();
            }

            await ConfigMenu.SendConfigMenuAsync(ctx.Interaction, buttons, InteractionResponseType.ChannelMessageWithSource);
        }
    }
}
