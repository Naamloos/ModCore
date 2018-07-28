using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using ModCore.Logic.Extensions;

namespace ModCore.Logic.Utils
{
    // TODO dependency injection
    public static class Utils
    {
        /// <summary>
        /// Tries to find, through enterprise-level heuristic analysis, a valid mute role. If not found, creates one.
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="callee"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public static async Task<(DiscordRole Role, string Message)> SetupMuteRole(DiscordGuild guild,
            DiscordMember callee, DiscordMember member)
        {
            var textChannels = guild.Channels.Where(e => e.Type == ChannelType.Text).ToArray();
            var candidateRoles = new List<DiscordRole>();
            foreach (var role in guild.Roles)
            {
                AddPotentialCandidate(member, textChannels, role, candidateRoles);
            }

            var lastRole = candidateRoles.OrderByDescending(e => e.Position).FirstOrDefault();
            if (lastRole != default && callee.Roles.All(e => e.Id != lastRole.Id))
            {
                return (lastRole,
                    $"using existing role {lastRole.Name} since callee does not have it and cannot speak in any channel."
                    );
            }

            var channels = textChannels.Select(e => (channel: e, overwrites: e.GetPermissionOverwrites())).ToArray();
            foreach (var role in guild.Roles)
            {
                if (channels.All(e => e.overwrites.Any(ew =>
                    ew.Type == OverwriteType.Role && ew.Id == role.Id &&
                    ew.Deny.HasPermission(Permissions.SendMessages))))
                {
                    return (role, $"using existing role {role.Name} since has override to not speak in any channel");
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
            var arole = await guild.CreateRoleAsync("ModCore Chat Mute", null, null, false, false,
                "ModCore automatic mute role configuration");
            foreach (var (channel, _) in channels)
            {
                await channel.AddOverwriteAsync(arole, Permissions.None, Permissions.SendMessages,
                    "ModCore automatic mute role channel overwrite");
            }
            return (arole, "automatically created it");
        }

        private static void AddPotentialCandidate(DiscordMember member, IEnumerable<DiscordChannel> textChannels, DiscordRole role,
            ICollection<DiscordRole> candidateRoles)
        {
            if (textChannels.Any(channel => PermissionsFor(channel, member, role).HasPermission(Permissions.SendMessages)))
            {
                return;
            }

            candidateRoles.Add(role);
        }

        public static Permissions PermissionsFor(DiscordChannel chan, DiscordMember mbr,
            params DiscordRole[] potentialRoles)
        {
            // default permissions
            const Permissions def = Permissions.None;

            // future note: might be able to simplify @everyone role checks to just check any role ... but i'm not sure
            // xoxo, ~uwx
            //
            // you should use a single tilde
            // ~emzi

            // user > role > everyone
            // allow > deny > undefined
            // =>
            // user allow > user deny > role allow > role deny > everyone allow > everyone deny
            // thanks to meew0

            if (chan.IsPrivate || chan.Guild == null)
                return def;

            if (chan.Guild.Owner.Id == mbr.Id)
                return ~def;

            Permissions perms;

            // assign @everyone permissions
            var everyoneRole = chan.Guild.EveryoneRole;
            perms = everyoneRole.Permissions;

            // roles that member is in
            var mbRoles = mbr.Roles.Where(xr => xr.Id != everyoneRole.Id).Concat(potentialRoles).ToArray();
            // channel overrides for roles that member is in
            var mbRoleOverrides = mbRoles
                .Select(xr => chan.PermissionOverwrites.FirstOrDefault(xo => xo.Id == xr.Id))
                .Where(xo => xo != null)
                .ToList();

            // assign permissions from member's roles (in order)
            perms |= mbRoles.Aggregate(def, (c, role) => c | role.Permissions);

            // assign channel permission overwrites for @everyone pseudo-role
            var everyoneOverwrites = chan.PermissionOverwrites.FirstOrDefault(xo => xo.Id == everyoneRole.Id);
            if (everyoneOverwrites != null)
            {
                perms &= ~everyoneOverwrites.Denied;
                perms |= everyoneOverwrites.Allowed;
            }

            // assign channel permission overwrites for member's roles (explicit deny)
            perms &= ~mbRoleOverrides.Aggregate(def, (c, overs) => c | overs.Denied);
            // assign channel permission overwrites for member's roles (explicit allow)
            perms |= mbRoleOverrides.Aggregate(def, (c, overs) => c | overs.Allowed);

            // channel overrides for just this member
            var mbOverrides = chan.PermissionOverwrites.FirstOrDefault(xo => xo.Id == mbr.Id);
            if (mbOverrides == null) return perms;

            // assign channel permission overwrites for just this member
            perms &= ~mbOverrides.Denied;
            perms |= mbOverrides.Allowed;

            return perms;
        }

        public static async Task GuaranteeMuteRoleDeniedEverywhere(DiscordGuild guild, DiscordRole role)
        {
            var textChannels = guild.Channels.Where(e => e.Type == ChannelType.Text).ToArray();
            foreach (var channel in textChannels)
            {
                var roleOverwrite =
                    channel.PermissionOverwrites.FirstOrDefault(e => e.Type == OverwriteType.Role && e.Id == role.Id);
                if (roleOverwrite != null)
                {
                    if (!roleOverwrite.Denied.HasPermission(Permissions.SendMessages) ||
                        roleOverwrite.Allowed.HasPermission(Permissions.SendMessages))
                    {
                        await channel.AddOverwriteAsync(role, roleOverwrite.Allowed & ~Permissions.SendMessages,
                            roleOverwrite.Denied | Permissions.SendMessages,
                            $"ModCore automatically replacing denying SendMessages permission for {role.Name} in {channel.Name}");
                    }
                }
                else
                {
                    await channel.AddOverwriteAsync(role, Permissions.None, Permissions.SendMessages,
                        $"ModCore automatically denying SendMessages permission for {role.Name} in {channel.Name}");
                }
            }
        }
    }
}