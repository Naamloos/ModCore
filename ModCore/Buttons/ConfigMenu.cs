using DSharpPlus;
using DSharpPlus.Entities;
using ModCore.Extensions;
using ModCore.Extensions.Buttons.Attributes;
using ModCore.Extensions.Buttons.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModCore.Buttons
{
    [Button("config")]
    public class ConfigMenu : IButton
    {
        private DiscordClient client;

        public ConfigMenu(DiscordClient client)
        {
            this.client = client;
        }

        public async Task HandleAsync(DiscordInteraction interaction, DiscordMessage message)
        {
            await SendConfigMenuAsync(interaction, client.GetButtons());
        }

        public static async Task SendConfigMenuAsync(DiscordInteraction interaction, ButtonExtension buttons, InteractionResponseType responsetype = InteractionResponseType.UpdateMessage)
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle("<:modcore:996915638545158184> Welcome to the ModCore Configuration Utility!")
                .WithDescription("Please select one of the available modules to configure.")
                .WithColor(new DiscordColor("#089FDF"));

            var resp = new DiscordInteractionResponseBuilder()
                .AddEmbed(embed)
                .WithContent("")
                .AsEphemeral();

            var starboard = buttons.GenerateCommand<StarboardConfig>();
            var rolestate = buttons.GenerateCommand<RolestateConfig>();
            var linkfilter = buttons.GenerateCommand<LinkfilterConfig>();
            var autorole = buttons.GenerateCommand<AutoroleConfig>();
            var rolemenu = buttons.GenerateCommand<RolemenuConfig>();
            var welcomer = buttons.GenerateCommand<WelcomerConfig>();
            var levels = buttons.GenerateCommand<LevelsConfig>();
            var logging = buttons.GenerateCommand<LoggingConfig>();

            resp.AddComponents(
                new DiscordButtonComponent(ButtonStyle.Secondary, starboard, "Starboard", emoji: new DiscordComponentEmoji("⭐")),
                new DiscordButtonComponent(ButtonStyle.Secondary, rolestate, "Role State", emoji: new DiscordComponentEmoji("🗿")),
                new DiscordButtonComponent(ButtonStyle.Secondary, linkfilter, "Link Filters", emoji: new DiscordComponentEmoji("🔗")),
                new DiscordButtonComponent(ButtonStyle.Secondary, autorole, "Auto Role", emoji: new DiscordComponentEmoji("🤖"))
                );

            resp.AddComponents(
                new DiscordButtonComponent(ButtonStyle.Secondary, rolemenu, "Role Menu", emoji: new DiscordComponentEmoji("📖")),
                new DiscordButtonComponent(ButtonStyle.Secondary, welcomer, "Welcomer", emoji: new DiscordComponentEmoji("👋")),
                new DiscordButtonComponent(ButtonStyle.Secondary, levels, "Level System", emoji: new DiscordComponentEmoji("📈")),
                new DiscordButtonComponent(ButtonStyle.Secondary, logging, "Update Logger", emoji: new DiscordComponentEmoji("🪵"))
                );

            await interaction.CreateResponseAsync(responsetype, resp);
        }
    }
}
