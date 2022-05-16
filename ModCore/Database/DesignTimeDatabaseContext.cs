using Microsoft.EntityFrameworkCore.Design;
using ModCore.Entities;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.IO;
using System.Text;

namespace ModCore.Database
{
    /// <inheritdoc />
    /// <summary>
    /// For SQLite migration creation. To use:
    /// <code>
    /// dotnet ef migrations add MyModCoreMigration
    /// dotnet ef database update
    /// </code>
    /// Then simply copy the example.db file to your configured path.
    /// To update, just copy it back and do the same thing.
    /// </summary>
    public class DesignTimeDatabaseContext : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            if (!File.Exists("settings.json"))
            {
                var json = JsonConvert.SerializeObject(new Settings(), Formatting.Indented);
                File.WriteAllText("settings.json", json, new UTF8Encoding(false));
                throw new Exception("Generated a new config file. Please fill this out with your DB info.");
            }

            var input = File.ReadAllText("settings.json", new UTF8Encoding(false));
            var settings = JsonConvert.DeserializeObject<Settings>(input);
            var cstring = settings.Database.BuildConnectionString();

            return new DatabaseContext(DatabaseProvider.PostgreSql, cstring);
        }
    }
}