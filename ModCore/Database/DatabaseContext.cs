using Microsoft.EntityFrameworkCore;

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

        private string ConnectionString { get; }

        public DatabaseContext(string cstring)
        {
            this.ConnectionString = cstring;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseNpgsql(this.ConnectionString);
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
        }
    }
}
