using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Components
{
    public abstract class InteractiveComponent : Component
    {
        [JsonPropertyName("custom_id")]
        public string CustomId { get; set; } = string.Empty;
    }
}
