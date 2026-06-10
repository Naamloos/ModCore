using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ModCore.Common.Configuration;
using ModCore.Common.Database.Entities;
using ModCore.Common.Database.Interceptors;
using Npgsql;
using static Microsoft.EntityFrameworkCore.NpgsqlModelBuilderExtensions;

// dotnet-ef database update (or run ModCore once to auto-apply)
// dotnet-ef migrations add MigrationName
// dotnet-ef database update (or run ModCore once to auto-apply)

// to revert, dotnet-ef database update MigrationToRollbackToName

// make sure to copy your debug settings.json to the build dir of the ModCore.Common.Database project!

namespace ModCore.Common.Database
{
    public class DatabaseContext : DbContext
    {
        public virtual DbSet<DatabaseGuild> Guilds { get; set; }
        public virtual DbSet<DatabaseLevelData> LevelData { get; set; }
        public virtual DbSet<DatabaseStarboard> Starboards { get; set; }
        public virtual DbSet<DatabaseStarboardItem> StarboardItems { get; set; }
        public virtual DbSet<DatabaseTag> Tags { get; set; }
        public virtual DbSet<DatabaseTagHistory> TagsHistory { get; set; }
        public virtual DbSet<DatabaseNicknameState> NicknameStates { get; set; }
        public virtual DbSet<DatabaseRoleState> RoleStates { get; set; }
        public virtual DbSet<DatabaseOverrideState> OverrideStates { get; set; }
        public virtual DbSet<DatabaseAutoRole> AutoRoles { get; set; }
        public virtual DbSet<DatabaseInfraction> Infractions { get; set; }
        public virtual DbSet<DatabaseUser> Users { get; set; }
        public virtual DbSet<DatabaseBanAppeal> BanAppeals { get; set; }
        public virtual DbSet<DatabaseRoleMenu> RoleMenus { get; set; }
        public virtual DbSet<DatabaseRoleMenuRole> RoleMenusRoles { get; set; }
        public virtual DbSet<DatabaseLoggerSettings> LoggerSettings { get; set; }
        public virtual DbSet<DatabaseWelcomeSettings> WelcomeSettings { get; set; }
        public virtual DbSet<DatabaseTicket> Tickets { get; set; }
        public virtual DbSet<DatabaseTimer> Timers { get; set; }
        public virtual DbSet<DatabaseLevelSettings> LevelSettings { get; set; }

        private readonly IConfiguration? _config;

        public DatabaseContext(IConfiguration config)
        {
            this._config = config;
        }

