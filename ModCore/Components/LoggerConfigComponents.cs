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
    public class LoggerConfigComponents : BaseComponentModule
    {
        private DatabaseContextBuilder database;

        public LoggerConfigComponents(DatabaseContextBuilder database)
        {
            this.database = database;
        }

        [Component("lg.join", ComponentType.Button)]
        public async Task ToggleJoinAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> values)
        {
            var enabled = values["on"] == "true";

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.Logging.JoinLog_Enable = enabled;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 {(enabled ? "Enabled" : "Disabled")} Join / Leave logging.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "lg", "Back to Logger config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [Component("lg.edit", ComponentType.Button)]
        public async Task ToggleEditAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> values)
        {
            var enabled = values["on"] == "true";

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.Logging.EditLog_Enable = enabled;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 {(enabled ? "Enabled" : "Disabled")} Message Update / Delete logging.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "lg", "Back to Logger config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [Component("lg.nick", ComponentType.Button)]
        public async Task ToggleNickAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> values)
        {
            var enabled = values["on"] == "true";

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.Logging.NickameLog_Enable = enabled;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 {(enabled ? "Enabled" : "Disabled")} Nickname Update logging.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "lg", "Back to Logger config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [Component("lg.inv", ComponentType.Button)]
        public async Task ToggleInvitesAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> values)
        {
            var enabled = values["on"] == "true";

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.Logging.InviteLog_Enable = enabled;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 {(enabled ? "Enabled" : "Disabled")} Invite logging.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "lg", "Back to Logger config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [Component("lg.avatar", ComponentType.Button)]
        public async Task ToggleAvatarAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> values)
        {
            var enabled = values["on"] == "true";

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.Logging.AvatarLog_Enable = enabled;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 {(enabled ? "Enabled" : "Disabled")} Avatar Update logging.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "lg", "Back to Logger config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [Component("lg.role", ComponentType.Button)]
        public async Task ToggleRolesAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> values)
        {
            var enabled = values["on"] == "true";

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.Logging.RoleLog_Enable = enabled;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 {(enabled ? "Enabled" : "Disabled")} Role Update logging.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "lg", "Back to Logger config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [Component("lg.mod", ComponentType.Button)]
        public async Task ToggleModAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> values)
        {
            var enabled = values["on"] == "true";

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.Logging.ModLog_Enable = enabled;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 {(enabled ? "Enabled" : "Disabled")} ModCore Moderator Action logging.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "lg", "Back to Logger config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [Component("lg.channel", ComponentType.ChannelSelect)]
        public async Task SetChannelAsync(ComponentInteractionCreateEventArgs e)
        {
            var value = e.Interaction.Data.Resolved.Channels.First();
            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.Logging.ChannelId = value.Key;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 Set your logging channel to <#{value.Key}>.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "lg", "Back to Logging config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        public static async Task PostMenuAsync(DiscordInteraction interaction, InteractionResponseType responseType, DatabaseContext db)
        {
            using (db)
            {
                var settings = interaction.Guild.GetGuildSettings(db).Logging;

                var embed = new DiscordEmbedBuilder()
                    .WithTitle("🪵 Logger Configuration")
                    .WithDescription("ModCore can log a bunch of actions to a special logging channel. All of these actions are separately configurable.")
                    .AddField("Logging Channel", $"Currently set to <#{settings.ChannelId}>")
                    .AddField("Join / Leave", settings.JoinLog_Enable? "✅ Enabled" : "⛔ Disabled", true)
                    .AddField("Message Edits", settings.EditLog_Enable ? "✅ Enabled" : "⛔ Disabled", true)
                    .AddField("Nickname Updates", settings.NickameLog_Enable ? "✅ Enabled" : "⛔ Disabled", true)
                    .AddField("Invite Creation", settings.InviteLog_Enable ? "✅ Enabled" : "⛔ Disabled", true)
                    .AddField("Avatar Updates", settings.AvatarLog_Enable ? "✅ Enabled" : "⛔ Disabled", true)
                    .AddField("Role Updates", settings.RoleLog_Enable ? "✅ Enabled" : "⛔ Disabled", true)
                    .AddField("Moderator Action", settings.ModLog_Enable ? "✅ Enabled" : "⛔ Disabled", true);

                var enableJoins = ExtensionStatics.GenerateIdString("lg.join", new Dictionary<string, string>() { { "on", "true" } });
                var disableJoins = ExtensionStatics.GenerateIdString("lg.join", new Dictionary<string, string>() { { "on", "false" } });

                var enabledEdits = ExtensionStatics.GenerateIdString("lg.edit", new Dictionary<string, string>() { { "on", "true" } });
                var disableEdits = ExtensionStatics.GenerateIdString("lg.edit", new Dictionary<string, string>() { { "on", "false" } });

                var enableNicknames = ExtensionStatics.GenerateIdString("lg.nick", new Dictionary<string, string>() { { "on", "true" } });
                var disableNicknames = ExtensionStatics.GenerateIdString("lg.nick", new Dictionary<string, string>() { { "on", "false" } });

                var enableInvites = ExtensionStatics.GenerateIdString("lg.inv", new Dictionary<string, string>() { { "on", "true" } });
                var disableInvites = ExtensionStatics.GenerateIdString("lg.inv", new Dictionary<string, string>() { { "on", "false" } });

                var enableAvatars = ExtensionStatics.GenerateIdString("lg.avatar", new Dictionary<string, string>() { { "on", "true" } });
                var disableAvatars = ExtensionStatics.GenerateIdString("lg.avatar", new Dictionary<string, string>() { { "on", "false" } });

                var enableRoles = ExtensionStatics.GenerateIdString("lg.role", new Dictionary<string, string>() { { "on", "true" } });
                var disableRoles = ExtensionStatics.GenerateIdString("lg.role", new Dictionary<string, string>() { { "on", "false" } });

                var enableModActions = ExtensionStatics.GenerateIdString("lg.mod", new Dictionary<string, string>() { { "on", "true" } });
                var disableModAction = ExtensionStatics.GenerateIdString("lg.mod", new Dictionary<string, string>() { { "on", "false" } });

                await interaction.CreateResponseAsync(responseType, new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed)
                    .WithContent("")
                    .AddComponents(new DiscordComponent[]
                    {
                        new DiscordButtonComponent(settings.JoinLog_Enable? ButtonStyle.Danger : ButtonStyle.Success, 
                            settings.JoinLog_Enable? disableJoins : enableJoins, settings.JoinLog_Enable? "Disable Join / Leave" : "Enable Join / Leave"),

                        new DiscordButtonComponent(settings.EditLog_Enable? ButtonStyle.Danger : ButtonStyle.Success,
                            settings.EditLog_Enable? disableEdits : enabledEdits, settings.EditLog_Enable? "Disable Messages" : "Enable Messages"),

                        new DiscordButtonComponent(settings.NickameLog_Enable? ButtonStyle.Danger : ButtonStyle.Success,
                            settings.NickameLog_Enable? disableNicknames : enableNicknames, settings.NickameLog_Enable? "Disable Nickname Update" : "Enable Nickname Update"),

                        new DiscordButtonComponent(settings.InviteLog_Enable? ButtonStyle.Danger : ButtonStyle.Success,
                            settings.InviteLog_Enable? disableInvites : enableInvites, settings.InviteLog_Enable? "Disable Invites" : "Enable Invites"),
                    })
                    .AddComponents(new DiscordComponent[]
                    {
                        new DiscordButtonComponent(settings.AvatarLog_Enable? ButtonStyle.Danger : ButtonStyle.Success,
                            settings.AvatarLog_Enable? disableAvatars : enableAvatars, settings.AvatarLog_Enable? "Disable Avatars" : "Enable Avatars"),

                        new DiscordButtonComponent(settings.RoleLog_Enable? ButtonStyle.Danger : ButtonStyle.Success,
                            settings.RoleLog_Enable? disableRoles : enableRoles, settings.RoleLog_Enable? "Disable Roles" : "Enable Roles"),

                        new DiscordButtonComponent(settings.ModLog_Enable? ButtonStyle.Danger : ButtonStyle.Success,
                            settings.ModLog_Enable? disableModAction : enableModActions, settings.ModLog_Enable? "Disable Mod Actions" : "Enable Mod Actions"),
                    })
                    .AddComponents(new DiscordChannelSelectComponent("lg.channel", "Select logging channel...", new List<ChannelType>() { ChannelType.Text }))
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "cfg", "Back to Config", emoji: new DiscordComponentEmoji("🏃"))));
            }
        }
    }
}
