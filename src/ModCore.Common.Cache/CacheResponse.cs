using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Cache
{
    public struct CacheResponse<T>
    {
        public bool Success { get; private set; }
        public T? Value { get; private set; }
        public CacheLayer CacheLayer { get; private set; }

        public CacheResponse(bool success, T? value, CacheLayer cacheLayer)
        {
            Success = success;
            Value = value;
            CacheLayer = cacheLayer;
        }
    }

    public enum CacheLayer
    {
        None,
        L1,
        L2
    }
}
