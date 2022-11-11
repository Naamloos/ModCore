using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Extensions.Abstractions;
using ModCore.Extensions.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModCore.Components
{
    public class UtilComponents : BaseComponentModule
    {
        [Component("del", ComponentType.Button)]
        public async Task UnSnipeAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> context)
        {
            var allowedUser = context["u"];

            if(ulong.TryParse(allowedUser, out var userId))
            {
                if(e.User.Id == userId)
                {
                    await e.Message.DeleteAsync();
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent($"✅ Alright, it's gone!").AsEphemeral());
                }
                else
                {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent($"⚠️ Only <@{userId}> can use this button to delete this message!").AsEphemeral());
                }
            }
        }
    }
}
