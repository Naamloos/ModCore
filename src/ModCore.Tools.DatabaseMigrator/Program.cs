namespace ModCore.Tools.DatabaseMigrator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MigratorConsole.WriteLine("Welcome, to the ModCore database migrator tool!" +
                "\nThis tool will assist you in migrating data from a v2 ModCore database to a v3 ModCore database." +
                "\nWhy write all this text when I'm the only user?" +
                "\nL Bozo Cope\n", ConsoleColor.Magenta);

            MigratorConsole.Write("DB Host: ");
            var host = Console.ReadLine();
            MigratorConsole.Write("DB Port: ");
            var port = int.Parse(MigratorConsole.ReadLine() ?? "5432");
            MigratorConsole.Write("DB Username: ");
            var username = Console.ReadLine();
            MigratorConsole.Write("DB Password: ");
            var password = Console.ReadLine();
            MigratorConsole.Write("Old DB name: ");
            var oldDB = Console.ReadLine();
            MigratorConsole.Write("New DB name: ");
            var newDB = Console.ReadLine();
            MigratorConsole.Write("Getting ready to migrate from v2 DB ", ConsoleColor.Red);
            MigratorConsole.Write($"{oldDB}", ConsoleColor.Green);
            MigratorConsole.Write(" to v3 DB ", ConsoleColor.Red);
            MigratorConsole.Write($"{newDB}", ConsoleColor.Green);
            MigratorConsole.Write(". Are you sure you want to proceed? (y/N): ", ConsoleColor.Red);
            var agree = (MigratorConsole.ReadLine() ?? "n").Trim().ToLower() == "y";
            if(agree)
            {
                var migrator = new Migrator(oldDB, newDB, username, password, host, port);
                migrator.StartMigration();
            }
            else
            {
                MigratorConsole.WriteLine("Operation canceled by user.");
            }

            Console.ReadKey();
        }
    }
}
