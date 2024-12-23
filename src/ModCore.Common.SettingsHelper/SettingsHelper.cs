using System.Text.Json;
using ModCore.Common.Utils;

namespace ModCore.Common.SettingsHelper
{
    public static class SettingsHelper
    {
        public static Settings EnsureSettingsExist()
        {
            var jsonOptions = JsonSerializerOptionsFactory.GetOptions();

            if (!File.Exists("settings.json"))
            {
                File.Create("settings.json").Close();
                File.WriteAllText("settings.json", JsonSerializer.Serialize(new Settings(), jsonOptions));
            }

            // ensure new config values are written
            var contents = File.ReadAllText("settings.json");
            var settings = JsonSerializer.Deserialize<Settings>(contents, jsonOptions);
            File.WriteAllText("settings.json", JsonSerializer.Serialize(settings, jsonOptions));

            return settings;
        }
    }
}
