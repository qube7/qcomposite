using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Qube7.Collections
{
    /// <summary>
    /// Provides helpers for the <see cref="IDictionary{TKey, TValue}"/>.
    /// </summary>
    public static class Dictionary
    {
        #region Methods

        /// <summary>
        /// Returns a read-only wrapper around the specified dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary to wrap.</param>
        /// <returns>A <see cref="ReadOnlyDictionary{TKey, TValue}"/> wrapper.</returns>
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return dictionary as ReadOnlyDictionary<TKey, TValue> ?? new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }

        /// <summary>
        /// Gets the value associated with the specified key, cast to the specified type.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <typeparam name="TResult">The type to cast the value to.</typeparam>
        /// <param name="dictionary">The source dictionary.</param>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, cast to the specified type, if the key is found and the value is of the <typeparamref name="TResult"/> type; otherwise, the default value for the type of the <paramref name="value"/> parameter.</param>
        /// <returns><c>true</c> if the <paramref name="dictionary"/> contains an element with the specified key and the value is of the <typeparamref name="TResult"/> type; otherwise, <c>false</c>.</returns>
        public static bool TryGetValue<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dictionary, TKey key, out TResult value) where TResult : class
        {
            Requires.NotNull(dictionary, nameof(dictionary));

            if (dictionary.TryGetValue(key, out TValue current) && current is TResult)
            {
                value = current as TResult;

                return true;
            }

            value = default;

            return false;
        }

        #endregion
    }
}
