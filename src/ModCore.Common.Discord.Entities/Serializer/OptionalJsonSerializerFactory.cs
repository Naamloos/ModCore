using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Serializer
{
    public class OptionalJsonSerializerFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type t) => t.IsGenericType && typeof(Optional<>) == t.GetGenericTypeDefinition();
        public override JsonConverter CreateConverter(Type t, JsonSerializerOptions options)
        {
            Type convType = typeof(OptionalJsonSerializer<>).MakeGenericType(t.GetGenericArguments()[0]);
            return (JsonConverter)Activator.CreateInstance(convType)!;
        }
    }
}
