using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModCore.Logic
{
    public class BitArraySerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is BitSet bitset))
            {
                writer.WriteNull();
                return;
            }
            writer.WriteStartArray();
            foreach(var b in (IEnumerable<byte>) bitset)
            {
                writer.WriteValue(b);
            }
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.TokenType == JsonToken.Null ? null : new BitSet(JArray.Load(reader).Values<byte>());
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(BitSet).IsAssignableFrom(objectType);
        }
    }


}