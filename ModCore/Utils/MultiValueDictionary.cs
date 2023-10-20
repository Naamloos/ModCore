// https://gist.github.com/njonsson/338938
namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a generic collection of keys paired with zero or more values
    /// each.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the
    /// <see cref="MultiValueDictionary{TKey,TValues}"/>.</typeparam>
    /// <typeparam name="TValues">The type of the value list elements in the
    /// <see cref="MultiValueDictionary{TKey,TValues}"/>.</typeparam>
    public class MultiValueDictionary<TKey, TValues> :
                 Dictionary<TKey, IEnumerable<TValues>>,
                 IMultiValueDictionary<TKey, TValues>
    {
        /// <summary>
        /// Gets or sets the values associated with the specified
        /// <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The values associated with the specified
        /// <paramref name="key"/>. If the specified <paramref name="key"/> is
        /// not found, a get operation returns an empty list, and a set
        /// operation creates a new element with the specified
        /// <paramref name="key"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is
        /// (<c>Nothing</c> in Visual Basic), or the list of values is a null
        /// reference in a set operation.</exception>
        public new IEnumerable<TValues> this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out var values)) return values;
                return Array.Empty<TValues>();
            }

            set
            {
                if (value == null) throw new ArgumentNullException();
                base[key] = value;
            }
        }

        /// <summary>
        /// Adds the specified <paramref name="key" /> and
        /// <paramref name="value"/> to the
        /// <see cref="MultiValueDictionary{TKey,TValues}"/>. If
        /// <paramref name="key"/> already exists in the
        /// <see cref="MultiValueDictionary{TKey,TValues}"/> then
        /// <paramref name="value"/> is added to the end of the list of values
        /// for <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can
        /// be a null reference (<c>Nothing</c> in Visual Basic).</param>
        /// <returns><c>true</c> if <paramref name="key"/> and
        /// <paramref name="value"/> are not both already present; otherwise,
        /// <c>false</c>. This method returns <c>true</c> if
        /// <paramref name="value"/> is added to the
        /// <see cref="MultiValueDictionary{TKey,TValues}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a
        /// null reference (<c>Nothing</c> in Visual Basic).</exception>
        public bool AddValue(TKey key, TValues value)
        {
            if (TryGetValue(key, out var existingValues))
            {
                var valuesList = new List<TValues>(existingValues);
                if (valuesList.Contains(value)) return false;
                valuesList.Add(value);
                this[key] = valuesList.ToArray();
            }
            else
            {
                Add(key, new[] { value });
            }
            return true;
        }

        /// <summary>
        /// Adds the specified <paramref name="key" /> and
        /// <paramref name="values"/> to the
        /// <see cref="MultiValueDictionary{TKey,TValues}"/>. If
        /// <paramref name="key"/> already exists in the
        /// <see cref="MultiValueDictionary{TKey,TValues}"/> then
        /// <paramref name="values"/> are added to the end of the list of values
        /// for <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="values">The values of the element to add. The values
        /// can be null references (<c>Nothing</c> in Visual Basic).</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a
        /// null reference (<c>Nothing</c> in Visual Basic).</exception>
        public void AddValues(TKey key, IEnumerable<TValues> values)
        {
            if (!TryGetValue(key, out var existingValues))
            {
                Add(key, new List<TValues>(values).ToArray());
                return;
            }

            var valuesList = new List<TValues>(existingValues);
            foreach (var v in values)
            {
                if (valuesList.Contains(v)) continue;
                valuesList.Add(v);
            }
            this[key] = valuesList.ToArray();
        }

        /// <summary>
        /// Removes the values with the specified <paramref name="key"/> from
        /// the <see cref="MultiValueDictionary{TKey,TValues}"/>.
        /// </summary>
        /// <param name="key">The key of the values to remove.</param>
        /// <returns><c>true</c> if the values are successfully found and
        /// removed; otherwise, <c>false</c>. This method returns <c>false</c>
        /// if <paramref name="key"/> is not found in the
        /// <see cref="MultiValueDictionary{TKey,TValues}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a
        /// null reference (<c>Nothing</c> in Visual Basic).</exception>
        public bool RemoveKey(TKey key)
        {
            return Remove(key);
        }

        /// <summary>
        /// Removes the values with the specified <paramref name="keys"/> from
        /// the <see cref="MultiValueDictionary{TKey,TValues}"/>.
        /// </summary>
        /// <param name="keys">The keys of the values to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/>
        /// is a null reference (<c>Nothing</c> in Visual Basic) or any of its
        /// elements are null references.</exception>
        public void RemoveKeys(IEnumerable<TKey> keys)
        {
            if (keys == null) throw new ArgumentNullException("keys");
            foreach (var k in keys) RemoveKey(k);
        }

        /// <summary>
        /// Removes the specified <param name="value"/> with the specified
        /// <paramref name="key"/> from the
        /// <see cref="MultiValueDictionary{TKey,TValues}"/>.
        /// </summary>
        /// <param name="key">The key of the values to remove.</param>
        /// <param name="value">The value to remove by
        /// <paramref name="key"/>. The value can be a null reference
        /// (<c>Nothing</c> in Visual Basic).</param>
        /// <returns><c>true</c> if <paramref name="value"/> is successfully
        /// found for <paramref name="key"/> and removed; otherwise,
        /// <c>false</c>. This method returns <c>false</c> if
        /// <paramref name="key"/> is not found in the
        /// <see cref="MultiValueDictionary{TKey,TValues}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a
        /// null reference (<c>Nothing</c> in Visual Basic).</exception>
        public bool RemoveValue(TKey key, TValues value)
        {
            if (!TryGetValue(key, out var existingValues)) return false;

            var valuesList = new List<TValues>(existingValues);
            if (!valuesList.Remove(value)) return false;

            this[key] = valuesList.ToArray();
            return true;
        }

        /// <summary>
        /// Removes the specified <param name="values"/> with the specified
        /// <paramref name="key"/> from the
        /// <see cref="MultiValueDictionary{TKey,TValues}"/>.
        /// </summary>
        /// <param name="key">The key of the values to remove.</param>
        /// <param name="values">The values to remove by
        /// <paramref name="key"/>. The values can be null references
        /// (<c>Nothing</c> in Visual Basic).</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a
        /// null reference (<c>Nothing</c> in Visual Basic).</exception>
        public void RemoveValues(TKey key, IEnumerable<TValues> values)
        {
            if (!TryGetValue(key, out var existingValues)) return;

            var valuesList = new List<TValues>(existingValues);
            foreach (var v in values) valuesList.Remove(v);
            this[key] = valuesList.ToArray();
        }
    }
}
