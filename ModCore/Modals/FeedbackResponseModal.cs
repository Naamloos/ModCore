using DSharpPlus;
using DSharpPlus.Entities;
using ModCore.Extensions.Interfaces;
using ModCore.Extensions.Attributes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Modals
{
    [Modal("fbr")]
    public class FeedbackResponseModal : IModal
    {
        [ModalHiddenField("u")]
        public string UserId { get; set; }
        [ModalHiddenField("g")]
        public string GuildId { get; set; }

        [ModalField("Response to feedback?", "response", style: TextInputStyle.Paragraph, required: true)]
        public string Response { get; set; }

        private DiscordClient client { get; set; }

        public FeedbackResponseModal(DiscordClient client)
        {
            this.client = client;
        }

        public async Task HandleAsync(DiscordInteraction interaction)
        {
            if (client.CurrentApplication.Owners.Any(x => x.Id == interaction.User.Id) 
                && ulong.TryParse(UserId, out var id) && ulong.TryParse(GuildId, out var gid))
            {
                var resp = new DiscordInteractionResponseBuilder().AsEphemeral();
                try
                {
                    var guild = await client.GetGuildAsync(gid);
                    var member = await guild.GetMemberAsync(id);
                    await member.SendMessageAsync(new DiscordEmbedBuilder()
                        .WithAuthor($"{interaction.User.Username}#{interaction.User.Discriminator}", iconUrl: interaction.User.GetAvatarUrl(ImageFormat.Png))
                        .WithTitle("Response to your feedback!")
                        .WithDescription(Response)
                        .WithFooter("Thank you for using ModCore!")
                        .WithColor(new DiscordColor("#089FDF")));
                    resp.WithContent("✅ Successfully sent response to feedback!");
                }
                catch (Exception ex)
                {
                    resp.WithContent("⚠️ Failed to DM member. Do they use ModCore still? Are their DMs disabled? We may never know.");
                }

                await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, resp);
            }
        }
    }
}
