using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Extensions.Abstractions;
using ModCore.Extensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.Components
{
    public class UtilComponents : BaseComponentModule
    {
        [Component("del", ComponentType.Button)]
        public async Task UnSnipeAsync(ComponentInteractionCreateEventArgs e, IDictionary<string, string> context)
        {
            var allowedUsers = context["u"].Split('|').Select(x => ulong.TryParse(x, out ulong res)? res : 0);

            if(allowedUsers.Contains(e.User.Id))
            {
                await e.Message.DeleteAsync();
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"✅ Alright, it's gone!").AsEphemeral());
            }
            else
            {
                var allowedUsersMention = string.Join(", ", allowedUsers.Select(x => $"<@{x}>"));

                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"⚠️ Only {allowedUsersMention} can use this button to delete this message!").AsEphemeral());
            }
        }
    }
}
