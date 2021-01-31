using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qube7
{
    /// <summary>
    /// Provides the helper extension methods.
    /// </summary>
    public static class Extensions
    {
        #region Methods

        /// <summary>
        /// Performs the specified action on each element of the specified sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence on whose elements the action is to be performed.</param>
        /// <param name="action">The <see cref="Action{T}"/> to perform on each element of <paramref name="source"/>.</param>
        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            Requires.NotNull(source, nameof(source));
            Requires.NotNull(action, nameof(action));

            foreach (TSource item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to check for emptiness.</param>
        /// <returns><c>true</c> if the source sequence contains any elements; otherwise, <c>false</c>.</returns>
        /// <remarks>To determine whether the input sequence contains any elements, the <see cref="ICollection.Count"/> is used, if available.</remarks>
        public static bool FastAny<TSource>(this IEnumerable<TSource> source)
        {
            ICollection collection = source as ICollection;

            return collection != null ? collection.Count > 0 : Enumerable.Any(source);
        }

        /// <summary>
        /// Produces the set difference of the sequence and an array by using the default equality comparer to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequence and array.</typeparam>
        /// <param name="first">An <see cref="IEnumerable{T}"/> whose elements that are not also in <paramref name="second"/> array will be returned.</param>
        /// <param name="second">An array whose elements that also occur in the <paramref name="first"/> sequence will cause those elements to be removed from the returned sequence.</param>
        /// <returns>A sequence that contains the set difference of the elements of the sequence and an array.</returns>
        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, params TSource[] second)
        {
            return Enumerable.Except(first, second ?? Enumerable.Empty<TSource>());
        }

        /// <summary>
        /// Produces the set intersection of the sequence and an array by using the default equality comparer to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequence and array.</typeparam>
        /// <param name="first">An <see cref="IEnumerable{T}"/> whose distinct elements that also appear in <paramref name="second"/> array will be returned.</param>
        /// <param name="second">An array whose distinct elements that also appear in the <paramref name="first"/> sequence will be returned.</param>
        /// <returns>A sequence that contains the elements that form the set intersection of the sequence and an array.</returns>
        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, params TSource[] second)
        {
            return Enumerable.Intersect(first, second ?? Enumerable.Empty<TSource>());
        }

        /// <summary>
        /// Produces the set union of the sequence and an array by using the default equality comparer to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequence and array.</typeparam>
        /// <param name="first">An <see cref="IEnumerable{T}"/> whose distinct elements form the first set for the union.</param>
        /// <param name="second">An array whose distinct elements form the second set for the union.</param>
        /// <returns>A sequence that contains the elements from both input sequence and array, excluding duplicates.</returns>
        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, params TSource[] second)
        {
            return Enumerable.Union(first, second ?? Enumerable.Empty<TSource>());
        }

        #endregion
    }
}
