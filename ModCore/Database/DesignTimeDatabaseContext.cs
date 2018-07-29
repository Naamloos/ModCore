using Microsoft.EntityFrameworkCore.Design;
using ModCore.Entities;
using Npgsql;

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
            return new DatabaseContext(DatabaseProvider.PostgreSql, 
                new NpgsqlConnectionStringBuilder
                {
                    Host = "localhost",
                    Port = 5439,
                    Database = "modcore",
                    Username = "postgres",
                    Password = null,
        
                    SslMode = SslMode.Prefer,
                    TrustServerCertificate = true,
        
                    Pooling = false
                }.ConnectionString);
        }
    }
}