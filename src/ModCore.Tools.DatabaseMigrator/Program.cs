namespace ModCore.Tools.DatabaseMigrator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Cons.WriteLine("Welcome, to the ModCore database migrator tool!" +
                "\nThis tool will assist you in migrating data from a v2 ModCore database to a v3 ModCore database." +
                "\nWhy write all this text when I'm the only user?" +
                "\nL Bozo Cope\n", ConsoleColor.Magenta);

            Cons.Write("DB Host: ");
            var host = Console.ReadLine();
            Cons.Write("DB Port: ");
            var port = int.Parse(Cons.ReadLine() ?? "5432");
            Cons.Write("DB Username: ");
            var username = Console.ReadLine();
            Cons.Write("DB Password: ");
            var password = Console.ReadLine();
            Cons.Write("Old DB name: ");
            var oldDB = Console.ReadLine();
            Cons.Write("New DB name: ");
            var newDB = Console.ReadLine();
            Cons.Write("Getting ready to migrate from v2 DB ", ConsoleColor.Red);
            Cons.Write($"{oldDB}", ConsoleColor.Green);
            Cons.Write(" to v3 DB ", ConsoleColor.Red);
            Cons.Write($"{newDB}", ConsoleColor.Green);
            Cons.Write(". Are you sure you want to proceed? (y/N): ", ConsoleColor.Red);
            var agree = (Cons.ReadLine() ?? "n").Trim().ToLower() == "y";
            if(agree)
            {
                var migrator = new Migrator(oldDB, newDB, username, password, host, port);
                migrator.StartMigration();
            }
            else
            {
                Cons.WriteLine("Operation canceled by user.");
            }

            Console.ReadKey();
        }
    }
}
