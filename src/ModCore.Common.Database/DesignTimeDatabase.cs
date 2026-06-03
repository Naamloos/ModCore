using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ModCore.Common.Database
{
    public class DesignTimeDatabase : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var settingsPath = Path.Combine(directory!, "settings.json");

            var obj = JsonSerializer.Deserialize<JsonObject>(File.ReadAllText(settingsPath))!;

            var configValues = new Dictionary<string, string?>
            {
                ["postgres_database"] = obj["postgres_database"]!.GetValue<string>(),
                ["postgres_username"] = obj["postgres_username"]!.GetValue<string>(),
                ["postgres_password"] = obj["postgres_password"]!.GetValue<string>(),
                ["postgres_port"] = obj["postgres_port"]!.GetValue<int>().ToString(),
                ["postgres_host"] = obj["postgres_host"]!.GetValue<string>(),
                ["master_key"] = obj["master_key"]!.GetValue<string>()
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

            return new DatabaseContext(config);
        }
    }
}