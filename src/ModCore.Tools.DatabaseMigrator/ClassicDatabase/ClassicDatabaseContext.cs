using Microsoft.EntityFrameworkCore;
using ModCore.Tools.DatabaseMigrator.ClassicDatabase.DatabaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Tools.DatabaseMigrator.ClassicDatabase
{
    /// <summary>
    /// Database context for the OLD ModCore database.
    /// </summary>
    internal class ClassicDatabaseContext : DbContext
    {
        public virtual DbSet<DatabaseGuildConfig> GuildConfig { get; set; }
        public virtual DbSet<DatabaseRolestateOverride> RolestateOverrides { get; set; }
        public virtual DbSet<DatabaseRolestateRoles> RolestateRoles { get; set; }
        public virtual DbSet<DatabaseRolestateNick> RolestateNicks { get; set; }
        public virtual DbSet<DatabaseTimer> Timers { get; set; }
        public virtual DbSet<DatabaseStarData> StarDatas { get; set; }
        public virtual DbSet<DatabaseTag> Tags { get; set; }
        public virtual DbSet<DatabaseCommandId> CommandIds { get; set; }
        public virtual DbSet<DatabaseLevel> Levels { get; set; }

        private string _connectionString;

        public ClassicDatabaseContext(string connectionString)
        {
            this._connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            optionsBuilder.EnableSensitiveDataLogging();

            optionsBuilder.UseNpgsql(_connectionString);
        }
    }
}
