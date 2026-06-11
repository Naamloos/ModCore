using ModCore.Common.Discord.Entities.Serializer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities
{
    public static class Optional
    {
        public static OptionalNone None => default;
    }

    public struct OptionalNone
    {
    }

    public struct Optional<T> : IOptional
    {
        public T? Value { get; private set; } = default;
        [MemberNotNullWhen(true, nameof(Value))]
        public bool HasValue { get; private set; } = false;

        public Optional([AllowNull] T? value)
        {
            this.Value = value;
            this.HasValue = true;
        }

        public Optional()
        {
        }

        public static Optional<T> None => new Optional<T>();
        public static implicit operator Optional<T>(OptionalNone _) => new Optional<T>();
        public static implicit operator Optional<T>(T? value) { return new Optional<T>(value); }
        public static implicit operator T?(Optional<T> value) { return value.HasValue ? value.Value : default; }

        public override string ToString()
        {
            return Value?.ToString() ?? string.Empty;
        }
    }

    public interface IOptional
    {
        bool HasValue { get; }
    }
}
