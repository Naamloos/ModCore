using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Components
{
    public class ComponentWithChildren : Component
    {
        [JsonPropertyName("components")]
        public List<Component> Components { get; set; } = new List<Component>();
    }
}
