using ModCore.Common.Discord.Entities.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities
{
    public struct Optional<T>
    {
        public T Value { get; private set; } = default(T);
        public bool HasValue { get; private set; } = false;

        public Optional(T value)
        {
            this.Value = value;
            this.HasValue = true;
        }

        public Optional()
        {
        }

        public static Optional<T> None => new Optional<T>();
        public static implicit operator Optional<T>(T value) { return new Optional<T>(value); }
        public static implicit operator T(Optional<T> value) { return value.HasValue? value.Value : default(T); }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
