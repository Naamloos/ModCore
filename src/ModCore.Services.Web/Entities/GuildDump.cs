using Microsoft.EntityFrameworkCore;
using ModCore.Common.Database;
using ModCore.Common.Database.Entities;
using System.Text.Json.Serialization;

namespace ModCore.Services.Web.Entities
{
    public class GuildDump
    {
        [JsonPropertyName("dumped_at")]
        public DateTimeOffset DumpedAt { get; set; }

        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }

        [JsonPropertyName("config")]
        public DatabaseGuild Config { get; set; }

        [JsonPropertyName("starboard_items")]
        public DatabaseStarboardItem[] StarboardItems { get; set; }

        [JsonPropertyName("level_data")]
        public DatabaseLevelData[] LevelData { get; set; }

        [JsonPropertyName("tags")]
        public DatabaseTag[] Tags { get; set; }

        [JsonPropertyName("role_states")]
        public DatabaseRoleState[] RoleStates { get; set; }

        [JsonPropertyName("override_states")]
        public DatabaseOverrideState[] OverrideStates { get; set; }

        [JsonPropertyName("nickname_states")]
        public DatabaseNicknameState[] NicknameStates { get; set; }

        [JsonPropertyName("infractions")]
        public DatabaseInfraction[] Infractions { get; set; }

        [JsonPropertyName("ban_appeals")]
        public DatabaseBanAppeal[] BanAppeals { get; set; }

        [JsonPropertyName("tickets")]
        public DatabaseTicket[] Tickets { get; set; }

        private GuildDump() { }

        public static GuildDump Create(ulong id, DatabaseContext database)
        {
            var dump = new GuildDump
            {
                DumpedAt = DateTimeOffset.UtcNow,
                GuildId = id
            };

            var dbGuild = database.Guilds
                .Include(x => x.Starboards)
                .Include(x => x.AutoRoles)
                .Include(x => x.RoleMenus)
                    .ThenInclude(x => x.Roles)
                .Include(x => x.LoggerSettings)
                .Include(x => x.WelcomeSettings)
                .Include(x => x.LevelSettings)
                .FirstOrDefault(x => x.GuildId == id);

            // Dumping data from DB
            if (dbGuild != null) 
            {
                dump.Config = dbGuild;
                dump.StarboardItems = database.StarboardItems.Where(x => x.Starboard.GuildId == id).ToArray();
                dump.LevelData = database.LevelData.Where(x => x.GuildId == id).ToArray();
                dump.Tags = database.Tags.Where(x => x.GuildId == id).ToArray();
                dump.RoleStates = database.RoleStates.Where(x => x.GuildId == id).ToArray();
                dump.OverrideStates = database.OverrideStates.Where(x => x.GuildId == id).ToArray();
                dump.NicknameStates = database.NicknameStates.Where(x => x.GuildId == id).ToArray();
                dump.Infractions = database.Infractions.Where(x => x.GuildId == id).ToArray();
                dump.BanAppeals = database.BanAppeals.Where(x => x.GuildId == id).ToArray();
                dump.Tickets = database.Tickets.Where(x => x.GuildId == id).ToArray();
            }

            // Return dump
            return dump;
        }
    }
}
