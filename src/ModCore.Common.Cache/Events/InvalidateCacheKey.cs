using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Cache.Events
{
    public record InvalidateCacheKey
    {
        public string Key { get; init; }
        public InvalidateCacheKey(string key)
        {
            Key = key;
        }
    }
}
