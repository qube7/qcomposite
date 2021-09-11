using System;
using System.Collections.Generic;
using System.Linq;

namespace Qube7
{
    /// <summary>
    /// Provides helper and extension methods for the <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class Iterable
    {
        #region Methods

        /// <summary>
        /// Generates a sequence of elements that are obtained by iterating through the values, while a specified condition evaluates to <c>true</c>, starting from the specified value, advancing the iteration by invoking the specified function.
        /// </summary>
        /// <typeparam name="TResult">The type of the elements in the result sequence.</typeparam>
        /// <param name="start">The first element in the iteration.</param>
        /// <param name="next">The function that advances the iteration to the next element.</param>
        /// <param name="predicate">The function to test each element for a condition.</param>
        /// <returns>A sequence that contains the resulting elements.</returns>
        public static IEnumerable<TResult> Iterate<TResult>(TResult start, Func<TResult, TResult> next, Func<TResult, bool> predicate)
        {
            Requires.NotNull(next, nameof(next));
            Requires.NotNull(predicate, nameof(predicate));

            return IteratePrivate(start, next, predicate);
        }

        /// <summary>
        /// Generates a sequence of elements that are obtained by iterating through the values, while a specified condition evaluates to <c>true</c>, starting from the specified value, advancing the iteration by invoking the specified function.
        /// </summary>
        /// <typeparam name="TResult">The type of the elements in the result sequence.</typeparam>
        /// <param name="start">The first element in the iteration.</param>
        /// <param name="next">The function that advances the iteration to the next element.</param>
        /// <param name="predicate">The function to test each element for a condition.</param>
        /// <returns>A sequence that contains the resulting elements.</returns>
        private static IEnumerable<TResult> IteratePrivate<TResult>(TResult start, Func<TResult, TResult> next, Func<TResult, bool> predicate)
        {
            TResult current = start;

            while (predicate(current))
            {
                yield return current;

                current = next(current);
            }
        }

        /// <summary>
        /// Generates a sequence of elements that are obtained by iterating through the values, while the current value is not <c>null</c>, starting from the specified value, advancing the iteration by invoking the specified function.
        /// </summary>
        /// <typeparam name="TResult">The type of the elements in the result sequence.</typeparam>
        /// <param name="start">The first element in the iteration.</param>
        /// <param name="next">The function that advances the iteration to the next element.</param>
        /// <returns>A sequence that contains the resulting elements.</returns>
        public static IEnumerable<TResult> Iterate<TResult>(TResult start, Func<TResult, TResult> next) where TResult : class
        {
            Requires.NotNull(next, nameof(next));

            return IteratePrivate(start, next);
        }

        /// <summary>
        /// Generates a sequence of elements that are obtained by iterating through the values, while the current value is not <c>null</c>, starting from the specified value, advancing the iteration by invoking the specified function.
        /// </summary>
        /// <typeparam name="TResult">The type of the elements in the result sequence.</typeparam>
        /// <param name="start">The first element in the iteration.</param>
        /// <param name="next">The function that advances the iteration to the next element.</param>
        /// <returns>A sequence that contains the resulting elements.</returns>
        private static IEnumerable<TResult> IteratePrivate<TResult>(TResult start, Func<TResult, TResult> next) where TResult : class
        {
            TResult current = start;

            while (current != null)
            {
                yield return current;

                current = next(current);
            }
        }

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
