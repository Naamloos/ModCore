using DSharpPlus;
using DSharpPlus.Entities;
using ModCore.Database;
using ModCore.Extensions;
using ModCore.Extensions.Buttons.Attributes;
using ModCore.Extensions.Buttons.Interfaces;
using ModCore.Utils.Extensions;
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

            List<DiscordComponent> options = new List<DiscordComponent>();

            var currentConfig = new DiscordEmbedBuilder()
                .WithTitle("Starboard Configuration")
                .WithDescription(settings.Starboard.Enable ? "✅ Module Enabled" : "⛔ Module Disabled")
                .AddField("Selected Channel", settings.Starboard.ChannelId > 0 ? $"<#{settings.Starboard.ChannelId}>" : "‼️ Not yet configured");

            var menu = new DiscordInteractionResponseBuilder()
                .AddEmbed(currentConfig)
                .AddComponents(
                    new DiscordButtonComponent(ButtonStyle.Success, "config_sb_enable", "Enable Module", emoji: new DiscordComponentEmoji("✅")),
                    new DiscordButtonComponent(ButtonStyle.Danger, "config_sb_disable", "Disable Module", emoji: new DiscordComponentEmoji("⛔")))
                .AddComponents(new DiscordChannelSelectComponent("config_sb_channel", "Change Starboard Channel...", new List<ChannelType>() { ChannelType.Text }))
                .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, client.GetButtons().GenerateCommand<ConfigMenu>(), "Back to menu", emoji: new DiscordComponentEmoji("👈")));

            await interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, menu);
        }
    }
}
