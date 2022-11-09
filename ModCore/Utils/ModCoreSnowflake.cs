using System;
using DSharpPlus;

namespace ModCore.Utils
{
    public abstract class ModCoreSnowflake
    {
        /// <summary>Gets the ID of this object.</summary>
        public ulong Id { get; protected set; }

        /// <summary>Gets the date and time this object was created.</summary>
        public DateTimeOffset CreationTimestamp => new DateTimeOffset(2015, 1, 1, 0, 0, 0, TimeSpan.Zero).AddMilliseconds(this.Id >> 22);

        /// <summary>Gets the client instance this object is tied to.</summary>
        internal BaseDiscordClient Discord { get; set; }

        internal ModCoreSnowflake()
        {
        }
    }
}