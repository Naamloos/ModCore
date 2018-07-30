using System.Collections.Generic;

namespace ModCore.Logic
{
    public class MultiValueDictionaryMetadata<TKey, TMetadata, TValue>
    {
        private readonly Dictionary<TKey, (TMetadata meta, List<TValue> values)> _container 
            = new Dictionary<TKey, (TMetadata, List<TValue>)>();

        /// <summary>
        /// Adds a value to this dictionary. Adds metadata if it not already present.
        /// </summary>
        /// <param name="k">The dictionary key</param>
        /// <param name="m">The extra metadata</param>
        /// <param name="v">The dictionary value</param>
        public void Add(TKey k, TMetadata m, TValue v)
        {
            if (_container.TryGetValue(k, out var entry))
                entry.values.Add(v);
            else
                _container.Add(k, (m, new List<TValue> {v}));
        }

    }
}