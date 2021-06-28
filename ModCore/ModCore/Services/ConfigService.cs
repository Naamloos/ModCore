using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace ModCore.Services
{
    public class ConfigService
    {
        private const string CONFIG_FILENAME = "config.json";
        private string basePath;
        private Config config = null;

        public ConfigService()
        {
            basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        private Config loadFromFile()
        {
            var cfg = new Config();
            var path = Path.Combine(basePath, CONFIG_FILENAME);

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                cfg = JsonSerializer.Deserialize<Config>(json);
            }
            else
            {
                File.Create(path).Close();
            }

            File.WriteAllText(path, JsonSerializer.Serialize(cfg, new JsonSerializerOptions() { WriteIndented = true }));

            return cfg;
        }

        public Config GetConfig()
        {
            if (config == null)
            {
                this.config = loadFromFile();
            }

            return this.config;
        }
    }

    public class Config
    {
        [JsonInclude]
        public string Token = "";

        [JsonInclude]
        public string DefaultPrefix = "";
    }
}
