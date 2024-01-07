namespace ModCore.Tools.DatabaseMigrator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Welcome, to the ModCore database migrator tool!" +
                "\nThis tool will assist you in migrating data from a v2 ModCore database to a ModCore v3 database." +
                "\nWhy write all this text when I'm the only user?" +
                "\nL Bozo Cope\n");
            Console.ResetColor();

            Console.Write("DB Host: ");
            var host = Console.ReadLine();
            Console.Write("DB Username: ");
            var username = Console.ReadLine();
            Console.Write("DB Password: ");
            var password = Console.ReadLine();
            Console.Write("Old DB name: ");
            var oldDB = Console.ReadLine();
            Console.Write("New DB name: ");
            var newDB = Console.ReadLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Getting ready to migrate from v2 DB ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{oldDB}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" to v3 DB ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{newDB}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(". Are you sure you want to proceed? (Y/n): ");
            Console.ResetColor();
            var agree = Console.ReadLine();
        }
    }
}
