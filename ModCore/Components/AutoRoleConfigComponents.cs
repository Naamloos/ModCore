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
    public class AutoRoleConfigComponents : BaseComponentModule
    {
        private DatabaseContextBuilder database;

        public AutoRoleConfigComponents(DatabaseContextBuilder database)
        {
            this.database = database;
        }

        [Component("ar.toggle", ComponentType.Button)]
        public async Task ToggleAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> values)
        {
            var enabled = values["on"] == "true";

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            await using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.AutoRole.Enable = enabled;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 {(enabled ? "Enabled" : "Disabled")} auto roles.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "ar", "Back to Auto Role config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [Component("ar.select", ComponentType.RoleSelect)]
        public async Task SelectRolesAsync(ComponentInteractionCreateEventArgs e)
        {
            var roles = e.Interaction.Data.Resolved.Roles;

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            await using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.AutoRole.RoleIds = roles.Keys.ToList();
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 The following roles will now automatically be assigned to new members:" +
                $"\n{string.Join(',', roles.Keys.Select(x => $"<@&{x}>"))}")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "ar", "Back to Auto Role config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        public static async Task PostMenuAsync(DiscordInteraction interaction, InteractionResponseType responseType, DatabaseContext db)
        {
            await using (db)
            {
                var settings = interaction.Guild.GetGuildSettings(db).AutoRole;

                var embed = new DiscordEmbedBuilder()
                    .WithTitle("🤖 Auto Role Configuration")
                    .WithDescription("Auto Role allows ModCore to assign new roles to members when they join the server.")
                    .AddField("Enabled", $"{(settings.Enable ? "✅" : "⛔")} Auto Roles are currently **{(settings.Enable ? "enabled" : "disabled")}**.")
                    .AddField("Selected Roles", settings.RoleIds.Count > 0? string.Join(',', settings.RoleIds.Select(x => $"<@&{x}>")) : "No roles selected.");

                var enableId = ExtensionStatics.GenerateIdString("ar.toggle", new Dictionary<string, string>() { { "on", "true" } });
                var disableId = ExtensionStatics.GenerateIdString("ar.toggle", new Dictionary<string, string>() { { "on", "false" } });

                await interaction.CreateResponseAsync(responseType, new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed)
                    .WithContent("")
                    .AddComponents(new DiscordComponent[]
                    {
                        new DiscordButtonComponent(settings.Enable? ButtonStyle.Danger : ButtonStyle.Success, settings.Enable? disableId : enableId, settings.Enable? "Disable Auto Role" : "Enable Auto Role")
                    })
                    .AddComponents(new DiscordRoleSelectComponent("ar.select", "Select Auto Roles...", maxOptions: 5))
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "cfg", "Back to Config", emoji: new DiscordComponentEmoji("🏃"))));
            }
        }
    }
}
