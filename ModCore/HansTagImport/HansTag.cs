using Newtonsoft.Json;
using System;

namespace ModCore.HansTagImport
{
    public struct HansTag
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("guild")]
        public ulong Guild {  get; set; }

        [JsonProperty("channel")]
        public ulong? Channel { get; set; }

        // unknown
        [JsonProperty("kind")]
        public int Kind { get; set; }

        [JsonProperty("owner")]
        public ulong Owner { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }

        [JsonProperty("latestRevision")]
        public DateTimeOffset LatestRevision { get; set; }

        [JsonProperty("aliases")]
        public string[] Aliases { get; set; }

        [JsonProperty("revisions")]
        public HansTagRevision[] Revisions { get; set; }
    }

    public struct HansTagRevision
    {
        [JsonProperty("contents")]
        public string Contents { get; set; }

        [JsonProperty("created")]
        public DateTimeOffset Created { get; set; }

        [JsonProperty("user")]
        public ulong User {  get; set; }
    }
}
