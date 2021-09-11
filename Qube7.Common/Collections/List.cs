using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Qube7.Collections
{
    /// <summary>
    /// Provides extension methods for the <see cref="IList{T}"/>.
    /// </summary>
    public static class List
    {
        #region Methods

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="IList{T}"/> that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The source list.</param>
        /// <param name="item">The object to locate in the <paramref name="list"/>. The value can be <c>null</c> for reference types.</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="comparer">The equality comparer to use to locate <paramref name="item"/>.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the <paramref name="list"/> that starts at <paramref name="index"/> and contains <paramref name="count"/> number of elements, if found; otherwise, -1.</returns>
        public static int IndexOf<T>(this IList<T> list, T item, int index, int count, IEqualityComparer<T> comparer)
        {
            Requires.NotNull(list, nameof(list));

            if (index < 0 || index > list.Count)
            {
                throw Error.ArgumentOutOfRange(Strings.IndexRangeList, nameof(index));
            }

            if (count < 0 || count > list.Count - index)
            {
                throw Error.ArgumentOutOfRange(Strings.CountRangeList, nameof(count));
            }

            comparer = comparer ?? EqualityComparer<T>.Default;

            int num = index + count;

            for (int i = index; i < num; i++)
            {
                if (comparer.Equals(list[i], item))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="IList{T}"/> that extends from the specified index to the last element.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The source list.</param>
        /// <param name="item">The object to locate in the <paramref name="list"/>. The value can be <c>null</c> for reference types.</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="comparer">The equality comparer to use to locate <paramref name="item"/>.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the <paramref name="list"/> that extends from <paramref name="index"/> to the last element, if found; otherwise, -1.</returns>
        public static int IndexOf<T>(this IList<T> list, T item, int index, IEqualityComparer<T> comparer)
        {
            Requires.NotNull(list, nameof(list));

            return IndexOf(list, item, index, list.Count - index, comparer);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="IList{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The source list.</param>
        /// <param name="item">The object to locate in the <paramref name="list"/>. The value can be <c>null</c> for reference types.</param>
        /// <param name="comparer">The equality comparer to use to locate <paramref name="item"/>.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item"/> within the entire <paramref name="list"/>, if found; otherwise, -1.</returns>
        public static int IndexOf<T>(this IList<T> list, T item, IEqualityComparer<T> comparer)
        {
            return IndexOf(list, item, 0, comparer);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="IList{T}"/> that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The source list.</param>
        /// <param name="item">The object to locate in the <paramref name="list"/>. The value can be <c>null</c> for reference types.</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the <paramref name="list"/> that starts at <paramref name="index"/> and contains <paramref name="count"/> number of elements, if found; otherwise, -1.</returns>
        public static int IndexOf<T>(this IList<T> list, T item, int index, int count)
        {
            return IndexOf(list, item, index, count, null);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="IList{T}"/> that extends from the specified index to the last element.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The source list.</param>
        /// <param name="item">The object to locate in the <paramref name="list"/>. The value can be <c>null</c> for reference types.</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the <paramref name="list"/> that extends from <paramref name="index"/> to the last element, if found; otherwise, -1.</returns>
        public static int IndexOf<T>(this IList<T> list, T item, int index)
        {
            return IndexOf(list, item, index, null);
        }

        /// <summary>
        /// Returns a read-only wrapper around the <see cref="IList{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to wrap.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> wrapper.</returns>
        public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> list)
        {
            return list as ReadOnlyCollection<T> ?? new ReadOnlyCollection<T>(list);
        }

        #endregion
    }
}
