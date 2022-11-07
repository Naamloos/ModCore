using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Database;
using ModCore.Extensions.Abstractions;
using ModCore.Extensions.Attributes;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ModCore.Components
{
    [ComponentPermissions(Permissions.ManageGuild)]
    public class ConfigComponents : BaseComponentModule
    {
        private DatabaseContextBuilder database;
        public ConfigComponents(DatabaseContextBuilder database)
        {
            this.database = database;
        }

        [Component("cfg", ComponentType.Button)]
        public async Task ConfigMenuAsync(ComponentInteractionCreateEventArgs e)
            => await PostMenuAsync(e.Interaction, InteractionResponseType.UpdateMessage);

        [Component("sb", ComponentType.Button)]
        public async Task StarboardAsync(ComponentInteractionCreateEventArgs e)
            => await StarboardConfigComponents.PostMenuAsync(e.Interaction, InteractionResponseType.UpdateMessage, database.CreateContext());

        [Component("rs", ComponentType.Button)]
        public async Task RoleStateAsync(ComponentInteractionCreateEventArgs e)
            => await WipAsync(e);

        [Component("lf", ComponentType.Button)]
        public async Task LinkFiltersAsync(ComponentInteractionCreateEventArgs e)
            => await WipAsync(e);

        [Component("ar", ComponentType.Button)]
        public async Task AutoRoleAsync(ComponentInteractionCreateEventArgs e)
            => await WipAsync(e);

        [Component("rm", ComponentType.Button)]
        public async Task RoleMenuAsync(ComponentInteractionCreateEventArgs e)
            => await WipAsync(e);

        [Component("wc", ComponentType.Button)]
        public async Task WelcomerAsync(ComponentInteractionCreateEventArgs e)
            => await WipAsync(e);

        [Component("ls", ComponentType.Button)]
        public async Task LevelSystemAsync(ComponentInteractionCreateEventArgs e)
            => await WipAsync(e);

        [Component("ul", ComponentType.Button)]
        public async Task UpdateLoggerAsync(ComponentInteractionCreateEventArgs e)
            => await WipAsync(e);

        private async Task WipAsync(ComponentInteractionCreateEventArgs e, [CallerMemberName] string caller = "")
        {
            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder()
                .WithContent($"Page WIP {caller}")
                .AsEphemeral()
                .AddComponents(new DiscordButtonComponent(ButtonStyle.Danger, "cfg", "Back to Main Menu", emoji: new DiscordComponentEmoji("🏃"))));
        }

        public static async Task PostMenuAsync(DiscordInteraction interaction, InteractionResponseType responseType)
        {
            var response = new DiscordInteractionResponseBuilder()
                .WithContent("")
                .AsEphemeral()
                .AddEmbed(new DiscordEmbedBuilder().WithTitle($"<:modcore:996915638545158184> Welcome to the ModCore Configuration Utility!")
                    .WithDescription("Select one of the modules below to configure ModCore.")
                    .WithThumbnail("https://i.imgur.com/AzWYXOc.png"))
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Secondary, "sb", "Starboard", emoji: new DiscordComponentEmoji("⭐")),
                    new DiscordButtonComponent(ButtonStyle.Secondary, "rs", "Role State", emoji: new DiscordComponentEmoji("🗿")),
                    new DiscordButtonComponent(ButtonStyle.Secondary, "lf", "Link Filters", emoji: new DiscordComponentEmoji("🔗")),
                    new DiscordButtonComponent(ButtonStyle.Secondary, "ar", "Auto Role", emoji: new DiscordComponentEmoji("🤖"))
                })
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Secondary, "rm", "Role Menu", emoji: new DiscordComponentEmoji("📖")),
                    new DiscordButtonComponent(ButtonStyle.Secondary, "wc", "Welcomer", emoji: new DiscordComponentEmoji("👋")),
                    new DiscordButtonComponent(ButtonStyle.Secondary, "ls", "Level System", emoji: new DiscordComponentEmoji("📈")),
                    new DiscordButtonComponent(ButtonStyle.Secondary, "ul", "Update Logger", emoji: new DiscordComponentEmoji("🪵"))
                });

            await interaction.CreateResponseAsync(responseType, response);
        }
    }
}
