using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Tools.DatabaseMigrator
{
    public static class Cons
    {
        public static void Write(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.Write($"{message}");
            Console.ResetColor();
        }

        public static void WriteLine(string message, ConsoleColor color = ConsoleColor.White)
        {
            Write(message + Environment.NewLine, color);
        }

        public static string? ReadLine() => Console.ReadLine();
    }
}
