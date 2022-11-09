using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Database;
using ModCore.Extensions;
using ModCore.Extensions.Abstractions;
using ModCore.Extensions.Attributes;
using ModCore.Utils.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Components
{
    [ComponentPermissions(Permissions.ManageGuild)]
    public class LevelSystemConfigComponents : BaseComponentModule
    {
        private DatabaseContextBuilder database;

        public LevelSystemConfigComponents(DatabaseContextBuilder database)
        {
            this.database = database;
        }

        [Component("lv.toggle", ComponentType.Button)]
        public async Task ToggleAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> values)
        {
            var enabled = values["on"] == "true";

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.Levels.Enabled = enabled;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 {(enabled ? "Enabled" : "Disabled")} level system.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "lv", "Back to Level System config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [Component("lv.msg", ComponentType.Button)]
        public async Task ToggleMessagesAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> values)
        {
            var enabled = values["on"] == "true";

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.Levels.MessagesEnabled = enabled;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 {(enabled ? "Enabled" : "Disabled")} sending level-up messages.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "lv", "Back to Level System config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [Component("lv.redir", ComponentType.Button)]
        public async Task ToggleRedirectAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> values)
        {
            var enabled = values["on"] == "true";

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.Levels.RedirectMessages = enabled;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 {(enabled ? "Enabled" : "Disabled")} redirecting level-up messages.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "lv", "Back to Level System config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [Component("lv.channel", ComponentType.ChannelSelect)]
        public async Task SetChannelAsync(ComponentInteractionCreateEventArgs e)
        {
            var value = e.Interaction.Data.Resolved.Channels.First();
            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.Levels.ChannelId = value.Key;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 Set your level-up redirect channel to <#{value.Key}>.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "lv", "Back to Level System config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        public static async Task PostMenuAsync(DiscordInteraction interaction, InteractionResponseType responseType, DatabaseContext db)
        {
            using (db)
            {
                var settings = interaction.Guild.GetGuildSettings(db).Levels;

                var embed = new DiscordEmbedBuilder()
                    .WithTitle("📈 Level System Configuration")
                    .WithDescription("ModCore's level system allows members to earn \"experience\" through server activity. " +
                        "This feature also includes a top list that contains the top members in this server. " +
                        "This feature could also be considered a \"gamified\" method for assessing member activity.")
                    .AddField("Enabled", $"{(settings.Enabled ? "✅" : "⛔")} The level system is currently **{(settings.Enabled ? "enabled" : "disabled")}**.")
                    .AddField("Send messages", $"{(settings.MessagesEnabled ? "✅" : "⛔")} Level-up messages are currently {(settings.MessagesEnabled ? "enabled" : "disabled")}")
                    .AddField("Redirecting", $"{(settings.RedirectMessages ? "✅" : "⛔")} Level-up messages are currently {(settings.RedirectMessages ? "" : "not ")}" +
                        $"being redirected to selected channel: <#{settings.ChannelId}>");

                var enableId = ExtensionStatics.GenerateIdString("lv.toggle", new Dictionary<string, string>() { { "on", "true" } });
                var disableId = ExtensionStatics.GenerateIdString("lv.toggle", new Dictionary<string, string>() { { "on", "false" } });

                var enableMessagesId = ExtensionStatics.GenerateIdString("lv.msg", new Dictionary<string, string>() { { "on", "true" } });
                var disableMessagesId = ExtensionStatics.GenerateIdString("lv.msg", new Dictionary<string, string>() { { "on", "false" } });

                var enableRedirectId = ExtensionStatics.GenerateIdString("lv.redir", new Dictionary<string, string>() { { "on", "true" } });
                var disableRedirectId = ExtensionStatics.GenerateIdString("lv.redir", new Dictionary<string, string>() { { "on", "false" } });

                await interaction.CreateResponseAsync(responseType, new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed)
                    .WithContent("")
                    .AddComponents(new DiscordComponent[]
                    {
                        new DiscordButtonComponent(settings.Enabled? ButtonStyle.Danger : ButtonStyle.Success, settings.Enabled? disableId : enableId, settings.Enabled? "Disable Level System" : "Enable Level System"),
                        new DiscordButtonComponent(settings.MessagesEnabled? ButtonStyle.Danger : ButtonStyle.Success, settings.MessagesEnabled? disableMessagesId : enableMessagesId, settings.MessagesEnabled? "Disable Messages" : "Enable Messages"),
                        new DiscordButtonComponent(settings.RedirectMessages? ButtonStyle.Danger : ButtonStyle.Success, settings.RedirectMessages? disableRedirectId : enableRedirectId, settings.RedirectMessages? "Disable Redirecting Messages" : "Enable Redirecting Messages"),
                    })
                    .AddComponents(new DiscordChannelSelectComponent("lv.channel", "Select redirect channel...", new List<ChannelType>() { ChannelType.Text }))
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "cfg", "Back to Config", emoji: new DiscordComponentEmoji("🏃"))));
            }
        }
    }
}
