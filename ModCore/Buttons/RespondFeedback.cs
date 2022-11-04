using DSharpPlus;
using DSharpPlus.Entities;
using ModCore.Extensions;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Interfaces;
using ModCore.Modals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModCore.Buttons
{
    [Button("fb")]
    public class RespondFeedback : IButton
    {
        [ButtonField("u")]
        public string UserId { get; set; }

        [ButtonField("g")]
        public string GuildId { get; set; }

        private DiscordClient client;

        public RespondFeedback(DiscordClient client)
        {
            this.client = client;
        }

        public async Task HandleAsync(DiscordInteraction interaction, DiscordMessage msg)
        {
            if(ulong.TryParse(UserId, out _) && ulong.TryParse(GuildId, out _))
            {
                await client.GetInteractionExtension().RespondWithModalAsync<FeedbackResponseModal>(interaction, "Response to feedback.",
                    new Dictionary<string, string>()
                    {
                        { "g", GuildId },
                        { "u", UserId }
                    });
            }
        }
    }
}
