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

namespace ModCore.Modals
{
    [Modal("postrm")]
    public class PostRoleMenuModal : IModal
    {
        [ModalField("What message should we attach?", "txt", "Open Role Menu!", null, true, TextInputStyle.Short, 0, 255)]
        public string Text { get; set; }

        [ModalHiddenField("n")]
        public string Name { get; set; }

        private DiscordClient client;

        public PostRoleMenuModal(DiscordClient client, Settings settings)
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

                if (!settings.RoleMenus.Any(x => x.Name == Name))
                {
                    await interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"⛔ A role menu with that name doesn't exist!")
                        .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "rm", "Back to Role Menu config", emoji: new DiscordComponentEmoji("🏃"))));
                    return;
                }

                var menu = settings.RoleMenus.First(x => x.Name == Name);

                var customId = ExtensionStatics.GenerateIdString("rolemenu", new Dictionary<string, string>()
                {
                    { "n", Name }
                });

                var options = new List<DiscordSelectComponentOption>();
                var roles = interaction.Guild.Roles.Values.Where(x => menu.RoleIds.Contains(x.Id));
                foreach(var role in roles)
                {
                    options.Add(new DiscordSelectComponentOption(role.Name, role.Id.ToString()));
                }

                await interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AsEphemeral().WithContent($"✅"));

                await interaction.Channel.SendMessageAsync(new DiscordMessageBuilder().WithContent(string.IsNullOrEmpty(Text)? "Open Role Menu" : Text)
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Primary, customId, "Select roles...")));

                await interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"✅ Role menu was posted in this channel!")
                        .AddComponents(new DiscordButtonComponent(ButtonStyle.Secondary, "rm", "Back to Role Menu config", emoji: new DiscordComponentEmoji("🏃"))));
            }
        }
    }
}
