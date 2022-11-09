using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Extensions;
using ModCore.Extensions.Abstractions;
using ModCore.Extensions.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModCore.Modals
{
    [Modal("rolemenuname")]
    public class RoleMenuNameModal : IModal
    {
        [ModalField("What name should this new role menu get?", "name", "mymenu", null, true, TextInputStyle.Short, 3, 25)]
        public string Name { get; set; }

        private DiscordClient client;

        public RoleMenuNameModal(DiscordClient client, Settings settings)
        {
            this.client = client;
        }

        public async Task HandleAsync(DiscordInteraction interaction)
        {
            var member = await interaction.Guild.GetMemberAsync(interaction.User.Id);
            if (!member.Permissions.HasPermission(Permissions.ManageGuild))
            {
                return;
            }

            using (var db = ((DatabaseContextBuilder)client.GetCommandsNext().Services.GetService(typeof(DatabaseContextBuilder))).CreateContext())
            {
                var guildConfig = db.GuildConfig.FirstOrDefault(x => x.GuildId == (long)interaction.GuildId);
                var settings = guildConfig?.GetSettings();
                if (settings == null)
                {
                    await interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"⛔ No guild config??? contact devs!!1")
                        .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "rm", "Back to Role Menu config", emoji: new DiscordComponentEmoji("🏃"))));
                    return;
                }

                if(settings.RoleMenus.Any(x => x.Name == Name))
                {
                    await interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"⛔ A role menu with that name already exists!")
                        .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "rm", "Back to Role Menu config", emoji: new DiscordComponentEmoji("🏃"))));
                    return;
                }

                var customId = ExtensionStatics.GenerateIdString("rm.setroles", new Dictionary<string, string>()
                {
                    { "n", Name }
                });

                await interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"📝 Pick the roles for this new menu...")
                        .AddComponents(new DiscordRoleSelectComponent(customId, "Select roles...", maxOptions: 25)));
            }
        }
    }
}
