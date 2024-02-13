using DSharpPlus.EventArgs;
using DSharpPlus;
using ModCore.Extensions.Abstractions;
using ModCore.Extensions.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using ModCore.Database;
using ModCore.Extensions;
using ModCore.Utils.Extensions;
using ModCore.Database.JsonEntities;
using System.Linq;
using ModCore.Entities;

namespace ModCore.Components
{
    public class AutoEmbedComponents : BaseComponentModule
    {
        private DatabaseContextBuilder database;

        public AutoEmbedComponents(DatabaseContextBuilder database)
        {
            this.database = database;
        }

        [Component("ae.select", ComponentType.StringSelect)]
        public async Task AutoRoleConfig(ComponentInteractionCreateEventArgs e)
        {
            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            EmbedMessageLinksMode mode;

            switch(e.Interaction.Data.Values[0])
            {
                default:
                    mode = EmbedMessageLinksMode.Disabled; break;
                case "prefix":
                    mode = EmbedMessageLinksMode.Prefixed; break;
                case "always":
                    mode = EmbedMessageLinksMode.Always; break;
            }

            await using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.EmbedMessageLinks = mode;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            string state;
            switch (mode)
            {
                default:
                    state = "Disabled";
                    break;
                case EmbedMessageLinksMode.Prefixed:
                    state = "Only when prefixed with `!`";
                    break;
                case EmbedMessageLinksMode.Always:
                    state = "Always embed";
                    break;
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 New state for Auto Jump Link Embed: {state}.")
                .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "ae", "Back to Jump Link Embed config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        public static async Task PostMenuAsync(DiscordInteraction interaction, InteractionResponseType responseType, DatabaseContext db)
        {
            await using (db)
            {
                var settings = interaction.Guild.GetGuildSettings(db);

                string state;
                var mode = settings.EmbedMessageLinks;

                switch(mode)
                {
                    default:
                        state = "Disabled";
                        break;
                    case EmbedMessageLinksMode.Prefixed:
                        state = "Only when prefixed with `!`";
                        break;
                    case EmbedMessageLinksMode.Always:
                        state = "Always embed";
                        break;
                }

                var embed = new DiscordEmbedBuilder()
                    .WithTitle("🖇️ Jump Link Embed Configuration")
                    .WithDescription("Jump Link Embed allows ModCore to automatically post embeds for jump links.")
                    .AddField("Current State", state);

                await interaction.CreateResponseAsync(responseType, new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed)
                    .WithContent("")
                    .AddComponents(new DiscordSelectComponent("ae.select", "Change state", new List<DiscordSelectComponentOption>()
                    {
                        new DiscordSelectComponentOption("Disable", "off", "Disables jump link embeds", mode == EmbedMessageLinksMode.Disabled, new DiscordComponentEmoji("❌")),
                        new DiscordSelectComponentOption("Prefixed with !", "prefix", "Create embeds when prefixed with !", 
                            mode == EmbedMessageLinksMode.Prefixed, new DiscordComponentEmoji("❕")),
                        new DiscordSelectComponentOption("Always", "always", "Always create jump link embeds", mode == EmbedMessageLinksMode.Always, new DiscordComponentEmoji("✅"))
                    }))
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "cfg", "Back to Config", emoji: new DiscordComponentEmoji("🏃"))));
            }
        }
    }
}
