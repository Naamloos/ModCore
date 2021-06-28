using Microsoft.EntityFrameworkCore;
using ModCore.Entities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ModCore.Services
{
    public class DatabaseService
    {
        private DatabaseContext db;

        public DatabaseService(ConfigService configService)
        {
            this.db = new DatabaseContext(configService.GetConfig());
        }

        public void Connect()
        {
            this.db.Database.EnsureCreated();
        }

        public DatabaseContext GetDatabase()
        {
            return this.db;
        }
    }

    public class DatabaseContext : DbContext
    {
        public DbSet<GuildData> GuildData { get; set; }

        public DbSet<ChannelData> ChannelData { get; set; }

        public DbSet<UserData> UserData { get; set; }

        public DbSet<TimerData> TimerData { get; set; }

        private string connectionString;

        public DatabaseContext(Config config)
        {
            var builder = new NpgsqlConnectionStringBuilder();
            builder.Host = config.DatabaseHost;
            builder.Username = config.DatabaseUsername;
            builder.Password = config.DatabasePassword;
            builder.Database = config.DatabaseName;
            builder.Port = config.DatabasePort;

            this.connectionString = builder.ToString();
            this.Database.SetCommandTimeout(TimeSpan.FromSeconds(30));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseNpgsql(this.connectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // overriding model creation for entities that
            // require +composite keys.
            modelBuilder.Entity<ChannelData>()
                .HasKey(e => new { e.GuildId, e.ChannelId });

            modelBuilder.Entity<GuildData>()
                .HasKey(e => e.GuildId);

            modelBuilder.Entity<UserData>()
                .HasKey(e => e.UserId);

            modelBuilder.Entity<TimerData>()
                .HasKey(e => e.TimerId);
        }
    }
}
