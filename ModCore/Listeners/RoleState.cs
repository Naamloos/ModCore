using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using ModCore.Database.JsonEntities;
using ModCore.Extensions.Attributes;
using ModCore.Extensions.Enums;
using ModCore.Utils.Extensions;

namespace ModCore.Listeners
{
    public static class RoleState
    {
        [AsyncListener(EventType.GuildMemberRemoved)]
        public static async Task OnMemberLeave(GuildMemberRemoveEventArgs eventargs, DatabaseContextBuilder database, DiscordClient client)
        {
            await using var db = database.CreateContext();
            var config = eventargs.Guild.GetGuildSettings(db);
            if (config == null || !config.RoleState.Enable)
                return;
            var rolestateconfig = config.RoleState;

            if (eventargs.Member.Roles.Any()) // no roles or cache miss, but at this point little can be done about it
            {
                var ignoredroles = rolestateconfig.IgnoredRoleIds;
                var roles = eventargs.Member.Roles.Select(xr => xr.Id).Except(ignoredroles).Select(xul => (long)xul);

                var state = db.RolestateRoles.SingleOrDefault(xs => xs.GuildId == (long)eventargs.Guild.Id && xs.MemberId == (long)eventargs.Member.Id);
                if (state == null) // no rolestate, create it
                {
                    state = new DatabaseRolestateRoles
                    {
                        GuildId = (long)eventargs.Guild.Id,
                        MemberId = (long)eventargs.Member.Id,
                        RoleIds = roles.ToArray()
                    };
                    await db.RolestateRoles.AddAsync(state);
                }
                else // rolestate exists, update it
                {
                    state.RoleIds = roles.ToArray();
                    db.RolestateRoles.Update(state);
                }
            }

            var nickstate = db.RolestateNicks.SingleOrDefault(xs => xs.GuildId == (long)eventargs.Guild.Id && xs.MemberId == (long)eventargs.Member.Id);
            client.Logger.Log(LogLevel.Debug, "ModCore", $"Do nickname shites: {eventargs.Member.Nickname}", System.DateTime.Now);
            if (nickstate == null) // no nickstate, create it
            {
                client.Logger.Log(LogLevel.Debug, "ModCore", "Create nickname shites", System.DateTime.Now);
                nickstate = new DatabaseRolestateNick
                {
                    GuildId = (long)eventargs.Guild.Id,
                    MemberId = (long)eventargs.Member.Id,
                    Nickname = eventargs.Member.Nickname
                };
                await db.RolestateNicks.AddAsync(nickstate);
            }
            else // nickstate exists, update it
            {
                client.Logger.Log(LogLevel.Debug, "ModCore", "Update nickname shites", System.DateTime.Now);
                nickstate.Nickname = eventargs.Member.Nickname;
                db.RolestateNicks.Update(nickstate);
            }

            // at this point, channel overrides do not exist
            await db.SaveChangesAsync();
        }

        [AsyncListener(EventType.GuildMemberAdded)]
        public static async Task OnMemberJoin(GuildMemberAddEventArgs eventargs, DatabaseContextBuilder database, DiscordClient client)
        {
            var guild = eventargs.Guild;
            GuildSettings config = null;
            GuildRoleStateConfig rolestateconfig = null;
            DatabaseRolestateRoles roleids = null;
            DatabaseRolestateOverride[] channelperms = null;
            DatabaseRolestateNick nickname = null;

            await using (var db = database.CreateContext())
            {
                config = eventargs.Guild.GetGuildSettings(db);
                if (config == null)
                    return;
                rolestateconfig = config.RoleState;

                roleids = db.RolestateRoles.SingleOrDefault(xs => xs.GuildId == (long)eventargs.Guild.Id && xs.MemberId == (long)eventargs.Member.Id);
                channelperms = db.RolestateOverrides.Where(xs => xs.GuildId == (long)eventargs.Guild.Id && xs.MemberId == (long)eventargs.Member.Id).ToArray();
                nickname = db.RolestateNicks.SingleOrDefault(xs => xs.GuildId == (long)eventargs.Guild.Id && xs.MemberId == (long)eventargs.Member.Id);
            }

            if (config.RoleState.Enable && roleids?.RoleIds != null)
            {
                var oroles = roleids.RoleIds
                    .Select(xid => (ulong)xid)
                    .Except(rolestateconfig.IgnoredRoleIds)
                    .Select(xid => guild.GetRole(xid))
                    .Where(xr => xr != null).ToList();

                var highestself = eventargs.Guild.CurrentMember.Roles.Select(x => x.Position).Max();
                    oroles.RemoveAll(x => x.Position > highestself);

                    if (oroles.Any())
                        await eventargs.Member.ReplaceRolesAsync(oroles, "Restoring Role State.");
            }
            else if(config.AutoRole.Enable)
            {
                var autoroleconfig = config.AutoRole;

                var roles = eventargs.Guild.Roles.Where(x => autoroleconfig.RoleIds.Any(y => y == x.Key));
                if(roles.Any())
                    await eventargs.Member.ReplaceRolesAsync(roles.Select(x => x.Value), "AutoRole");
            }

            if (channelperms.Any())
            {
                foreach (var channelpermission in channelperms)
                {
                    var channel = guild.GetChannel((ulong)channelpermission.ChannelId);
                    if (channel == null)
                        continue;

                    await channel.AddOverwriteAsync(eventargs.Member, (Permissions)channelpermission.PermsAllow, (Permissions)channelpermission.PermsDeny, "Restoring Role State");
                }
            }

            if (config.RoleState.Nickname && nickname != null)
            {
                client.Logger.Log(LogLevel.Debug, "ModCore", $"Set new old nick: {nickname.Nickname}", System.DateTime.Now);
                var member = await eventargs.Guild.GetMemberAsync(eventargs.Member.Id);
                await member.ModifyAsync(x => x.Nickname = nickname.Nickname);
            }
        }

