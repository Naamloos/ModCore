using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using ModCore.Extensions.Attributes;
using ModCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModCore.Extensions.Abstractions;

namespace ModCore.Modals
{
    [Modal("poll")]
    public class PollModal : IModal
    {
        [ModalField("Poll title", "title")]
        public string Title { get; set; }

        [ModalField("How long will this poll run?", "duration")]
        public string Duration { get; set; }

        [ModalField("Poll options, separated new lines", "options", style: TextInputStyle.Paragraph)]
        public string Options { get; set; }

        private DiscordClient client;

        public PollModal(DiscordClient client)
        {
            this.client = client;
        }

        public async Task HandleAsync(DiscordInteraction interaction)
        {
            // Setup for this poll
            var splitOptions = Options.Split('\n', StringSplitOptions.TrimEntries);
            Dictionary<ulong, int> responses = new Dictionary<ulong, int>();

            if (splitOptions.Count() < 2)
            {
                await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("⚠️ Polls need at least two options!").AsEphemeral());
            }

            if (splitOptions.Count() > 4)
            {
                await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("⚠️ Polls may only have up to four options!").AsEphemeral());
            }

            var (duration, _) = Dates.ParseTime(Duration);

            if (duration > TimeSpan.FromHours(2)) // limit to 2 hours.
            {
                await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("⚠️ Maximum allowed time span to run a poll is 2 hours.").AsEphemeral());
                return;
            }

            await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("✅ Creating your poll!").AsEphemeral());

            // Build poll with buttons n shit
            StringBuilder pollText = new StringBuilder($"**✏️ Poll: {Title}**\n\n");

            var message = new DiscordMessageBuilder();

            List<DiscordComponent> buttons = new List<DiscordComponent>();
            List<DiscordComponent> disabledButtons = new List<DiscordComponent>();

            for (int i = 0; i < splitOptions.Length; i++)
            {
                var option = splitOptions[i];
                buttons.Add(new DiscordButtonComponent(ButtonStyle.Primary, i.ToString(),
                    (i + 1).ToString(), emoji: new DiscordComponentEmoji(DiscordEmoji.FromUnicode("✏️"))));
                disabledButtons.Add(new DiscordButtonComponent(ButtonStyle.Primary, i.ToString(),
                    (i + 1).ToString(), true, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("✏️"))));
                pollText.AppendLine($"**[{i + 1}]**: {splitOptions[i]}");
            }
            buttons.Add(new DiscordButtonComponent(ButtonStyle.Danger, "x", "Clear vote", emoji: new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🗑"))));

            message.WithContent(pollText.ToString());
            message.AddComponents(buttons);

            var msg = await interaction.Channel.SendMessageAsync(message);

            // Temporary event handler for this specific poll.
            Task handlePoll(DiscordClient sender, ComponentInteractionCreateEventArgs e)
            {
                _ = Task.Run(async () =>
                {
                    // check message ID is correct
                    if (e.Message.Id == msg.Id)
                    {
                        if (e.Interaction.Data.CustomId == "x")
                        {
                            responses.Remove(e.User.Id);
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .WithContent("✅ Cleared your poll response (if any).").AsEphemeral());
                            return;
                        }

                        if (!int.TryParse(e.Interaction.Data.CustomId, out var index))
                        {
                            return;
                        }

                        if (responses.Any(x => x.Key == e.User.Id))
                        {
                            responses[e.User.Id] = index;
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .WithContent($"✅ Updated your poll vote to `{splitOptions[index]}`.").AsEphemeral());
                        }
                        else
                        {
                            responses.Add(e.User.Id, index);
                            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .WithContent($"✅ Set your poll vote to `{splitOptions[index]}`.").AsEphemeral());
                        }
                    }
                });

                return Task.CompletedTask;
            }

            client.ComponentInteractionCreated += handlePoll;
            await Task.Delay(duration);
            client.ComponentInteractionCreated -= handlePoll;

            // poll ended! time to tally our results.
            StringBuilder results = new StringBuilder($"**✏️ Poll Results: {Title}**\n\n");

            List<int> ordered = 
                responses.Select(x => x.Value)
                .GroupBy(x => x)
                .OrderByDescending(x => x.Count())
                .Select(x => x.Key)
                .ToList();

            for (int i = 0; i < splitOptions.Length; i++)
            {
                var medal = ordered.IndexOf(i) switch
                {
                    0 => "🥇",
                    1 => "🥈",
                    2 => "🥉",
                    _ => "❌"
                };

                results.AppendLine($"{medal} {splitOptions[i]} (**{responses.Where(x => x.Value == i).Count()} votes**)");
            }

            await msg.ModifyAsync(new DiscordMessageBuilder().WithContent(results.ToString()));
        }
    }
}
