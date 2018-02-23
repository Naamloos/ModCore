using ModCore.Entities;
using Newtonsoft.Json;

namespace ModCore.Database
{
    public class DatabaseGuildConfig
    {
        public int Id { get; set; }
        public long GuildId { get; set; }
        public string Settings { get; set; }

        public GuildSettings GetSettings() =>
            JsonConvert.DeserializeObject<GuildSettings>(this.Settings);

        public void SetSettings(GuildSettings settings) =>
            this.Settings = JsonConvert.SerializeObject(settings);
    }
}
