using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Qube7.Collections
{
    /// <summary>
    /// Provides extension methods for the <see cref="IDictionary{TKey, TValue}"/>.
    /// </summary>
    public static class Dictionary
    {
        #region Methods

        /// <summary>
        /// Returns a read-only wrapper around the <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary to wrap.</param>
        /// <returns>A <see cref="ReadOnlyDictionary{TKey, TValue}"/> wrapper.</returns>
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return dictionary as ReadOnlyDictionary<TKey, TValue> ?? new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }

        #endregion
    }
}
