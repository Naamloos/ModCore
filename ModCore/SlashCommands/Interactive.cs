using DSharpPlus.SlashCommands;
using ModCore.Extensions;
using ModCore.Modals;
using System.Threading.Tasks;

namespace ModCore.SlashCommands
{
    public class Interactive : ApplicationCommandModule
    {
        [SlashCommand("poll", "Starts a poll in this channel.")]
        public async Task PollAsync(InteractionContext ctx)
            => await ctx.Client.GetModalExtension().RespondWithModalAsync<PollModal>(ctx.Interaction, "Create poll...");
    }
}
