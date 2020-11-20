using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using ModCore.Database;
using ModCore.Entities;
using ModCore.Logic;
using ModCore.Logic.Extensions;

namespace ModCore.Listeners
{
    public static class RoleState
    {
        [AsyncListener(EventTypes.GuildMemberRemoved)]
        public static async Task OnMemberLeave(ModCoreShard shard, GuildMemberRemoveEventArgs ea)
        {
            using (var db = shard.Database.CreateContext())
            {
                var cfg = ea.Guild.GetGuildSettings(db);
                if (cfg == null || !cfg.RoleState.Enable)
                    return;
                var rs = cfg.RoleState;

                if (ea.Member.Roles.Any()) // no roles or cache miss, but at this point little can be done about it
                {
                    var rsx = rs.IgnoredRoleIds;
                    var roles = ea.Member.Roles.Select(xr => xr.Id).Except(rsx).Select(xul => (long)xul);

                    var state = db.RolestateRoles.SingleOrDefault(xs => xs.GuildId == (long)ea.Guild.Id && xs.MemberId == (long)ea.Member.Id);
                    if (state == null) // no rolestate, create it
                    {
                        state = new DatabaseRolestateRoles
                        {
                            GuildId = (long)ea.Guild.Id,
                            MemberId = (long)ea.Member.Id,
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

                var nickstate = db.RolestateNicks.SingleOrDefault(xs => xs.GuildId == (long)ea.Guild.Id && xs.MemberId == (long)ea.Member.Id);
                shard.Client.Logger.Log(LogLevel.Debug, "ModCore", $"Do nickname shites: {ea.Member.Nickname}", System.DateTime.Now);
                if (nickstate == null) // no nickstate, create it
                {
                    shard.Client.Logger.Log(LogLevel.Debug, "ModCore", "Create nickname shites", System.DateTime.Now);
                    nickstate = new DatabaseRolestateNick
                    {
                        GuildId = (long)ea.Guild.Id,
                        MemberId = (long)ea.Member.Id,
                        Nickname = ea.Member.Nickname
                    };
                    await db.RolestateNicks.AddAsync(nickstate);
                }
                else // nickstate exists, update it
                {
                    shard.Client.Logger.Log(LogLevel.Debug, "ModCore", "Update nickname shites", System.DateTime.Now);
                    nickstate.Nickname = ea.Member.Nickname;
                    db.RolestateNicks.Update(nickstate);
                }

                // at this point, channel overrides do not exist
                await db.SaveChangesAsync();
            }
        }

        [AsyncListener(EventTypes.GuildMemberAdded)]
        public static async Task OnMemberJoin(ModCoreShard shard, GuildMemberAddEventArgs ea)
        {
            var gld = ea.Guild;
            GuildSettings cfg = null;
            GuildRoleStateConfig rs = null;
            DatabaseRolestateRoles roleids = null;
            DatabaseRolestateOverride[] chperms = null;
            DatabaseRolestateNick nick = null;

            using (var db = shard.Database.CreateContext())
            {
                cfg = ea.Guild.GetGuildSettings(db);
                if (cfg == null || !cfg.RoleState.Enable)
                    return;
                rs = cfg.RoleState;

                roleids = db.RolestateRoles.SingleOrDefault(xs => xs.GuildId == (long)ea.Guild.Id && xs.MemberId == (long)ea.Member.Id);
                chperms = db.RolestateOverrides.Where(xs => xs.GuildId == (long)ea.Guild.Id && xs.MemberId == (long)ea.Member.Id).ToArray();
                nick = db.RolestateNicks.SingleOrDefault(xs => xs.GuildId == (long)ea.Guild.Id && xs.MemberId == (long)ea.Member.Id);
            }

            if (roleids?.RoleIds != null)
            {
                var oroles = roleids.RoleIds
                    .Select(xid => (ulong)xid)
                    .Except(rs.IgnoredRoleIds)
                    .Select(xid => gld.GetRole(xid))
                    .Where(xr => xr != null).ToList();

                    var roles = oroles;

                    var highestself = ea.Guild.CurrentMember.Roles.Select(x => x.Position).Max();
                    roles.RemoveAll(x => x.Position > highestself);

                    if (roles.Any())
                        await ea.Member.ReplaceRolesAsync(roles, "Restoring Role State.");
            }
            else
            {
                var ar = cfg.AutoRole;

                if (ar.Enable && ea.Guild.Roles.Count(x => x.Value.Id == (ulong)ar.RoleId) > 0)
                {
                    var role = ea.Guild.Roles.First(x => x.Value.Id == (ulong)ar.RoleId);
                    await ea.Member.GrantRoleAsync(role.Value, "AutoRole");
                }
            }

            if (chperms.Any())
            {
                foreach (var chperm in chperms)
                {
                    var chn = gld.GetChannel((ulong)chperm.ChannelId);
                    if (chn == null)
                        continue;

                    await chn.AddOverwriteAsync(ea.Member, (Permissions)chperm.PermsAllow, (Permissions)chperm.PermsDeny, "Restoring Role State");
                }
            }

            if(nick != null)
            {
                shard.Client.Logger.Log(LogLevel.Debug, "ModCore", $"Set new old nick: {nick.Nickname}", System.DateTime.Now);
                var m = await ea.Guild.GetMemberAsync(ea.Member.Id);
                await m.ModifyAsync(x => x.Nickname = nick.Nickname);
            }
        }

        [AsyncListener(EventTypes.GuildMemberUpdated)]
        public static async Task OnMemberUpdate(ModCoreShard shard, GuildMemberUpdateEventArgs ea)
        {
            using (var db = shard.Database.CreateContext())
            {
                var cfg = ea.Guild.GetGuildSettings(db);
                if (cfg == null || !cfg.RoleState.Enable)
                    return;
                var rs = cfg.RoleState;

                var gld = ea.Guild;
                var roleids = db.RolestateRoles.SingleOrDefault(xs => xs.GuildId == (long)ea.Guild.Id && xs.MemberId == (long)ea.Member.Id);

                if (roleids == null)
                {
                    roleids = new DatabaseRolestateRoles
                    {
                        GuildId = (long)ea.Guild.Id,
                        MemberId = (long)ea.Member.Id
                    };
                }

                roleids.RoleIds = ea.RolesAfter
                    .Select(xr => xr.Id)
                    .Except(rs.IgnoredRoleIds)
                    .Select(xid => (long)xid)
                    .ToArray();
                db.RolestateRoles.Update(roleids);

                var nick = db.RolestateNicks.SingleOrDefault(xs => xs.GuildId == (long)ea.Guild.Id && xs.MemberId == (long)ea.Member.Id);
                if(nick == null)
                {
                    nick = new DatabaseRolestateNick
                    {
                        GuildId = (long)ea.Guild.Id,
                        MemberId = (long)ea.Member.Id,
                    };
                }
                nick.Nickname = ea.NicknameAfter;
                db.RolestateNicks.Update(nick);

                await db.SaveChangesAsync();
            }
        }

        [AsyncListener(EventTypes.ChannelDeleted)]
        public static async Task OnChannelRemove(ModCoreShard shard, ChannelDeleteEventArgs ea)
        {
            if (ea.Guild == null)
                return;

            using (var db = shard.Database.CreateContext())
            {
                var cfg = ea.Guild.GetGuildSettings(db);
                if (cfg == null || !cfg.RoleState.Enable)
                    return;
                var rs = cfg.RoleState;

                var chperms = db.RolestateOverrides.Where(xs => xs.GuildId == (long)ea.Guild.Id && xs.ChannelId == (long)ea.Channel.Id);

                if (chperms.Any())
                {
                    db.RolestateOverrides.RemoveRange(chperms);
                    await db.SaveChangesAsync();
                }
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

        [AsyncListener(EventTypes.ChannelUpdated)]
        public static async Task OnChannelUpdate(ModCoreShard shard, ChannelUpdateEventArgs ea)
        {
            if (ea.Guild == null)
                return;

            using (var db = shard.Database.CreateContext())
            {
                var cfg = ea.Guild.GetGuildSettings(db);
                if (cfg == null || !cfg.RoleState.Enable)
                    return;
                var rs = cfg.RoleState;

                if (rs.IgnoredChannelIds.Contains(ea.ChannelAfter.Id))
                    return;

                var os = ea.ChannelAfter.PermissionOverwrites.Where(xo => xo.Type.ToString().ToLower() == "member").ToDictionary(xo => (long)xo.Id, xo => xo);
                var osids = os.Select(xo => xo.Key).ToArray();

                var chperms = db.RolestateOverrides.Where(xs => xs.GuildId == (long)ea.Guild.Id && xs.ChannelId == (long)ea.ChannelAfter.Id)
                    .ToDictionary(xs => xs.MemberId, xs => xs);
                var permids = chperms.Select(xo => xo.Key).ToArray();

                var del = permids.Except(osids);
                var add = osids.Except(permids);
                var mod = osids.Intersect(permids);

                if (del.Any())
                    db.RolestateOverrides.RemoveRange(del.Select(xid => chperms[xid]));

                if (add.Any())
                    await db.RolestateOverrides.AddRangeAsync(add.Select(xid => new DatabaseRolestateOverride
                    {
                        ChannelId = (long)ea.ChannelAfter.Id,
                        GuildId = (long)ea.Guild.Id,
                        MemberId = xid,
                        PermsAllow = (long)os[xid].Allowed,
                        PermsDeny = (long)os[xid].Denied
                    }));

                if (mod.Any())
                    foreach (var xid in mod)
                    {
                        chperms[xid].PermsAllow = (long)os[xid].Allowed;
                        chperms[xid].PermsDeny = (long)os[xid].Denied;

                        db.RolestateOverrides.Update(chperms[xid]);
                    }

                if (del.Any() || add.Any() || mod.Any())
                    await db.SaveChangesAsync();
            }
        }

        [AsyncListener(EventTypes.GuildAvailable)]
        public static async Task OnGuildAvailable(ModCoreShard shard, GuildCreateEventArgs ea)
        {
            // Ensure full member cache?

            ////if(ea.Guild.MemberCount < 10000)
            ////    await ea.Guild.GetAllMembersAsync();

            // haha lets not

            using (var db = shard.Database.CreateContext())
            {
                var cfg = ea.Guild.GetGuildSettings(db);
                if (cfg == null || !cfg.RoleState.Enable)
                    return;
                var rs = cfg.RoleState;

                var chperms = db.RolestateOverrides.Where(xs => xs.GuildId == (long)ea.Guild.Id);
                var prms = chperms.AsEnumerable().GroupBy(xs => xs.ChannelId).ToDictionary(xg => xg.Key, xg => xg.ToDictionary(xs => xs.MemberId, xs => xs));

                var any = false;

                foreach (var chn in ea.Guild.Channels)
                {
                    if (rs.IgnoredChannelIds.Contains(chn.Key))
                        continue;

                    if (!prms.ContainsKey((long)chn.Key))
                    {
                        any = true;

                        var os = chn.Value.PermissionOverwrites.Where(xo => xo.Type.ToString().ToLower() == "member");
                        if (!os.Any())
                            continue;

                        await db.RolestateOverrides.AddRangeAsync(os.Select(xo => new DatabaseRolestateOverride
                        {
                            ChannelId = (long)chn.Value.Id,
                            GuildId = (long)chn.Value.Guild.Id,
                            MemberId = (long)xo.Id,
                            PermsAllow = (long)xo.Allowed,
                            PermsDeny = (long)xo.Denied
                        }));
                    }
                    else
                    {
                        var cps = prms[(long)chn.Value.Id];
                        var os = chn.Value.PermissionOverwrites.Where(xo => xo.Type.ToString().ToLower() == "member").ToDictionary(xo => (long)xo.Id, xo => xo);
                        var osids = os.Keys.ToArray();

                        var del = cps.Keys.Except(osids);
                        var add = osids.Except(cps.Keys);
                        var mod = osids.Intersect(cps.Keys);

                        if (any |= del.Any())
                            db.RolestateOverrides.RemoveRange(del.Select(xid => cps[xid]));

                        if (any |= add.Any())
                            await db.RolestateOverrides.AddRangeAsync(add.Select(xid => new DatabaseRolestateOverride
                            {
                                ChannelId = (long)chn.Key,
                                GuildId = (long)ea.Guild.Id,
                                MemberId = xid,
                                PermsAllow = (long)os[xid].Allowed,
                                PermsDeny = (long)os[xid].Denied
                            }));

                        if (any |= mod.Any())
                            foreach (var xid in mod)
                            {
                                cps[xid].PermsAllow = (long)os[xid].Allowed;
                                cps[xid].PermsDeny = (long)os[xid].Denied;

                                db.RolestateOverrides.Update(cps[xid]);
                            }
                    }
                }

                if (any)
                    await db.SaveChangesAsync();
            }
        }

        [AsyncListener(EventTypes.GuildCreated)]
        public static Task OnGuildCreated(ModCoreShard shard, GuildCreateEventArgs ea) =>
            OnGuildAvailable(shard, ea);
    }
}
