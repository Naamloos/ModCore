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
    public class RoleStateConfigComponents : BaseComponentModule
    {
        private DatabaseContextBuilder database;

        public RoleStateConfigComponents(DatabaseContextBuilder database)
        {
            this.database = database;
        }

        [Component("rs.t_role", ComponentType.Button)]
        public async Task ToggleRoleAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> values)
        {
            var enabled = values["on"] == "true";

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.RoleState.Enable = enabled;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 {(enabled ? "Enabled" : "Disabled")} restoring old member's roles.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "rs", "Back to Member State config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        [Component("rs.t_nick", ComponentType.Button)]
        public async Task ToggleNicknameAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> values)
        {
            var enabled = values["on"] == "true";

            await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral(true));

            using (var db = database.CreateContext())
            {
                var guild = db.GuildConfig.First(x => x.GuildId == (long)e.Guild.Id);
                var settings = guild.GetSettings();

                settings.RoleState.Nickname = enabled;
                guild.SetSettings(settings);
                db.GuildConfig.Update(guild);
                await db.SaveChangesAsync();
            }

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"👍 {(enabled ? "Enabled" : "Disabled")} restoring old member's nicknames.")
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "rs", "Back to Member State config", emoji: new DiscordComponentEmoji("🏃"))));
        }

        public static async Task PostMenuAsync(DiscordInteraction interaction, InteractionResponseType responseType, DatabaseContext db)
        {
            using (db)
            {
                var settings = interaction.Guild.GetGuildSettings(db).RoleState;

                var embed = new DiscordEmbedBuilder()
                    .WithTitle("🗿 Member State Configuration")
                    .WithDescription("Member states allow ModCore to restore roles and nicknames for old members that rejoin the server.")
                    .AddField("Roles", $"{(settings.Enable ? "✅" : "⛔")} Restoring roles is currently **{(settings.Enable ? "enabled" : "disabled")}**.")
                    .AddField("Roles", $"{(settings.Nickname ? "✅" : "⛔")} Restoring nicknames is currently **{(settings.Enable ? "enabled" : "disabled")}**.");

                var enableRoleId = ExtensionStatics.GenerateIdString("rs.t_role", new Dictionary<string, string>() { { "on", "true" } });
                var disableRoleId = ExtensionStatics.GenerateIdString("rs.t_role", new Dictionary<string, string>() { { "on", "false" } });

                var enableNickId = ExtensionStatics.GenerateIdString("rs.t_nick", new Dictionary<string, string>() { { "on", "true" } });
                var disableNickId = ExtensionStatics.GenerateIdString("rs.t_nick", new Dictionary<string, string>() { { "on", "false" } });

                var minimumOptions = new List<DiscordSelectComponentOption>();
                for (int i = 1; i <= 10; i++)
                    minimumOptions.Add(new DiscordSelectComponentOption($"{i}", $"{i}", $"Set Starboard minimum to {i}."));

                await interaction.CreateResponseAsync(responseType, new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed)
                    .WithContent("")
                    .AddComponents(new DiscordComponent[]
                    {
                        new DiscordButtonComponent(settings.Enable? ButtonStyle.Danger : ButtonStyle.Success, settings.Enable? disableRoleId : enableRoleId, settings.Enable? "Disable Role States" : "Enable Role States"),
                        new DiscordButtonComponent(settings.Nickname? ButtonStyle.Danger : ButtonStyle.Success, settings.Nickname? disableNickId : enableNickId, settings.Nickname? "Disable Nickname States" : "Enable Nickname States"),
                    })
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "cfg", "Back to Config", emoji: new DiscordComponentEmoji("🏃"))));
            }
        }
    }
}
