using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ModCore.Logic;

namespace ModCore.Listeners
{
    public static class InviteLinkRemover
    {
        [AsyncListener(EventTypes.MessageCreated)]
        public static  async Task RemoveInviteLinks(ModCoreShard bot, MessageCreateEventArgs e)
        {
            if (bot.Settings.BlockInvites && (e.Channel.PermissionsFor(e.Author as DiscordMember) & Permissions.ManageMessages) == 0)
            {
                var m = Regex.Match(e.Message.Content, @"discord(\.gg|app\.com\/invite)\/.+");
                if (m.Success)
                {
                    await e.Message.DeleteAsync("Discovered invite and deleted message");
                }
            }
        }
    }
}