using Microsoft.EntityFrameworkCore;
using ModCore.Common.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Database.Helpers
{
    public class InfractionHelper
    {
        private ulong userId { get; set; }
        private ulong guildId { get; set; }
        private DatabaseContext database { get; set; }
        private IQueryable<DatabaseInfraction> infractions { get; set; }

        public InfractionHelper(DatabaseContext database, ulong user_id, ulong guild_id)
        {
            this.database = database;
            this.userId = user_id;
            this.guildId = guild_id;
            this.infractions = database.Infractions.Where(x => x.UserId == user_id && x.GuildId == guild_id);
        }

        public async Task<DatabaseInfraction> CreateInfractionAsync(InfractionType type, 
            ulong responsible_moderator, string reason = null, bool notified = false)
        {
            // ensure data for this guild exists
            await database.TouchGuild(guildId);

            var infraction = await database.Infractions.AddAsync(new DatabaseInfraction()
            {
                GuildId = guildId,
                UserId = userId,
                Type = type,
                Reason = reason,
                ResponsibleModerator = responsible_moderator,
                UserNotified = notified,
                Id = 0
            });

            database.SaveChanges();

            return infraction.Entity;
        }

        public async Task<IEnumerable<DatabaseInfraction>> GetInfractionsAsync()
        {
            return await infractions.ToListAsync();
        }
    }
}
