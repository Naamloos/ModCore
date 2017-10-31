using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using ModCore.Entities;

namespace ModCore.Logic.Utils
{
    // TODO dependency injection
    public static partial class Utils
    {
        /// <summary>
        /// Tries to figure out, through enterprise-level heuristic analysis, 
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static async Task<(DiscordRole Role, string Message)> SetupMuteRole(DiscordGuild guild)
        {
            // TODO: make this method take DiscordMember, and heuristic test if the member would be able to speak in any channel with each role. if none apply, create new.
            var channels = guild.Channels.Where(e => e.Type == ChannelType.Text).Select(e => (channel: e, overwrites: e.GetPermissionOverwrites())).ToArray();
            foreach (var role in guild.Roles)
            {
                if (channels.All(e => e.overwrites.Any(ew =>
                    ew.Type == OverwriteType.Role && ew.Id == role.Id && ew.Deny.HasPermission(Permissions.SendMessages))))
                {
                    return (role, $"using existing role {role.Name}");
                }
            }
            foreach (var role in guild.Roles)
            {
                if (role.CheckPermission(Permissions.SendMessages) != PermissionLevel.Unset ||
                    !role.Name.ToLowerInvariant().Contains("mute")) continue;
                
                foreach (var (channel, overwrites) in channels)
                {
                    // don't tamper with existing overwrites
                    if (overwrites.Any(e => e.Id == role.Id)) continue;
                    
                    await channel.AddOverwriteAsync(role, Permissions.None, Permissions.SendMessages,
                        "ModCore automatic mute role channel overwrite for preexisting role");
                }
                return (role, $"using existing role `{role.Name}`");
            }
            var arole = await guild.CreateRoleAsync("ModCore Chat Mute", null, null, false, false, "ModCore automatic mute role configuration");
            foreach (var (channel, _) in channels)
            {
                await channel.AddOverwriteAsync(arole, Permissions.None, Permissions.SendMessages,
                    "ModCore automatic mute role channel overwrite");
            }
            return (arole, "automatically created it");
        }
    }
}