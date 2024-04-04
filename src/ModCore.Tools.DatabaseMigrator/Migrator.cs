using Microsoft.EntityFrameworkCore;
using ModCore.Common.Database;
using ModCore.Tools.DatabaseMigrator.ClassicDatabase;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Tools.DatabaseMigrator
{
    public class Migrator
    {
        private DatabaseContext _newDatabase;
        private ClassicDatabaseContext _oldDatabase;

        public Migrator(string oldDB, string newDB, string username, string pass, string host, int port)
        {
            var oldCStringBuilder = new NpgsqlConnectionStringBuilder()
            {
                Database = oldDB,
                Username = username,
                Password = pass,
                Port = port,
                Host = host,
                IncludeErrorDetail = true
            };
            var newCStringBuilder = new NpgsqlConnectionStringBuilder()
            {
                Database = newDB,
                Username = username,
                Password = pass,
                Port = port,
                Host = host,
                IncludeErrorDetail = true
            };

            _newDatabase = new DatabaseContext(newCStringBuilder.ToString());
            _oldDatabase = new ClassicDatabaseContext(oldCStringBuilder.ToString());
        }

        public void StartMigration()
        {
            var migrations = _newDatabase.Database.GetPendingMigrations();
            if(migrations.Count() > 0)
            {
                Cons.Write("Pending migrations found for new database. Apply? (y/N)");
                var confirm = (Console.ReadLine() ?? "n").Trim().ToLower() == "y";
                if (!confirm)
                {
                    Cons.WriteLine("Migrations pending were not applied. Cancelling operation.", ConsoleColor.Red);
                    return;
                }
                Cons.WriteLine("Applying latest migrations to new Database:");
                foreach(var migration in migrations)
                {
                    Cons.WriteLine(migration, ConsoleColor.Magenta);
                }
                _newDatabase.Database.Migrate();
                Cons.WriteLine("Applied migrations!", ConsoleColor.Green);
            }

            foreach(var tag in _oldDatabase.Tags)
            {
                Cons.WriteLine($"g:{tag.GuildId} c:{tag.ChannelId} o:{tag.OwnerId} n:{tag.Name}", ConsoleColor.Yellow);
            }

            foreach(var cfg in _oldDatabase.GuildConfig)
            {
                var sett = cfg.GetSettings();
                Cons.WriteLine($"g:{cfg.GuildId} sb:{sett.Starboard.Enable}", ConsoleColor.Magenta);
            }
        }
    }
}
