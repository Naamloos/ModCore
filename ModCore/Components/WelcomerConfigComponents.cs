using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Database;
using ModCore.Extensions;
using ModCore.Extensions.Abstractions;
using ModCore.Extensions.Attributes;
using ModCore.Modals;
using ModCore.Utils.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Components
{
    [ComponentPermissions(Permissions.ManageGuild)]
    public class WelcomerConfigComponents : BaseComponentModule
    {
        private DatabaseContextBuilder database;

        public WelcomerConfigComponents(DatabaseContextBuilder database)
        {
            this.database = database;
        }

        [Component("wc.toggle", ComponentType.Button)]
        public async Task ToggleAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> values)
        {
            var enabled = values["on"] == "true";

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            await using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.Welcome.Enable = enabled;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 {(enabled ? "Enabled" : "Disabled")} welcomer.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "wc", "Back to Welcomer config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [Component("wc.embed", ComponentType.Button)]
        public async Task ToggleEmbedAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> values)
        {
            var enabled = values["on"] == "true";

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            await using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.Welcome.IsEmbed = enabled;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 {(enabled ? "Enabled" : "Disabled")} sending welcomer message in an embed.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "wc", "Back to Welcomer config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [Component("wc.channel", ComponentType.ChannelSelect)]
        public async Task SetChannelAsync(ComponentInteractionCreateEventArgs e)
        {
            var value = e.Interaction.Data.Resolved.Channels.First();
            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            await using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.Welcome.ChannelId = value.Key;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 Set your welcomer channel to <#{value.Key}>.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "wc", "Back to Welcomer config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [Component("wc.set", ComponentType.Button)]
        public async Task ShowSetMessageModalAsync(ComponentInteractionCreateEventArgs e)
        {
            await Client.GetInteractionExtension().RespondWithModalAsync<WelcomeMessageModal>(e.Interaction, "Set Welcome Message");
        }

        public static async Task PostMenuAsync(DiscordInteraction interaction, InteractionResponseType responseType, DatabaseContext db)
        {
            await using (db)
            {
                var settings = interaction.Guild.GetGuildSettings(db).Welcome;

                var embed = new DiscordEmbedBuilder()
                    .WithTitle("👋 Welcomer Configuration")
                    .WithDescription("Welcomer allows you to post a simple customized message to a channel any time a new member joins.")
                    .AddField("Enabled", $"{(settings.Enable ? "✅" : "⛔")} Welcomer is currently **{(settings.Enable ? "enabled" : "disabled")}**.")
                    .AddField("Welcomer Channel", $"Welcome messages currently get posted to <#{settings.ChannelId}>")
                    .AddField("Embed", $"Welcome messages {(settings.IsEmbed? "" : "do not ")}get posted as an embed.")
                    .AddField("Current welcome message", string.IsNullOrEmpty(settings.Message)? "**Unset.**" : settings.Message);

                var enableId = ExtensionStatics.GenerateIdString("wc.toggle", new Dictionary<string, string>() { { "on", "true" } });
                var disableId = ExtensionStatics.GenerateIdString("wc.toggle", new Dictionary<string, string>() { { "on", "false" } });

                var enableEmbedId = ExtensionStatics.GenerateIdString("wc.embed", new Dictionary<string, string>() { { "on", "true" } });
                var disableEmbedId = ExtensionStatics.GenerateIdString("wc.embed", new Dictionary<string, string>() { { "on", "false" } });

                await interaction.CreateResponseAsync(responseType, new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed)
                    .WithContent("")
                    .AddComponents(new DiscordComponent[]
                    {
                        new DiscordButtonComponent(settings.Enable? ButtonStyle.Danger : ButtonStyle.Success, settings.Enable? disableId : enableId, settings.Enable? "Disable Welcomer" : "Enable Welcomer"),
                        new DiscordButtonComponent(settings.IsEmbed? ButtonStyle.Danger : ButtonStyle.Success, settings.IsEmbed? disableEmbedId : enableEmbedId, settings.IsEmbed? "Disable sending as embed" : "Enable sending as embed"),
                    })
                    .AddComponents(new DiscordComponent[]
                    {
                        new DiscordButtonComponent(ButtonStyle.Primary, "wc.set", "Set Welcome Message"),
                        new DiscordLinkButtonComponent("https://gist.github.com/Naamloos/a1c87c24ff238edbdd28258b08452ed4", "Welcomer Text Tag List")
                    })
                    .AddComponents(new DiscordChannelSelectComponent("wc.channel", "Select welcomer channel...", new List<ChannelType>() { ChannelType.Text }))
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "cfg", "Back to Config", emoji: new DiscordComponentEmoji("🏃"))));
            }
        }
    }
}
