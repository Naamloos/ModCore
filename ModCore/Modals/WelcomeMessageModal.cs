using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Extensions.Modals.Attributes;
using ModCore.Extensions.Modals.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Modals
{
    [Modal("welcome")]
    public class WelcomeMessageModal : IModal
    {
        [ModalField("New welcome message?", "welcome", "Welcome messages support a handful of tags that get parsed to their actual values:\r\n{{username}}, {{discriminator}}, {{mention}}, {{userid}},\r\n{{guildname}}, {{channelname}}, {{membercount}}, {{prefix}},\r\n{{owner-username}}, {{owner-discriminator}}, {{guild-icon-url}}, {{channel-count}}, {{role-count}},\r\n{{attach:url}}, {{embed-title:title}}, {{isembed}}", 
            null, true, TextInputStyle.Paragraph, 10, 255)]
        public string Welcome { get; set; }

        private DiscordClient client;

        public WelcomeMessageModal(DiscordClient client, Settings settings)
        {
            this.client = client;
        }

        public async Task HandleAsync(DiscordInteraction interaction)
        {
            var member = await interaction.Guild.GetMemberAsync(interaction.User.Id);
            if(!member.Permissions.HasPermission(Permissions.ManageGuild))
            {
                return;
            }

            var db = ((DatabaseContextBuilder)client.GetCommandsNext().Services.GetService(typeof(DatabaseContextBuilder))).CreateContext();
            var settings = db.GuildConfig.FirstOrDefault(x => x.GuildId == (long)interaction.GuildId)?.GetSettings();
            if(settings == null)
            {
                await interaction.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().WithContent("❌ No guild config?? contact devs!!1").AsEphemeral());
                return;
            }

            settings.Welcome.Message = Welcome;

            await interaction.CreateFollowupMessageAsync(
                new DiscordFollowupMessageBuilder().WithContent("✅ Welcome message was configured!").AsEphemeral());
        }
    }
}
