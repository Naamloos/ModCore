using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using ModCore.Database;
using ModCore.Logic;

namespace ModCore.Listeners
{
    public static class RoleState
    {
        [AsyncListener(EventTypes.GuildMemberRemoved)]
        public static async Task OnMemberLeave(ModCoreShard shard, GuildMemberRemoveEventArgs ea)
        {
            var db = shard.Database.CreateContext();
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

            // at this point, channel overrides do not exist
            await db.SaveChangesAsync();
        }

        [AsyncListener(EventTypes.GuildMemberAdded)]
        public static async Task OnMemberJoin(ModCoreShard shard, GuildMemberAddEventArgs ea)
        {
            var db = shard.Database.CreateContext();
            var cfg = ea.Guild.GetGuildSettings(db);
            if (cfg == null || !cfg.RoleState.Enable)
                return;
            var rs = cfg.RoleState;

        }

        [AsyncListener(EventTypes.GuildMemberUpdated)]
        public static async Task OnMemberUpdate(ModCoreShard shard, GuildMemberUpdateEventArgs ea)
        {
            var db = shard.Database.CreateContext();
            var cfg = ea.Guild.GetGuildSettings(db);
            if (cfg == null || !cfg.RoleState.Enable)
                return;
            var rs = cfg.RoleState;

        }

        [AsyncListener(EventTypes.ChannelDeleted)]
        public static async Task OnChannelRemove(ModCoreShard shard, ChannelDeleteEventArgs ea)
        {
            var db = shard.Database.CreateContext();
            var cfg = ea.Guild.GetGuildSettings(db);
            if (cfg == null || !cfg.RoleState.Enable)
                return;
            var rs = cfg.RoleState;

        }

        [AsyncListener(EventTypes.ChannelCreated)]
        public static async Task OnChannelCreate(ModCoreShard shard, ChannelCreateEventArgs ea)
        {
            var db = shard.Database.CreateContext();
            var cfg = ea.Guild.GetGuildSettings(db);
            if (cfg == null || !cfg.RoleState.Enable)
                return;
            var rs = cfg.RoleState;

        }

        [AsyncListener(EventTypes.ChannelUpdated)]
        public static async Task OnChannelUpdate(ModCoreShard shard, ChannelUpdateEventArgs ea)
        {
            var db = shard.Database.CreateContext();
            var cfg = ea.Guild.GetGuildSettings(db);
            if (cfg == null || !cfg.RoleState.Enable)
                return;
            var rs = cfg.RoleState;

        }
    }
}
