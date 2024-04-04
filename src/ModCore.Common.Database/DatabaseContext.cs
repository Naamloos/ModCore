using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ModCore.Common.Database.Entities;
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

        private string _connectionString;

        public DatabaseContext(IConfiguration config)
        {
            var cStringBuilder = new NpgsqlConnectionStringBuilder()
            {
                Database = config.GetRequiredSection("postgres_database").Value!,
                Username = config.GetRequiredSection("postgres_username").Value!,
                Password = config.GetRequiredSection("postgres_password").Value!,
                Port = int.Parse(config.GetRequiredSection("postgres_port").Value!),
                Host = config.GetRequiredSection("postgres_host").Value!,
                IncludeErrorDetail = true
            };

            this._connectionString = cStringBuilder.ToString();
        }

        public DatabaseContext(string cstring)
        {
            this._connectionString = cstring;
        }

        internal DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            #if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
            #endif

            optionsBuilder.UseNpgsql(this._connectionString);
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
                .WithMany(x =>x.LevelData)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseStarboard>()
                .HasKey(x => new { x.Id });
            modelBuilder.Entity<DatabaseStarboard>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<DatabaseStarboard>()
                .HasOne(x => x.Guild)
                .WithMany(x => x.Starboards)
                .HasForeignKey(x => x.GuildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseStarboardItem>()
                .HasKey(x => new { x.StarboardId, x.MessageId, x.ChannelId });
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

            modelBuilder.Entity<DatabaseTag>()
                .HasKey(x => new {x.Id});
            modelBuilder.Entity<DatabaseTag>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();
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
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<DatabaseInfraction>()
                .HasOne(x =>x.Guild)
                .WithMany(x => x.Infractions)
                .HasForeignKey(x => x.GuildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseBanAppeal>()
                .HasKey(x => new{ x.UserId, x.GuildId });
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
                .ValueGeneratedOnAdd();
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
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<DatabaseRoleMenu>()
                .HasOne(x => x.Guild)
                .WithMany(x => x.RoleMenus)
                .HasForeignKey(x => x.GuildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseRoleMenuRole>()
                .HasKey(x => new {x.RoleId, x.MenuId});
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
                .ValueGeneratedOnAdd();

            modelBuilder.UseIdentityAlwaysColumns();
        }
    }
}