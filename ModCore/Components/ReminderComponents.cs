using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Extensions;
using ModCore.Extensions.Abstractions;
using ModCore.Extensions.Attributes;
using ModCore.Modals;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Components
{
    public class ReminderComponents : BaseComponentModule
    {
        [Component("snooze", ComponentType.Button)]
        public async Task SnoozeReminderAsync(ComponentInteractionCreateEventArgs e)
        {
            if (e.Message.MentionedUsers.Any(x => x.Id == e.User.Id))
            {
                await Client.GetInteractionExtension().RespondWithModalAsync<SnoozeModal>(e.Interaction, "Snooze reminder", new Dictionary<string, string>()
                {
                    { "msg", e.Message.Id.ToString() }
                });
            }
            else
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("⛔ This is not your reminder! You can only snooze your own reminders!").AsEphemeral());
            }
        }
    }
}
