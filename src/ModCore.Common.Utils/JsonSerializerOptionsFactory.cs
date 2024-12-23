using ModCore.Common.Discord.Entities;
using ModCore.Common.Discord.Entities.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace ModCore.Common.Utils
{
    public static class JsonSerializerOptionsFactory
    {
        public static JsonSerializerOptions GetOptions()
        {
            var options = new JsonSerializerOptions()
            {
                Converters = { new OptionalJsonSerializerFactory() },
                TypeInfoResolver = new DefaultJsonTypeInfoResolver
                {
                    Modifiers = { HandleOptional }
                },
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };

            return options;
        }

        private static void HandleOptional(JsonTypeInfo typeInfo)
        {
            foreach (var propertyInfo in typeInfo.Properties)
            {
                if (!propertyInfo.PropertyType.IsGenericType || propertyInfo.PropertyType.GetGenericTypeDefinition() != typeof(Optional<>))
                {
                    continue;
                }

                propertyInfo.ShouldSerialize = (_, property) => ((IOptional)property)?.HasValue ?? false;
            }
        }
    }
}
