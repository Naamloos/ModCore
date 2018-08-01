using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModCore.Entities;
using ModCore.Logic;
using ModCore.Logic.EntityFramework;

namespace ModCore.Database
{
    public class DatabaseContext : DbContext, IEfCustomContext
    {
        public virtual DbSet<DatabaseInfo> Info { get; set; }
        public virtual DbSet<DatabaseGuildConfig> GuildConfig { get; set; }
        public virtual DbSet<DatabaseModNote> Modnotes { get; set; }
        public virtual DbSet<DatabaseRolestateOverride> RolestateOverrides { get; set; }
        public virtual DbSet<DatabaseRolestateRoles> RolestateRoles { get; set; }
        public virtual DbSet<DatabaseWarning> Warnings { get; set; }
        public virtual DbSet<DatabaseTimer> Timers { get; set; }
        public virtual DbSet<DatabaseStarData> StarDatas { get; set; }
        public virtual DbSet<DatabaseBan> Bans { get; set; }
        public virtual DbSet<DatabaseTag> Tags { get; set; }
        public virtual DbSet<DatabaseCommandId> CommandIds { get; set; }

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
            model.BuildCustomAttributes(this);
            
            // TODO: at the end of moving all of these to annotations
            // > add a new EF migration, don't apply it, generate a script for it, theoretically there should be nothing
            //   changed, otherwise we messed up
            // > test the bot (obviously)
            
            // TODO https://stackoverflow.com/questions/2878272/when-should-i-use-primary-key-or-index
            // the use of indices here makes sense, however, should we add [Key] to the .Id properties?
            
            // TODO change index_id in DatabaseCommandId to somethign less likely to conflict, like
            // mcore_cmd_state_index_id
            
            // https://www.learnentityframeworkcore.com/configuration/data-annotation-attributes
            // this site is GOLD for finding out the data annotation equivalents to fluent API methods, and finding out 
            // whether or not they even exist
            
            // TODO is cmd state index_id even necessary? isn't it duplicate, since we already have a key?
            // is the alternate key needed? i guess making id an alternate key makes it unique, which is
            // important.
        }
    }
}
