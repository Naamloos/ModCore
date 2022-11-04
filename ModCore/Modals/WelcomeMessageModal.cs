using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Extensions.Interfaces;
using ModCore.Extensions.Attributes;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Modals
{
    [Modal("welcome")]
    public class WelcomeMessageModal : IModal
    {
        [ModalField("New welcome message?", "welcome", "",
            "Supported tags: https://gist.github.com/Naamloos/a1c87c24ff238edbdd28258b08452ed4", true, TextInputStyle.Paragraph, 10, 255)]
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

            var db = ((DatabaseContextBuilder)client.GetCommandsNext().Services.GetService(typeof(DatabaseContextBuilder))).CreateContext();
            var settings = db.GuildConfig.FirstOrDefault(x => x.GuildId == (long)interaction.GuildId)?.GetSettings();
            if(settings == null)
            {
                await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("❌ No guild config?? contact devs!!1").AsEphemeral());
                return;
            }

            settings.Welcome.Message = Welcome;

            await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("✅ Welcome message was configured!").AsEphemeral());
        }
    }
}