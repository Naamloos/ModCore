using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Extensions.Attributes;
using System.Linq;
using System.Threading.Tasks;
using ModCore.Extensions.Abstractions;

namespace ModCore.Modals
{
    [Modal("welcome")]
    public class WelcomeMessageModal : IModal
    {
        [ModalField("New welcome message?", "welcome", "Welcomer supports replacement tags. These can be found behind a link button in the welcomer menu.",
            "", true, TextInputStyle.Paragraph, 10, 255)]
        public string Welcome { get; set; }

        private DiscordClient client;

        public WelcomeMessageModal(DiscordClient client, Settings settings)
        {
            this.client = client;
        }

        public async Task HandleAsync(DiscordInteraction interaction)
        {
            var member = await interaction.Guild.GetMemberAsync(interaction.User.Id);
            if(!member.Permissions.HasPermission(Permissions.ManageGuild))
            {
                return;
            }

            using (var db = ((DatabaseContextBuilder)client.GetCommandsNext().Services.GetService(typeof(DatabaseContextBuilder))).CreateContext())
            {
                var guildConfig = db.GuildConfig.FirstOrDefault(x => x.GuildId == (long)interaction.GuildId);
                var settings = guildConfig?.GetSettings();
                if (settings == null)
                {
                    await interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"⛔ No guild config??? contact devs!!1")
                        .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "wc", "Back to Welcomer config", emoji: new DiscordComponentEmoji("🏃"))));
                    return;
                }

                settings.Welcome.Message = Welcome;

                guildConfig.SetSettings(settings);
                db.GuildConfig.Update(guildConfig);
                await db.SaveChangesAsync();

                await interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AsEphemeral().WithContent($"✅ Set new welcome message"));

                await interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"✅ Welcome message was configured!")
                        .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "wc", "Back to Welcomer config", emoji: new DiscordComponentEmoji("🏃"))));
            }
        }
    }
}