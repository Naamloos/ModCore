using System.Security.Cryptography;

namespace ModCore.Tools.Keygen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==================================================");
            Console.WriteLine("                     ModCore v3                   ");
            Console.WriteLine("             Master Key Generation Tool           ");
            Console.WriteLine("==================================================");
            Console.ResetColor();

            // Cryptographically secure random key generation
            byte[] keyBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes);
            }

            string base64Key = Convert.ToBase64String(keyBytes);

            Console.WriteLine("Generated Master Key (Base64):");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(base64Key);
            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine("IMPORTANT: Store this key securely. It is required for encrypting and decrypting sensitive data in ModCore v3.");
            Console.WriteLine("Do NEVER share this key with ANYONE. This key gives full access to all encrypted data.");
            Console.WriteLine("Losing this key will result in permanent loss of access to all encrypted data. Make sure to back it up securely.");
            Console.WriteLine("\nPress the any key to exit...");
            Console.ReadKey();
        }
    }
}
