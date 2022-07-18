using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModCore.Database.DatabaseEntities;
using ModCore.Entities;
using ModCore.Utils;
using ModCore.Utils.EntityFramework;

namespace ModCore.Database
{
    public class DatabaseContext : DbContext, IEfCustomContext
    {
        public virtual DbSet<DatabaseInfo> Info { get; set; }
        public virtual DbSet<DatabaseGuildConfig> GuildConfig { get; set; }
        public virtual DbSet<DatabaseRolestateOverride> RolestateOverrides { get; set; }
        public virtual DbSet<DatabaseRolestateRoles> RolestateRoles { get; set; }
        public virtual DbSet<DatabaseRolestateNick> RolestateNicks { get; set; }
        public virtual DbSet<DatabaseTimer> Timers { get; set; }
        public virtual DbSet<DatabaseStarData> StarDatas { get; set; }
        public virtual DbSet<DatabaseTag> Tags { get; set; }
        public virtual DbSet<DatabaseCommandId> CommandIds { get; set; }
        public virtual DbSet<DatabaseUserData> UserDatas { get; set; }
        public virtual DbSet<DatabaseLevel> Levels { get; set; }

        public DatabaseProvider Provider { get; }
        private string ConnectionString { get; }
        
        public DatabaseContext(DatabaseProvider provider, string cstring)
        {
            this.Provider = provider;
            this.ConnectionString = cstring;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;

            #if DEBUG
            optionsBuilder.EnableSensitiveDataLogging(true);
            #endif
            
            switch (this.Provider)
            {
                case DatabaseProvider.PostgreSql:
                    optionsBuilder.UseNpgsql(this.ConnectionString);
                    break;
                case DatabaseProvider.Sqlite:
                    optionsBuilder.UseSqlite(this.ConnectionString);
                    break;
                case DatabaseProvider.InMemory:
                    optionsBuilder.UseInMemoryDatabase("modcore");
                    break;
            }
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            // https://www.learnentityframeworkcore.com/configuration/data-annotation-attributes
            // this site is GOLD for finding out the data annotation equivalents to fluent API methods, and finding out 
            // whether or not they even exist
            
            model.BuildCustomAttributes(this);
        }
    }
}