        [AsyncListener(EventType.GuildMemberUpdated)]
        public static async Task OnMemberUpdate(GuildMemberUpdateEventArgs eventargs, DatabaseContextBuilder database)
        {
            await using var db = database.CreateContext();
            var config = eventargs.Guild.GetGuildSettings(db);
            if (config == null)
                return;
            var rolestate = config.RoleState;

            var guild = eventargs.Guild;
            var roleids = db.RolestateRoles.SingleOrDefault(xs => xs.GuildId == (long)eventargs.Guild.Id && xs.MemberId == (long)eventargs.Member.Id);

            if (config.RoleState.Enable)
            {
                if (roleids == null)
                {
                    roleids = new DatabaseRolestateRoles
                    {
                        GuildId = (long)eventargs.Guild.Id,
                        MemberId = (long)eventargs.Member.Id
                    };
                }

                roleids.RoleIds = eventargs.RolesAfter
                    .Select(xr => xr.Id)
                    .Except(rolestate.IgnoredRoleIds)
                    .Select(xid => (long)xid)
                    .ToArray();
                db.RolestateRoles.Update(roleids);
            }

            if (config.RoleState.Nickname)
            {
                var nickname = db.RolestateNicks.SingleOrDefault(xs => xs.GuildId == (long)eventargs.Guild.Id && xs.MemberId == (long)eventargs.Member.Id);
                if (nickname == null)
                {
                    nickname = new DatabaseRolestateNick
                    {
                        GuildId = (long)eventargs.Guild.Id,
                        MemberId = (long)eventargs.Member.Id,
                    };
                }
                nickname.Nickname = eventargs.NicknameAfter;
                db.RolestateNicks.Update(nickname);
            }

            await db.SaveChangesAsync();
        }

        [AsyncListener(EventType.ChannelDeleted)]
        public static async Task OnChannelRemove(ChannelDeleteEventArgs eventargs, DatabaseContextBuilder database)
        {
            if (eventargs.Guild == null)
                return;

            await using var db = database.CreateContext();
            var config = eventargs.Guild.GetGuildSettings(db);
            if (config == null || !config.RoleState.Enable)
                return;
            var rolestate = config.RoleState;

            var channelpermissions = db.RolestateOverrides.Where(xs => xs.GuildId == (long)eventargs.Guild.Id && xs.ChannelId == (long)eventargs.Channel.Id);

            if (channelpermissions.Any())
            {
                db.RolestateOverrides.RemoveRange(channelpermissions);
                await db.SaveChangesAsync();
            }
        }

        // not necessary right now
        //[AsyncListener(EventTypes.ChannelCreated)]
        //public static async Task OnChannelCreate(ModCoreShard shard, ChannelCreateEventArgs ea)
        //{
        //    var db = shard.Database.CreateContext();
        //    var cfg = ea.Guild.GetGuildSettings(db);
        //    if (cfg == null || !cfg.RoleState.Enable)
        //        return;
        //    var rs = cfg.RoleState;

        //}

