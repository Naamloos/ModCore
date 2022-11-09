using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.VisualBasic;
using ModCore.Database;
using ModCore.Database.JsonEntities;
using ModCore.Extensions.Abstractions;
using ModCore.Extensions.Attributes;
using ModCore.Utils.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            => await RoleStateConfigComponents.PostMenuAsync(e.Interaction, InteractionResponseType.UpdateMessage, database.CreateContext());

        [Component("ar", ComponentType.Button)]
        public async Task AutoRoleAsync(ComponentInteractionCreateEventArgs e)
            => await AutoRoleConfigComponents.PostMenuAsync(e.Interaction, InteractionResponseType.UpdateMessage, database.CreateContext());

        [Component("wc", ComponentType.Button)]
        public async Task WelcomerAsync(ComponentInteractionCreateEventArgs e)
            => await WelcomerConfigComponents.PostMenuAsync(e.Interaction, InteractionResponseType.UpdateMessage, database.CreateContext());

        [Component("lv", ComponentType.Button)]
        public async Task LevelSystemAsync(ComponentInteractionCreateEventArgs e)
            => await LevelSystemConfigComponents.PostMenuAsync(e.Interaction, InteractionResponseType.UpdateMessage, database.CreateContext());

        [Component("lg", ComponentType.Button)]
        public async Task UpdateLoggerAsync(ComponentInteractionCreateEventArgs e)
            => await LoggerConfigComponents.PostMenuAsync(e.Interaction, InteractionResponseType.UpdateMessage, database.CreateContext());

        [Component("nick", ComponentType.Button)]
        public async Task NicknameAsync(ComponentInteractionCreateEventArgs e)
            => await NicknameApprovalConfigComponents.PostMenuAsync(e.Interaction, InteractionResponseType.UpdateMessage, database.CreateContext());

        [Component("rm", ComponentType.Button)]
        public async Task RoleMenuAsync(ComponentInteractionCreateEventArgs e)
            => await RoleMenuConfigComponents.PostMenuAsync(e.Interaction, InteractionResponseType.UpdateMessage, database.CreateContext());

        [Component("cfg.dump", ComponentType.Button)]
        public async Task DumpConfigAsync(ComponentInteractionCreateEventArgs e)
        {
            using var db = database.CreateContext();
            var settings = db.GuildConfig.FirstOrDefault(x => x.GuildId == (long)e.Guild.Id).GetSettings();
            using var ms = new MemoryStream();
            using var sw = new StreamWriter(ms);
            sw.Write(JsonConvert.SerializeObject(settings, Formatting.Indented));
            sw.Flush();
            ms.Position = 0;

            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent("✅ Your ModCore Guild configuration dump is ready!")
                .AddFile($"config_{e.Guild.Id}.json", ms).AsEphemeral());
        }

        [Component("cfg.reset", ComponentType.Button)]
        public async Task ResetConfigAsync(ComponentInteractionCreateEventArgs e)
        {
            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder()
                .WithContent("❓ Are you sure you want to reset your server's config? You will lose your ModCore setup!")
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Primary, "cfg.reset.confirm", "Yes, I am sure!", emoji: new DiscordComponentEmoji("🗑")),
                    new DiscordButtonComponent(ButtonStyle.Danger, "cfg", "Oops, I didn't mean to click this!", emoji: new DiscordComponentEmoji("😨"))
                })
                .AsEphemeral());
        }

        [Component("cfg.reset.confirm", ComponentType.Button)]
        public async Task ResetConfigConfirmAsync(ComponentInteractionCreateEventArgs e)
        {
            using var db = database.CreateContext();
            var settings = db.GuildConfig.FirstOrDefault(x => x.GuildId == (long)e.Guild.Id);
            settings.SetSettings(new GuildSettings());
            db.GuildConfig.Update(settings);
            await db.SaveChangesAsync();

            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder()
                .WithContent("♻️ Your ModCore Guild configuration was reset!")
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Primary, "cfg", "Great! Bring me back to the config!", emoji: new DiscordComponentEmoji("🏃"))
                })
                .AsEphemeral());
        }

        public static async Task PostMenuAsync(DiscordInteraction interaction, InteractionResponseType responseType)
        {
            var response = new DiscordInteractionResponseBuilder()
                .WithContent("")
                .AsEphemeral()
                .AddEmbed(new DiscordEmbedBuilder().WithTitle($"<:modcore:996915638545158184> Welcome to the ModCore Server Configuration Utility!")
                    .WithDescription("Select one of the following modules to configure ModCore.")
                    .WithThumbnail("https://i.imgur.com/AzWYXOc.png"))
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Primary, "sb", "Starboard", emoji: new DiscordComponentEmoji("⭐")),
                    new DiscordButtonComponent(ButtonStyle.Primary, "rs", "Member States", emoji: new DiscordComponentEmoji("🗿")),
                    //new DiscordButtonComponent(ButtonStyle.Secondary, "lf", "Link Filters", emoji: new DiscordComponentEmoji("🔗")),
                    new DiscordButtonComponent(ButtonStyle.Primary, "ar", "Auto Role", emoji: new DiscordComponentEmoji("🤖")),
                    new DiscordButtonComponent(ButtonStyle.Primary, "nick", "Nickname Approval", emoji: new DiscordComponentEmoji("📝"))
                })
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Primary, "lv", "Level System", emoji: new DiscordComponentEmoji("📈")),
                    new DiscordButtonComponent(ButtonStyle.Primary, "lg", "Update Logger", emoji: new DiscordComponentEmoji("🪵")),
                    new DiscordButtonComponent(ButtonStyle.Primary, "wc", "Welcomer", emoji: new DiscordComponentEmoji("👋")),
                    new DiscordButtonComponent(ButtonStyle.Primary, "rm", "Role Menu", emoji: new DiscordComponentEmoji("📖"))
                })
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Secondary, "cfg.reset", "Reset Server Configuration", emoji: new DiscordComponentEmoji("🗑")),
                    new DiscordButtonComponent(ButtonStyle.Secondary, "cfg.dump", "Dump JSON (Advanced)", emoji: new DiscordComponentEmoji("📩"))
                })
                .AddFiles(new Dictionary<string, Stream>());

            await interaction.CreateResponseAsync(responseType, response);
        }
    }
}
