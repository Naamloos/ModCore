using DSharpPlus;
using DSharpPlus.Entities;
using ModCore.Database;
using ModCore.Database.JsonEntities;
using ModCore.Extensions;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Interfaces;
using ModCore.Utils;
using ModCore.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModCore.Buttons
{
    [Button("config_sb")]
    public class StarboardConfig : IButton
    {
        private DiscordClient client;
        private DatabaseContextBuilder database;

        public StarboardConfig(DiscordClient client, DatabaseContextBuilder database)
        {
            this.client = client;
            this.database = database;
        }

        public async Task HandleAsync(DiscordInteraction interaction, DiscordMessage message)
        {
            if (!interaction.Channel.PermissionsFor(interaction.User as DiscordMember).HasPermission(Permissions.ManageGuild))
                return;

            var settings = interaction.Guild.GetGuildSettings(database.CreateContext());

            var currentConfig = new DiscordEmbedBuilder()
                .WithTitle("Starboard Configuration")
                .WithDescription(settings.Starboard.Enable ? "✅ Module Enabled" : "⛔ Module Disabled")
                .AddField("Selected Channel", settings.Starboard.ChannelId > 0 ? $"<#{settings.Starboard.ChannelId}>" : "‼️ Not yet configured");

            var enablePropertyPath = ConfigValueSerialization.GetConfigPropertyPath(x => x.Starboard.Enable);
            var interactions = client.GetInteractionExtension();
            var enable = interactions.GenerateButton<ConfigValueButton>(("p", enablePropertyPath), ("v", $"{true}"));
            var disable = interactions.GenerateButton<ConfigValueButton>(("p", enablePropertyPath), ("v", $"{false}"));

            var menu = new DiscordInteractionResponseBuilder()
                .AddEmbed(currentConfig)
                .AddComponents(
                    new DiscordButtonComponent(ButtonStyle.Success, enable, "Enable Module", emoji: new DiscordComponentEmoji("✅")),
                    new DiscordButtonComponent(ButtonStyle.Danger, disable, "Disable Module", emoji: new DiscordComponentEmoji("⛔")))
                .AddComponents(new DiscordChannelSelectComponent("config_sb_channel", "Change Starboard Channel...", new List<ChannelType>() { ChannelType.Text }))
                .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, interactions.GenerateButton<ConfigMenu>(), "Back to menu", emoji: new DiscordComponentEmoji("👈")));

            await interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, menu);
        }
    }
}