        [AsyncListener(EventType.ChannelUpdated)]
        public static async Task OnChannelUpdate(ChannelUpdateEventArgs eventargs, DatabaseContextBuilder database)
        {
            if (eventargs.Guild == null)
                return;

            await using var db = database.CreateContext();
            var config = eventargs.Guild.GetGuildSettings(db);
            if (config == null || !config.RoleState.Enable)
                return;
            var rolestateconfig = config.RoleState;

            if (rolestateconfig.IgnoredChannelIds.Contains(eventargs.ChannelAfter.Id))
                return;

            var overwrites = eventargs.ChannelAfter.PermissionOverwrites.Where(xo => xo.Type.ToString().ToLower() == "member").ToDictionary(xo => (long)xo.Id, xo => xo);
            var overwriteids = overwrites.Select(xo => xo.Key).ToArray();

            var channelpermissions = db.RolestateOverrides.Where(xs => xs.GuildId == (long)eventargs.Guild.Id && xs.ChannelId == (long)eventargs.ChannelAfter.Id)
                .ToDictionary(xs => xs.MemberId, xs => xs);
            var channelpermissionids = channelpermissions.Select(xo => xo.Key).ToArray();

            var removepermissions = channelpermissionids.Except(overwriteids);
            var addpermissions = overwriteids.Except(channelpermissionids);
            var modifypermissions = overwriteids.Intersect(channelpermissionids);

            if (removepermissions.Any())
                db.RolestateOverrides.RemoveRange(removepermissions.Select(xid => channelpermissions[xid]));

            if (addpermissions.Any())
                await db.RolestateOverrides.AddRangeAsync(addpermissions.Select(xid => new DatabaseRolestateOverride
                {
                    ChannelId = (long)eventargs.ChannelAfter.Id,
                    GuildId = (long)eventargs.Guild.Id,
                    MemberId = xid,
                    PermsAllow = (long)overwrites[xid].Allowed,
                    PermsDeny = (long)overwrites[xid].Denied
                }));

            if (modifypermissions.Any())
                foreach (var xid in modifypermissions)
                {
                    channelpermissions[xid].PermsAllow = (long)overwrites[xid].Allowed;
                    channelpermissions[xid].PermsDeny = (long)overwrites[xid].Denied;

                    db.RolestateOverrides.Update(channelpermissions[xid]);
                }

            if (removepermissions.Any() || addpermissions.Any() || modifypermissions.Any())
                await db.SaveChangesAsync();
        }

        [AsyncListener(EventType.GuildAvailable)]
        public static async Task OnGuildAvailable(GuildCreateEventArgs eventargs, DatabaseContextBuilder database)
        {
            // Ensure full member cache?

            ////if(ea.Guild.MemberCount < 10000)
            ////    await ea.Guild.GetAllMembersAsync();

            // haha lets not

            await using var db = database.CreateContext();
            var config = eventargs.Guild.GetGuildSettings(db);
            if (config == null || !config.RoleState.Enable)
                return;
            var rolestateconfig = config.RoleState;

            var channelpermissions = db.RolestateOverrides.Where(xs => xs.GuildId == (long)eventargs.Guild.Id);
            var permissions = channelpermissions.AsEnumerable().GroupBy(xs => xs.ChannelId).ToDictionary(xg => xg.Key, xg => xg.ToDictionary(xs => xs.MemberId, xs => xs));

            var any = false;

            foreach (var channel in eventargs.Guild.Channels)
            {
                if (rolestateconfig.IgnoredChannelIds.Contains(channel.Key))
                    continue;

                if (!permissions.ContainsKey((long)channel.Key))
                {
                    any = true;

                    var overwrites = channel.Value.PermissionOverwrites.Where(xo => xo.Type.ToString().ToLower() == "member");
                    if (!overwrites.Any())
                        continue;

                    await db.RolestateOverrides.AddRangeAsync(overwrites.Select(xo => new DatabaseRolestateOverride
                    {
                        ChannelId = (long)channel.Value.Id,
                        GuildId = (long)channel.Value.Guild.Id,
                        MemberId = (long)xo.Id,
                        PermsAllow = (long)xo.Allowed,
                        PermsDeny = (long)xo.Denied
                    }));
                }
                else
                {
                    var rolestateoverrides = permissions[(long)channel.Value.Id];
                    var overrides = channel.Value.PermissionOverwrites.Where(xo => xo.Type.ToString().ToLower() == "member").ToDictionary(xo => (long)xo.Id, xo => xo);
                    var overrideids = overrides.Keys.ToArray();

                    var deleteoverrides = rolestateoverrides.Keys.Except(overrideids);
                    var addoverrides = overrideids.Except(rolestateoverrides.Keys);
                    var modifyoverrides = overrideids.Intersect(rolestateoverrides.Keys);

                    if (any |= deleteoverrides.Any())
                        db.RolestateOverrides.RemoveRange(deleteoverrides.Select(xid => rolestateoverrides[xid]));

                    if (any |= addoverrides.Any())
                        await db.RolestateOverrides.AddRangeAsync(addoverrides.Select(xid => new DatabaseRolestateOverride
                        {
                            ChannelId = (long)channel.Key,
                            GuildId = (long)eventargs.Guild.Id,
                            MemberId = xid,
                            PermsAllow = (long)overrides[xid].Allowed,
                            PermsDeny = (long)overrides[xid].Denied
                        }));

                    if (any |= modifyoverrides.Any())
                        foreach (var overrideid in modifyoverrides)
                        {
                            rolestateoverrides[overrideid].PermsAllow = (long)overrides[overrideid].Allowed;
                            rolestateoverrides[overrideid].PermsDeny = (long)overrides[overrideid].Denied;

                            db.RolestateOverrides.Update(rolestateoverrides[overrideid]);
                        }
                }
            }

            if (any)
                await db.SaveChangesAsync();
        }

        [AsyncListener(EventType.GuildCreated)]
        public static Task OnGuildCreated(GuildCreateEventArgs eventargs, DatabaseContextBuilder database) =>
            OnGuildAvailable(eventargs, database);
    }
}
