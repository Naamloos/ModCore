using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Gateway
{
    internal record Payload
    {
        [JsonPropertyName("op")]
        public OpCodes OpCode { get; set; }

        [JsonPropertyName("d")]
        public JsonElement Data { get; set; }

        [JsonPropertyName("s")]
        public int? Sequence { get; set; }

        [JsonPropertyName("t")]
        public string? EventName { get; set; }

        public T? GetDataAs<T>(JsonSerializerOptions options)
        {
            return Data.Deserialize<T>(options);
        }

        public void SetData<T>(T data, JsonSerializerOptions options)
        {
            Data = JsonSerializer.SerializeToElement(data, options);
        }

        public Payload WithData<T>(T data, JsonSerializerOptions options)
        {
            this.SetData(data, options);
            return this;
        }

        public Payload(OpCodes opCode)
        {
            this.OpCode = opCode;
        }
    }
}