        public async Task TouchGuild(ulong guildId)
        {
            var guild = await this.Guilds.FindAsync(guildId);
            if (guild is null)
            {
                await this.Guilds.AddAsync(new DatabaseGuild()
                {
                    GuildId = guildId,
                });
                await this.SaveChangesAsync();
            }
            guild = await this.Guilds.FindAsync(guildId);
            guild.LastSeenAt = DateTime.UtcNow;
            this.Guilds.Update(guild);
            await this.SaveChangesAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
#endif

            if (_config != null)
            {
                var cStringBuilder = new NpgsqlConnectionStringBuilder()
                {
                    Database = _config.GetRequiredSection(ConfigurationHelper.GetConfigKeyString(ConfigKey.PostgresDatabase)).Value!,
                    Username = _config.GetRequiredSection(ConfigurationHelper.GetConfigKeyString(ConfigKey.PostgresUsername)).Value!,
                    Password = _config.GetRequiredSection(ConfigurationHelper.GetConfigKeyString(ConfigKey.PostgresPassword)).Value!,
                    Port = int.Parse(_config.GetRequiredSection(ConfigurationHelper.GetConfigKeyString(ConfigKey.PostgresPort)).Value!),
                    Host = _config.GetRequiredSection(ConfigurationHelper.GetConfigKeyString(ConfigKey.PostgresHost)).Value!,
                    IncludeErrorDetail = true
                };

                var encryptionKey = _config.GetRequiredSection(ConfigurationHelper.GetConfigKeyString(ConfigKey.MasterKey)).Value!;

                if (!optionsBuilder.IsConfigured)
                {
                    optionsBuilder.UseNpgsql(cStringBuilder.ToString());
                }

                optionsBuilder.AddInterceptors(
                    new EncryptionInterceptor(encryptionKey),
                    new DecryptionInterceptor(encryptionKey)
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DatabaseGuild>()
                .HasKey(x => x.GuildId);
            modelBuilder.Entity<DatabaseUser>()
                .HasKey(x => x.UserId);

            modelBuilder.Entity<DatabaseLevelData>()
                .HasKey(x => new { x.GuildId, x.UserId });
            modelBuilder.Entity<DatabaseLevelData>()
                .HasOne(x => x.Guild)
                .WithMany(x => x.LevelData)
                .HasForeignKey(x => x.GuildId);
            modelBuilder.Entity<DatabaseLevelData>()
                .HasOne(x => x.User)
                .WithMany(x => x.LevelData)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseStarboard>()
                .HasKey(x => new { x.Id });
            modelBuilder.Entity<DatabaseStarboard>()
                .Property(x => x.Id)
                .UseIdentityAlwaysColumn();
            modelBuilder.Entity<DatabaseStarboard>()
                .HasOne(x => x.Guild)
                .WithMany(x => x.Starboards)
                .HasForeignKey(x => x.GuildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseStarboardItem>()
                .HasKey(x => new { x.StarboardId, x.MessageId, x.ChannelId, x.StargazerId });
            modelBuilder.Entity<DatabaseStarboardItem>()
                .HasOne(x => x.Starboard)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.StarboardId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DatabaseStarboardItem>()
                .HasOne(x => x.Author)
                .WithMany(x => x.StarboardItems)
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DatabaseStarboardItem>()
                .HasOne(x => x.Stargazer)
                .WithMany(x => x.StarredItems)
                .HasForeignKey(x => x.StargazerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseTag>()
                .HasKey(x => new { x.Id });
            modelBuilder.Entity<DatabaseTag>()
                .Property(x => x.Id)
                .UseIdentityAlwaysColumn();
            modelBuilder.Entity<DatabaseTag>()
                .HasOne(x => x.Guild)
                .WithMany(x => x.Tags)
                .HasForeignKey(x => x.GuildId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DatabaseTag>()
                .HasOne(x => x.Author)
                .WithMany(x => x.Tags)
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseTagHistory>()
                .HasKey(x => new { x.Id });
            modelBuilder.Entity<DatabaseTagHistory>()
                .HasOne(x => x.Tag)
                .WithMany(x => x.History)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseNicknameState>()
                .HasKey(x => new { x.GuildId, x.UserId });
            modelBuilder.Entity<DatabaseNicknameState>()
                .HasOne(x => x.Guild)
                .WithMany(x => x.NicknameStates)
                .HasForeignKey(x => x.GuildId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DatabaseNicknameState>()
                .HasOne(x => x.User)
                .WithMany(x => x.NicknameStates)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseRoleState>()
                .HasKey(x => new { x.RoleId, x.UserId, x.GuildId });
            modelBuilder.Entity<DatabaseRoleState>()
                .HasOne(x => x.Guild)
                .WithMany(x => x.RoleStates)
                .HasForeignKey(x => x.GuildId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DatabaseRoleState>()
                .HasOne(x => x.User)
                .WithMany(x => x.RoleStates)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseOverrideState>()
                .HasKey(x => new { x.GuildId, x.UserId, x.ChannelId });
            modelBuilder.Entity<DatabaseOverrideState>()
                .HasOne(x => x.Guild)
                .WithMany(x => x.OverrideStates)
                .HasForeignKey(x => x.GuildId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DatabaseOverrideState>()
                .HasOne(x => x.User)
                .WithMany(x => x.OverrideStates)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseAutoRole>()
                .HasKey(x => new { x.GuildId, x.RoleId });
            modelBuilder.Entity<DatabaseAutoRole>()
                .HasOne(x => x.Guild)
                .WithMany(x => x.AutoRoles)
                .HasForeignKey(x => x.GuildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseInfraction>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<DatabaseInfraction>()
                .Property(x => x.Id)
                .UseIdentityAlwaysColumn();
            modelBuilder.Entity<DatabaseInfraction>()
                .HasOne(x => x.Guild)
                .WithMany(x => x.Infractions)
                .HasForeignKey(x => x.GuildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseBanAppeal>()
                .HasKey(x => new { x.UserId, x.GuildId });
            modelBuilder.Entity<DatabaseBanAppeal>()
                .HasOne(x => x.User)
                .WithMany(x => x.BanAppeals)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DatabaseBanAppeal>()
                .HasOne(x => x.Guild)
                .WithMany(x => x.BanAppeals)
                .HasForeignKey(x => x.GuildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseTicket>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<DatabaseTicket>()
                .Property(x => x.Id)
                .UseIdentityAlwaysColumn();
            modelBuilder.Entity<DatabaseTicket>()
                .HasOne(x => x.Author)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DatabaseTicket>()
                .HasOne(x => x.Guild)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.GuildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseRoleMenu>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<DatabaseRoleMenu>()
                .Property(x => x.Id)
                .UseIdentityAlwaysColumn();
            modelBuilder.Entity<DatabaseRoleMenu>()
                .HasOne(x => x.Guild)
                .WithMany(x => x.RoleMenus)
                .HasForeignKey(x => x.GuildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseRoleMenuRole>()
                .HasKey(x => new { x.RoleId, x.MenuId });
            modelBuilder.Entity<DatabaseRoleMenuRole>()
                .HasOne(x => x.Menu)
                .WithMany(x => x.Roles)
                .HasForeignKey(x => x.MenuId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseLoggerSettings>()
                .HasKey(x => x.GuildId);
            modelBuilder.Entity<DatabaseLoggerSettings>()
                .HasOne(x => x.Guild)
                .WithOne(x => x.LoggerSettings)
                .HasForeignKey<DatabaseLoggerSettings>(x => x.GuildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseWelcomeSettings>()
                .HasKey(x => x.GuildId);
            modelBuilder.Entity<DatabaseWelcomeSettings>()
                .HasOne(x => x.Guild)
                .WithOne(x => x.WelcomeSettings)
                .HasForeignKey<DatabaseWelcomeSettings>(x => x.GuildId);

            modelBuilder.Entity<DatabaseTimer>()
                .HasKey(x => x.TimerId);
            modelBuilder.Entity<DatabaseTimer>()
                .Property(x => x.TimerId)
                .UseIdentityAlwaysColumn();

            modelBuilder.Entity<DatabaseLevelSettings>()
                .HasKey(x => x.GuildId);
            modelBuilder.Entity<DatabaseLevelSettings>()
                .HasOne(x => x.Guild)
                .WithOne(x => x.LevelSettings)
                .HasForeignKey<DatabaseLevelSettings>(x => x.GuildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.UseIdentityAlwaysColumns();
        }
    }
}