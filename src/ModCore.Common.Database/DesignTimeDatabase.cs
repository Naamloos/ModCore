using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Database
{
    public class DesignTimeDatabase : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var settingsPath = Path.Combine(directory!, "settings.json"); // This is your ModCore debug's settings.json, throw it in build dir!

            var obj = JsonSerializer.Deserialize<JsonObject>(File.ReadAllText(settingsPath))!;

            var cStringBuilder = new NpgsqlConnectionStringBuilder()
            {
                Database = obj["postgres_database"]!.GetValue<string>(),
                Username = obj["postgres_username"]!.GetValue<string>(),
                Password = obj["postgres_password"]!.GetValue<string>(),
                Port = obj["postgres_port"]!.GetValue<int>(),
                Host = obj["postgres_host"]!.GetValue<string>()
            };

            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseNpgsql(cStringBuilder.ToString());

            return new DatabaseContext(options.Options);
        }
    }
}
