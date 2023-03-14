using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ModCore.Integrations;
using System.Threading.Tasks;

namespace ModCore.ContextMenu
{
    public class MemberContextMenu : ApplicationCommandModule
    {
        public PronounDB PronounDB { get; set; }

        // Member context menu commands here. Max 5.
        [ContextMenu(ApplicationCommandType.UserContextMenu, "Show Pronouns")]
        public async Task PronounsAsync(ContextMenuContext ctx)
        {
            var pronouns = await PronounDB.GetPronounsForDiscordUserAsync(ctx.TargetUser.Id);
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AsEphemeral().WithContent($"According to [PronounDB](https://pronoundb.org/), {ctx.TargetUser.Mention}'s preferred pronouns are: `{pronouns}`."));
        }
    }
}
