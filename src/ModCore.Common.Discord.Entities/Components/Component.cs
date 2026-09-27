using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ModCore.Common.Discord.Entities.Enums;

namespace ModCore.Common.Discord.Entities.Components
{
    public class Component
    {
        [JsonPropertyName("type")]
        public virtual ComponentType Type { get; set; }

        [JsonPropertyName("id")]
        public Optional<int> Id { get; set; } = Optional<int>.None;

        [JsonExtensionData]
        public Dictionary<string, JsonElement> AdditionalData { get; set; } = new();
    }
}
