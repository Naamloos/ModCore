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
                // This is a legacy fallback, MSBuild should ensure the settings file exists and gets copied from the repository directory to output.
                File.Create("settings.json").Close();
                File.WriteAllText("settings.json", JsonSerializer.Serialize(new Settings(), jsonOptions));
            }

            // ensure new config values are written
            var contents = File.ReadAllText("settings.json");
            var settings = JsonSerializer.Deserialize<Settings>(contents, jsonOptions);
            File.WriteAllText("settings.json", JsonSerializer.Serialize(settings, jsonOptions));

            return settings!;
        }

        public static string GetPathToSettings()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
        }
    }
}
