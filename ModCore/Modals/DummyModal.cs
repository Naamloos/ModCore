using DSharpPlus;
using DSharpPlus.Entities;
using ModCore.Entities;
using ModCore.Extensions.Modals.Attributes;
using ModCore.Extensions.Modals.Interfaces;
using System.Threading.Tasks;

namespace ModCore.Modals
{
    [Modal("dummy")]
    public class DummyModal : IModal
    {
        [ModalField("test field for testing", "test")]
        public string TestField { get; set; }

        [ModalHiddenField("hidden")]
        public string TestHiddenField { get; set; }

        private SharedData shared;

        public DummyModal(SharedData shared)
        {
            this.shared = shared;
        }

        public async Task HandleAsync(DiscordInteraction interaction)
        {
            await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                new DiscordInteractionResponseBuilder()
                .WithContent($"Working!\ntestfield=`{TestField}`\ntesthiddenfield=`{TestHiddenField}`\ndependency injection, bot id from shareddate=`{shared.ModCore.Settings.BotId}`")
                .AsEphemeral());
        }
    }
}
