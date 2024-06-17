using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Cache
{
    public struct CacheResponse<T>
    {
        public bool Success { get; set; }
        public T? Value { get; set; }

        public CacheResponse(bool success, T? value)
        {
            Success = success;
            Value = value;
        }
    }
}
