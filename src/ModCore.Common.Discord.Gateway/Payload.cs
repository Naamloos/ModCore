using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

        public T? GetDataAs<T>()
        {
            return Data.Deserialize<T>();
        }

        public void SetData<T>(T data)
        {
            Data = JsonSerializer.SerializeToElement(data);
        }

        public Payload WithData<T>(T data)
        {
            this.SetData(data);
            return this;
        }

        public Payload(OpCodes opCode)
        {
            this.OpCode = opCode;
        }
    }
}
