﻿using DSharpPlus;
using DSharpPlus.Entities;
using ModCore.Extensions;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Interfaces;
using System.Threading.Tasks;
namespace ModCore.Buttons
{
    [Button("config_wc")]
    public class WelcomerConfig : IButton
    {
        private DiscordClient client;

        public WelcomerConfig(DiscordClient client)
        {
            this.client = client;
        }

        public async Task HandleAsync(DiscordInteraction interaction, DiscordMessage message)
        {
            var menu = new DiscordInteractionResponseBuilder()
                .WithContent("TODO: Configuration module...")
                .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, client.GetInteractionExtension().GenerateButton<ConfigMenu>(), "Back to menu", emoji: new DiscordComponentEmoji("👈")));

            await interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, menu);
        }
    }
}
