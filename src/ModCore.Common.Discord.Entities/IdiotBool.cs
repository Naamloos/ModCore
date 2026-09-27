using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ModCore.Common.Discord.Entities.Serializer;

namespace ModCore.Common.Discord.Entities
{
    [JsonConverter(typeof(IdiotBoolSerializer))]
    public struct IdiotBool
    {
        public bool Value = false;

        public IdiotBool(bool value)
        {
            Value = value;
        }

        public static implicit operator IdiotBool(bool value)
        {
            return new IdiotBool(value);
        }

        public static implicit operator bool(IdiotBool value)
        {
            return value.Value;
        }
    }
}
