using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using ModCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore
{
    public class CommandLineHandler
    {
        public static bool Handle(string[] args)
        {
            if(args.Length == 0)
            {
                return true;
            }

            if (args.Contains("--migrate"))
            {
                Console.WriteLine("Applying migrations to database...");
                var db = new DatabaseContext();
                db.Database.Migrate();
                Console.WriteLine("Done applying migrations.");
            }
            else if (args.Contains("--rollback-one"))
            {
                Console.WriteLine("Undoing latest migration from database...");
                var db = new DatabaseContext();
                var migrations = db.Database.GetAppliedMigrations();
                Console.WriteLine($"latest migration: {migrations.Last()}");
                var rollback = migrations.ElementAt(migrations.Count() - 2);
                Console.WriteLine($"Rolling back to migration: {rollback}");
                db.GetInfrastructure().GetService<IMigrator>().Migrate(rollback);
                Console.WriteLine("Done undoing migration.");
            }
            else if (args.Contains("--rollback-full"))
            {
                Console.WriteLine("Undoing all migrations from database...");
                var db = new DatabaseContext();
                db.GetInfrastructure().GetService<IMigrator>().Migrate("0");
                Console.WriteLine("Done undoing migrations.");
            }
            else if (args.Contains("--generate-configs"))
            {
                Console.WriteLine("Pregenerating config files...");
                new ConfigService().GetConfig();
                Console.WriteLine("Config files pregenerated.");
            }
            else
            {
                Console.WriteLine("Available commands:");
            }

            return false;
        }
    }
}
