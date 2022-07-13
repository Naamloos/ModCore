// https://gist.github.com/njonsson/338938
namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a generic collection of keys paired with zero or more values
    /// each.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the
    /// <see cref="IMultiValueDictionary{TKey,TValues}"/>.</typeparam>
    /// <typeparam name="TValues">The type of the value list elements in the
    /// <see cref="IMultiValueDictionary{TKey,TValues}"/>.</typeparam>
    public interface IMultiValueDictionary<TKey, TValues> :
                     IDictionary<TKey, IEnumerable<TValues>>
    {
        /// <summary>
        /// Adds the specified <paramref name="key" /> and
        /// <paramref name="value"/> to the
        /// <see cref="IMultiValueDictionary{TKey,TValues}"/>. If
        /// <paramref name="key"/> already exists in the
        /// <see cref="IMultiValueDictionary{TKey,TValues}"/> then
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
        /// <see cref="IMultiValueDictionary{TKey,TValues}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a
        /// null reference (<c>Nothing</c> in Visual Basic).</exception>
        bool AddValue(TKey key, TValues value);

        /// <summary>
        /// Adds the specified <paramref name="key" /> and
        /// <paramref name="values"/> to the
        /// <see cref="IMultiValueDictionary{TKey,TValues}"/>. If
        /// <paramref name="key"/> already exists in the
        /// <see cref="IMultiValueDictionary{TKey,TValues}"/> then
        /// <paramref name="values"/> are added to the end of the list of values
        /// for <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="values">The values of the element to add. The values
        /// can be null references (<c>Nothing</c> in Visual Basic).</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a
        /// null reference (<c>Nothing</c> in Visual Basic).</exception>
        void AddValues(TKey key, IEnumerable<TValues> values);

        /// <summary>
        /// Removes the values with the specified <paramref name="key"/> from
        /// the <see cref="IMultiValueDictionary{TKey,TValues}"/>.
        /// </summary>
        /// <param name="key">The key of the values to remove.</param>
        /// <returns><c>true</c> if the values are successfully found and
        /// removed; otherwise, <c>false</c>. This method returns <c>false</c>
        /// if <paramref name="key"/> is not found in the
        /// <see cref="IMultiValueDictionary{TKey,TValues}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a
        /// null reference (<c>Nothing</c> in Visual Basic).</exception>
        bool RemoveKey(TKey key);

        /// <summary>
        /// Removes the values with the specified <paramref name="keys"/> from
        /// the <see cref="IMultiValueDictionary{TKey,TValues}"/>.
        /// </summary>
        /// <param name="keys">The keys of the values to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/>
        /// is a null reference (<c>Nothing</c> in Visual Basic) or any of its
        /// elements are null references.</exception>
        void RemoveKeys(IEnumerable<TKey> keys);

        /// <summary>
        /// Removes the specified <param name="value"/> with the specified
        /// <paramref name="key"/> from the
        /// <see cref="IMultiValueDictionary{TKey,TValues}"/>.
        /// </summary>
        /// <param name="key">The key of the values to remove.</param>
        /// <param name="value">The value to remove by
        /// <paramref name="key"/>. The value can be a null reference
        /// (<c>Nothing</c> in Visual Basic).</param>
        /// <returns><c>true</c> if <paramref name="value"/> is successfully
        /// found for <paramref name="key"/> and removed; otherwise,
        /// <c>false</c>. This method returns <c>false</c> if
        /// <paramref name="key"/> is not found in the
        /// <see cref="IMultiValueDictionary{TKey,TValues}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a
        /// null reference (<c>Nothing</c> in Visual Basic).</exception>
        bool RemoveValue(TKey key, TValues value);

        /// <summary>
        /// Removes the specified <param name="values"/> with the specified
        /// <paramref name="key"/> from the
        /// <see cref="IMultiValueDictionary{TKey,TValues}"/>.
        /// </summary>
        /// <param name="key">The key of the values to remove.</param>
        /// <param name="values">The values to remove by
        /// <paramref name="key"/>. The values can be null references
        /// (<c>Nothing</c> in Visual Basic).</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a
        /// null reference (<c>Nothing</c> in Visual Basic).</exception>
        void RemoveValues(TKey key, IEnumerable<TValues> values);
    }
}