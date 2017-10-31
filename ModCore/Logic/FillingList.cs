using System;
using System.Collections;
using System.Collections.Generic;

namespace ModCore.Logic
{
    /// <inheritdoc />
    /// <summary>
    /// A collection that reads from an <see cref="T:System.Collections.IEnumerable" />, minimizing the amount of reads and caching when possible.
    /// Instances of this list are not thread-safe.
    /// </summary>
    /// <typeparam name="T">the collection type</typeparam>
    public class FillingList<T> : IReadOnlyList<T>
    {
        private readonly IEnumerator<T> _enumerator;
        private readonly T[] _entries;
        private int _lastPresent;

        public int Count { get; }

        /// <inheritdoc />
        /// <summary>
        /// Get an index from this collection. If it's not present, reads all entries up to the index and caches them.
        /// </summary>
        /// <param name="i">the index to get.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">if the index is out of bounds of the collection, or past the end of the enumerable</exception>
        public T this[int i]
        {
            get
            {
                if (i > Count) throw new ArgumentOutOfRangeException(nameof(i), "out of bounds");
                if (i == _lastPresent)
                {
                    if (!_enumerator.MoveNext()) throw new ArgumentOutOfRangeException(nameof(i), "tried to read past end");
                    _lastPresent++;
                    return _entries[i] = _enumerator.Current;
                }
                return _lastPresent >= i ? _entries[i] : ReadUntil(i);
            }
        }

        private T ReadUntil(int index)
        {
            for (var i = _lastPresent; i < index; i++)
            {
                if (!_enumerator.MoveNext()) throw new ArgumentOutOfRangeException(nameof(index), "tried to read past end");
                _entries[i] = _enumerator.Current;
            }
            _lastPresent = index+1;
            return _entries[index];
        }

        /// <summary>
        /// Creates a new autofilling list.
        /// </summary>
        /// <param name="enumerable">The enumerable to read and cache entries from</param>
        /// <param name="count">The max amount of entries to be read from the enumerable</param>
        public FillingList(IEnumerable<T> enumerable, int count)
        {
            this._enumerator = enumerable.GetEnumerator();
            this._entries = new T[count];
            Count = count;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < _lastPresent; i++)
            {
                yield return _entries[i];
            }
            for (var i = _lastPresent; i < Count; i++)
            {
                if (!_enumerator.MoveNext()) throw new ArgumentOutOfRangeException(nameof(i), "tried to read past end");
                _lastPresent++;
                yield return _entries[i] = _enumerator.Current;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}