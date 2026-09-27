using System.Text.Json;
using ModCore.Common.Discord.Gateway.Events;

namespace ModCore.Common.Discord.Gateway.EventData.Incoming
{
    public record UnknownDispatch : IPublishable
    {
        public string Name { get; set; } = "";
        public JsonElement Data { get; set; }
        public int? Sequence { get; set; }
    }
}
