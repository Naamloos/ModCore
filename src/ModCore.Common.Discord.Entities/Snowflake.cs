using ModCore.Common.Discord.Entities.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities
{
    [JsonConverter(typeof(SnowflakeJsonSerializer))]
    public record Snowflake
    {
        const ulong DISCORD_EPOCH = 1420070400000;

        public DateTimeOffset Timestamp => DateTimeOffset.FromUnixTimeMilliseconds((long)((snowflake >> 22) + DISCORD_EPOCH));
        public ulong Value => snowflake;

        private ulong snowflake = 0;

        public Snowflake(ulong value)
        {
            snowflake = value;
        }

        public Snowflake(DateTimeOffset Timestamp)
        {
            snowflake = ((ulong)Timestamp.ToUnixTimeMilliseconds() - DISCORD_EPOCH) << 22;
        }

        public static implicit operator ulong(Snowflake snowflake) { return snowflake; }
        public static implicit operator Snowflake(ulong value) { return new Snowflake(value); }
        public static implicit operator Snowflake(DateTimeOffset timestamp) { return new Snowflake(timestamp); }
        public static implicit operator DateTimeOffset(Snowflake snowflake) { return snowflake.Timestamp; }

        public override string ToString()
        {
            return snowflake.ToString();
        }
    }
}
