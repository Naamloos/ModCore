using ModCore.Entities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace ModCore
{
    class Program
    {
        static Bot ModCore;
        static void Main(string[] args)
        {
            if (!File.Exists("settings.json"))
            {
                var json = JsonConvert.SerializeObject(new Settings());
                File.WriteAllText("settings.json", json, new UTF8Encoding(false));
                Console.WriteLine("Config file was not found, a new one was generated. Fill it with proper values and rerun this program");
                Console.ReadKey();
                return;
            }

            var input = File.ReadAllText("settings.json", new UTF8Encoding(false));
            var cfg = JsonConvert.DeserializeObject<Settings>(input);

            ModCore = new Bot(cfg);
            ModCore.RunAsync().GetAwaiter().GetResult();
        }
    }
}
