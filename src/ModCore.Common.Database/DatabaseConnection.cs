using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ModCore.Common.Database.Entities;
using Npgsql;

// dotnet-ef database update (or run ModCore once to auto-apply)
// dotnet-ef migrations add MigrationName
// dotnet-ef database update (or run ModCore once to auto-apply)

// to revert, dotnet-ef database update MigrationToRollbackToName

// make sure to copy your debug settings.json to the build dir of the ModCore.Common.Database project!

namespace ModCore.Common.Database
{
    public class DatabaseConnection : DbContext
    {
        public virtual DbSet<DatabaseGuild> Guilds { get; set; }
        public virtual DbSet<DatabaseLevelData> LevelData { get; set; }

        private string _connectionString;
        private bool inMemory = false;

        public DatabaseConnection(IConfiguration config)
        {
            var cStringBuilder = new NpgsqlConnectionStringBuilder()
            {
                Database = config.GetRequiredSection("postgres_database").Value!,
                Username = config.GetRequiredSection("postgres_username").Value!,
                Password = config.GetRequiredSection("postgres_password").Value!,
                Port = int.Parse(config.GetRequiredSection("postgres_port").Value!),
                Host = config.GetRequiredSection("postgres_host").Value!
            };

            this._connectionString = cStringBuilder.ToString();
        }

        internal DatabaseConnection(DbContextOptions<DatabaseConnection> options) : base(options)
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
                .HasKey(x => x.Id);
            modelBuilder.Entity<DatabaseGuild>()
                .HasMany(x => x.LevelData)
                .WithOne(x => x.Guild);

            modelBuilder.Entity<DatabaseLevelData>()
                .HasKey(x => new { x.GuildId, x.UserId });

            modelBuilder.Entity<DatabaseLevelData>()
                .HasOne(x => x.Guild)
                .WithMany(x => x.LevelData);

            base.OnModelCreating(modelBuilder);
        }
    }
}