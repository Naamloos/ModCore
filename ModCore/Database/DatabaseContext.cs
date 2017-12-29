using Microsoft.EntityFrameworkCore;
using ModCore.Entities;

namespace ModCore.Database
{
    public partial class DatabaseContext : DbContext
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
        public virtual DbSet<DatabaseBotManager> BotManagers { get; set; }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DatabaseInfo>(entity =>
            {
                entity.ToTable("mcore_database_info");

                entity.HasIndex(e => e.MetaKey)
                    .HasName("mcore_database_info_meta_key_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.MetaKey)
                    .IsRequired()
                    .HasColumnName("meta_key");

                entity.Property(e => e.MetaValue)
                    .IsRequired()
                    .HasColumnName("meta_value");
            });

            modelBuilder.Entity<DatabaseGuildConfig>(entity =>
            {
                entity.ToTable("mcore_guild_config");

                entity.HasIndex(e => e.GuildId)
                    .HasName("mcore_guild_config_guild_id_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.GuildId).HasColumnName("guild_id");

                entity.Property(e => e.Settings)
                    .IsRequired()
                    .HasColumnName("settings")
                    .HasColumnType("jsonb");
            });

            modelBuilder.Entity<DatabaseModNote>(entity =>
            {
                entity.ToTable("mcore_modnotes");

                entity.HasIndex(e => new { e.MemberId, e.GuildId })
                    .HasName("mcore_modnotes_member_id_guild_id_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Contents).HasColumnName("contents");

                entity.Property(e => e.GuildId).HasColumnName("guild_id");

                entity.Property(e => e.MemberId).HasColumnName("member_id");
            });

            modelBuilder.Entity<DatabaseRolestateOverride>(entity =>
            {
                entity.ToTable("mcore_rolestate_overrides");

                entity.HasIndex(e => new { e.MemberId, e.GuildId, e.ChannelId })
                    .HasName("mcore_rolestate_overrides_member_id_guild_id_channel_id_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ChannelId).HasColumnName("channel_id");

                entity.Property(e => e.GuildId).HasColumnName("guild_id");

                entity.Property(e => e.MemberId).HasColumnName("member_id");

                entity.Property(e => e.PermsAllow).HasColumnName("perms_allow");

                entity.Property(e => e.PermsDeny).HasColumnName("perms_deny");
            });

            modelBuilder.Entity<DatabaseRolestateRoles>(entity =>
            {
                entity.ToTable("mcore_rolestate_roles");

                entity.HasIndex(e => new { e.MemberId, e.GuildId })
                    .HasName("mcore_rolestate_roles_member_id_guild_id_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.GuildId).HasColumnName("guild_id");

                entity.Property(e => e.MemberId).HasColumnName("member_id");

                entity.Property(e => e.RoleIds).HasColumnName("role_ids");

                if (this.Provider != DatabaseProvider.PostgreSql)
                    entity.Ignore(e => e.RoleIds);
            });

            modelBuilder.Entity<DatabaseWarning>(entity =>
            {
                entity.ToTable("mcore_warnings");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.GuildId).HasColumnName("guild_id");

                entity.Property(e => e.IssuedAt)
                    .HasColumnName("issued_at")
                    .HasColumnType("timestamptz");

                entity.Property(e => e.IssuerId).HasColumnName("issuer_id");

                entity.Property(e => e.MemberId).HasColumnName("member_id");

                entity.Property(e => e.WarningText)
                    .IsRequired()
                    .HasColumnName("warning_text");
            });

            modelBuilder.Entity<DatabaseTimer>(entity =>
            {
                entity.ToTable("mcore_timers");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.GuildId).HasColumnName("guild_id");

                entity.Property(e => e.ChannelId).HasColumnName("channel_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.DispatchAt)
                    .HasColumnName("dispatch_at")
                    .HasColumnType("timestamptz");

                entity.Property(e => e.ActionType).HasColumnName("action_type");

                entity.Property(e => e.ActionData)
                    .IsRequired()
                    .HasColumnName("action_data")
                    .HasColumnType("jsonb");
            });

            modelBuilder.Entity<DatabaseStarData>(entity =>
            {
                entity.ToTable("mcore_stars");

                entity.HasIndex(e => new { e.MessageId, e.ChannelId, e.StargazerId })
                    .HasName("mcore_stars_member_id_guild_id_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.StargazerId).HasColumnName("stargazer_id");

                entity.Property(e => e.StarboardMessageId).HasColumnName("starboard_entry_id");

                entity.Property(e => e.MessageId).HasColumnName("message_id");

                entity.Property(e => e.AuthorId).HasColumnName("author_id");

                entity.Property(e => e.GuildId).HasColumnName("guild_id");

                entity.Property(e => e.ChannelId).HasColumnName("channel_id");
            });

            modelBuilder.Entity<DatabaseBan>(entity =>
            {
                entity.ToTable("mcore_bans");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.GuildId).HasColumnName("guild_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.IssuedAt)
                .HasColumnName("issued_at")
                .HasColumnType("timestamptz");

                entity.Property(e => e.BanReason).HasColumnName("ban_reason");
            });

            modelBuilder.Entity<DatabaseTag>(entity =>
            {
                entity.HasIndex(e => new { e.ChannelId, e.Name })
                    .HasName("mcore_tags_channel_id_tag_name_key")
                    .IsUnique();

                entity.ToTable("mcore_tags");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ChannelId).HasColumnName("channel_id");

                entity.Property(e => e.OwnerId).HasColumnName("owner_id");

                entity.Property(e => e.CreatedAt).HasColumnName("created_at")
                .HasColumnType("timestamptz");

                entity.Property(e => e.Name).HasColumnName("tagname");

                entity.Property(e => e.Contents).HasColumnName("contents");
            });

            modelBuilder.Entity<DatabaseBotManager>(entity =>
            {
                entity.ToTable("mcore_botmanager");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });
        }
    }
}
