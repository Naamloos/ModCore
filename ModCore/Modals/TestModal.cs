using DSharpPlus;
using DSharpPlus.Entities;
using ModCore.Entities;
using ModCore.Extensions.Interfaces;
using ModCore.Extensions.Attributes;
using System.Threading.Tasks;

namespace ModCore.Modals
{
    [Modal("test")]
    public class TestModal : IModal
    {
        [ModalField("This is a visible field!", "visible")]
        public string TestField { get; set; }

        [ModalHiddenField("hidden")]
        public string TestHiddenField { get; set; }

        private Settings settings;

        public TestModal(Settings settings)
        {
            this.settings = settings;
        }

        public async Task HandleAsync(DiscordInteraction interaction)
        {
            await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                new DiscordInteractionResponseBuilder()
                .WithContent($"Working!\ntestfield=`{TestField}`\ntesthiddenfield=`{TestHiddenField}`\ndependency injection, bot id from shareddate=`{settings.BotId}`")
                .AsEphemeral());
        }
    }
}
