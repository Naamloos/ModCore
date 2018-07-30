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
    public class DatabaseContext : DbContext
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

        private DatabaseProvider Provider { get; }
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
            model.BuildCustomAttributes();
            
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
            
            model.Entity<DatabaseInfo>(e => // done
            {
                e.ToTable("mcore_database_info");

                e.HasIndex(t => t.MetaKey)
                    .HasName("mcore_database_info_meta_key_key")
                    .IsUnique();

                e.Property(t => t.Id).HasColumnName("id");

                e.Property(t => t.MetaKey)
                    .IsRequired()
                    .HasColumnName("meta_key");

                e.Property(t => t.MetaValue)
                    .IsRequired()
                    .HasColumnName("meta_value");
            });

            model.Entity<DatabaseGuildConfig>(e =>
            {
                e.ToTable("mcore_guild_config");

                e.HasIndex(t => t.GuildId)
                    .HasName("mcore_guild_config_guild_id_key")
                    .IsUnique();

                e.Property(t => t.Id).HasColumnName("id");

                e.Property(t => t.GuildId).HasColumnName("guild_id");

                e.Property(t => t.Settings)
                    .IsRequired()
                    .HasColumnName("settings")
                    .HasColumnType("jsonb");
            });

            model.Entity<DatabaseModNote>(e =>
            {
                e.ToTable("mcore_modnotes");

                e.HasIndex(t => new { t.MemberId, t.GuildId })
                    .HasName("mcore_modnotes_member_id_guild_id_key")
                    .IsUnique();

                e.Property(t => t.Id).HasColumnName("id");

                e.Property(t => t.Contents).HasColumnName("contents");

                e.Property(t => t.GuildId).HasColumnName("guild_id");

                e.Property(t => t.MemberId).HasColumnName("member_id");
            });

            model.Entity<DatabaseRolestateOverride>(e =>
            {
                e.ToTable("mcore_rolestate_overrides");

                e.HasIndex(t => new { t.MemberId, t.GuildId, t.ChannelId })
                    .HasName("mcore_rolestate_overrides_member_id_guild_id_channel_id_key")
                    .IsUnique();

                e.Property(t => t.Id).HasColumnName("id");

                e.Property(t => t.ChannelId).HasColumnName("channel_id");

                e.Property(t => t.GuildId).HasColumnName("guild_id");

                e.Property(t => t.MemberId).HasColumnName("member_id");

                e.Property(t => t.PermsAllow).HasColumnName("perms_allow");

                e.Property(t => t.PermsDeny).HasColumnName("perms_deny");
            });

            model.Entity<DatabaseRolestateRoles>(e =>
            {
                e.ToTable("mcore_rolestate_roles");

                e.HasIndex(t => new { t.MemberId, t.GuildId })
                    .HasName("mcore_rolestate_roles_member_id_guild_id_key")
                    .IsUnique();

                e.Property(t => t.Id).HasColumnName("id");

                e.Property(t => t.GuildId).HasColumnName("guild_id");

                e.Property(t => t.MemberId).HasColumnName("member_id");

                e.Property(t => t.RoleIds).HasColumnName("role_ids");

                if (this.Provider != DatabaseProvider.PostgreSql)
                    e.Ignore(t => t.RoleIds);
            });

            model.Entity<DatabaseWarning>(e =>
            {
                e.ToTable("mcore_warnings");

                e.Property(t => t.Id).HasColumnName("id");

                e.Property(t => t.GuildId).HasColumnName("guild_id");

                e.Property(t => t.IssuedAt)
                    .HasColumnName("issued_at")
                    .HasColumnType("timestamptz");

                e.Property(t => t.IssuerId).HasColumnName("issuer_id");

                e.Property(t => t.MemberId).HasColumnName("member_id");

                e.Property(t => t.WarningText)
                    .IsRequired()
                    .HasColumnName("warning_text");
            });

            model.Entity<DatabaseTimer>(e =>
            {
                e.ToTable("mcore_timers");

                e.Property(t => t.Id).HasColumnName("id");

                e.Property(t => t.GuildId).HasColumnName("guild_id");

                e.Property(t => t.ChannelId).HasColumnName("channel_id");

                e.Property(t => t.UserId).HasColumnName("user_id");

                e.Property(t => t.DispatchAt)
                    .HasColumnName("dispatch_at")
                    .HasColumnType("timestamptz");

                e.Property(t => t.ActionType).HasColumnName("action_type");

                e.Property(t => t.ActionData)
                    .IsRequired()
                    .HasColumnName("action_data")
                    .HasColumnType("jsonb");
            });

            model.Entity<DatabaseStarData>(e =>
            {
                e.ToTable("mcore_stars");

                e.HasIndex(t => new { t.MessageId, t.ChannelId, t.StargazerId })
                    .HasName("mcore_stars_member_id_guild_id_key")
                    .IsUnique();

                e.Property(t => t.Id).HasColumnName("id");

                e.Property(t => t.StargazerId).HasColumnName("stargazer_id");

                e.Property(t => t.StarboardMessageId).HasColumnName("starboard_entry_id");

                e.Property(t => t.MessageId).HasColumnName("message_id");

                e.Property(t => t.AuthorId).HasColumnName("author_id");

                e.Property(t => t.GuildId).HasColumnName("guild_id");

                e.Property(t => t.ChannelId).HasColumnName("channel_id");
            });

            model.Entity<DatabaseBan>(e =>
            {
                e.ToTable("mcore_bans");

                e.Property(t => t.Id).HasColumnName("id");

                e.Property(t => t.GuildId).HasColumnName("guild_id");

                e.Property(t => t.UserId).HasColumnName("user_id");

                e.Property(t => t.IssuedAt)
                .HasColumnName("issued_at")
                .HasColumnType("timestamptz");

                e.Property(t => t.BanReason).HasColumnName("ban_reason");
            });

            model.Entity<DatabaseTag>(e =>
            {
                e.HasIndex(t => new { t.ChannelId, t.Name })
                    .HasName("mcore_tags_channel_id_tag_name_key")
                    .IsUnique();

                e.ToTable("mcore_tags");

                e.Property(t => t.Id).HasColumnName("id");

                e.Property(t => t.ChannelId).HasColumnName("channel_id");

                e.Property(t => t.OwnerId).HasColumnName("owner_id");

                e.Property(t => t.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamptz");

                e.Property(t => t.Name).HasColumnName("tagname");

                e.Property(t => t.Contents).HasColumnName("contents");
            });

            model.Entity<DatabaseCommandId>(e =>
            {
                e.HasIndex(t => t.Id).HasName("index_id").IsUnique();
                e.HasKey(t => t.Command).HasName("command_qualified");
                e.HasAlternateKey(t => t.Id).HasName("id");
                
                e.ToTable("mcore_cmd_state");

                e.Property(t => t.Id)
                    .HasColumnName("id")
                    .HasColumnType("smallint")
                    .ValueGeneratedOnAdd()

                    .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                    .HasAnnotation("Npgsql:ValueGeneratedOnAdd", true)
                    .HasAnnotation("Sqlite:Autoincrement", true);
                
                e.Property(t => t.Command).HasColumnName("command_qualified");
            });
        }
    }
}
