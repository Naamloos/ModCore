using DSharpPlus;
using DSharpPlus.Entities;
using ModCore.Entities;
using ModCore.Extensions;
using ModCore.Extensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModCore.Extensions.Abstractions;
using ModCore.Utils.Extensions;

namespace ModCore.Modals
{
    [Modal("feedback")]
    public class FeedbackModal : IModal
    {
        [ModalField("What feedback would you like to give?", "feedback", "Hello, ...", null, true, TextInputStyle.Paragraph, 10, 255)]
        public string Feedback { get; set; }

        [ModalHiddenField("c")]
        public string Category { get; set; }

        public FeedbackType Type => Enum.TryParse(typeof(FeedbackType), Category, out object result) ? (FeedbackType)result : FeedbackType.Other;

        private DiscordClient client;
        private Settings settings;

        public FeedbackModal(DiscordClient client, Settings settings)
        {
            this.client = client;
            this.settings = settings;
        }

        public async Task HandleAsync(DiscordInteraction interaction)
        {
            var feedbackChannel = await client.GetChannelAsync(settings.ContactChannelId);

            var embed = new DiscordEmbedBuilder()
                .WithAuthor($"{interaction.User.GetDisplayUsername()} in {interaction.Guild.Name}", 
                    iconUrl: interaction.User.GetAvatarUrl(ImageFormat.Png))
                .WithTitle(Category)
                .WithDescription(Feedback)
                .WithColor(GetColor());

            var button = ExtensionStatics.GenerateIdString("fb", new Dictionary<string, string>()
            {
                {"u", interaction.User.Id.ToString() },
                {"g", interaction.Guild.Id.ToString() }
            });

            var message = new DiscordMessageBuilder()
                .WithEmbed(embed)
                .AddComponents(new DiscordButtonComponent(ButtonStyle.Success, button,
                    "Respond to feedback with DM", emoji: new DiscordComponentEmoji(DiscordEmoji.FromUnicode("💬"))));

            await feedbackChannel.SendMessageAsync(message);

            await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"✨ Your feedback has been submitted to the ModCore team! Thank you for your feedback! 💖").AsEphemeral());
        }

        private DiscordColor GetColor() => Type switch
        {
            FeedbackType.FeatureRequest => DiscordColor.Turquoise,
            FeedbackType.Bug => DiscordColor.Red,
            FeedbackType.GeneralFeedback => DiscordColor.Yellow,
            FeedbackType.Complaint => DiscordColor.DarkRed,
            _ => DiscordColor.Gray,
        };
    }

    public enum FeedbackType
    {
        GeneralFeedback,
        Bug,
        FeatureRequest,
        Complaint,
        Other
    }
}
