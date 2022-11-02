using DSharpPlus;
using DSharpPlus.Entities;
using ModCore.Extensions;
using ModCore.Extensions.Buttons.Attributes;
using ModCore.Extensions.Buttons.Interfaces;
using System.Threading.Tasks;
namespace ModCore.Buttons
{
    [Button("config_lv")]
    public class LevelsConfig : IButton
    {
        private DiscordClient client;

        public LevelsConfig(DiscordClient client)
        {
            this.client = client;
        }

        public async Task HandleAsync(DiscordInteraction interaction, DiscordMessage message)
        {
            var menu = new DiscordInteractionResponseBuilder()
                .WithContent("TODO: Configuration module...")
                .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, client.GetButtons().GenerateCommand<ConfigMenu>(), "Back to menu", emoji: new DiscordComponentEmoji("👈")));

            await interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, menu);
        }
    }
}
