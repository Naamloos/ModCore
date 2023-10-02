using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Gateway.EventData.Outgoing
{
    internal record Identify
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("properties")]
        public ConnectionProperties Properties { get; set; } = new ConnectionProperties();

        [JsonPropertyName("compress")]
        public bool Compress { get; set; } = false;

        [JsonPropertyName("large_treshold")]
        public int LargeTreshold { get; set; } = 50;

        [JsonPropertyName("shard")]
        public int[]? Shard { get; set; } = new int[] { 0, 1 };

        [JsonPropertyName("presence")]
        public PresenceUpdate? Presence { get; set; } = new PresenceUpdate();

        [JsonPropertyName("intents")]
        public Intents Intents { get; set; }
    }

    internal record PresenceUpdate
    {
        [JsonPropertyName("since")]
        public int? Since { get; set; } = null;

        [JsonPropertyName("activities")]
        public List<Activity> activities { get; set; } = new List<Activity>() { new Activity() };

        [JsonPropertyName("status")]
        public string Status { get; set; } = "online";

        [JsonPropertyName("afk")]
        public bool Afk { get; set; } = false;
    }

    internal record Activity
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "ModCore.Common.Discord";

        [JsonPropertyName("type")]
        public int Type { get; set; } = 0;
        // TODO implement the rest https://discord.com/developers/docs/topics/gateway-events#activity-object
    }

    internal record ConnectionProperties
    {
        [JsonPropertyName("os")]
        public string OperatingSystem { get; set; } = "Linux";

        [JsonPropertyName("browser")]
        public string Browser { get; set; } = "ModCore v3";

        [JsonPropertyName("device")]
        public string Device { get; set; } = "ModCore v3";
    }
}
