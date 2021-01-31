using System;

namespace Qube7.Collections
{
    /// <summary>
    /// Provides helpers for the arrays.
    /// </summary>
    public static class ArrayHelper
    {
        #region Methods

        /// <summary>
        /// Returns a new array in which a specified element is inserted at the end of the specified array.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array.</typeparam>
        /// <param name="array">The one-dimensional, zero-based array in which to insert the element.</param>
        /// <param name="value">The object to insert.</param>
        /// <returns>A new array with <paramref name="value"/> inserted at the end of the <paramref name="array"/>.</returns>
        public static T[] Append<T>(T[] array, T value)
        {
            Requires.NotNull(array, nameof(array));

            if (array.Length > 0)
            {
                T[] buffer = new T[array.Length + 1];

                Array.Copy(array, 0, buffer, 0, array.Length);

                buffer[array.Length] = value;

                return buffer;
            }

            return Params.Array(value);
        }

        /// <summary>
        /// Returns a new array in which a specified element is inserted at the specified index in the specified array.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array.</typeparam>
        /// <param name="array">The one-dimensional, zero-based array in which to insert the element.</param>
        /// <param name="index">The zero-based index at which to insert the element.</param>
        /// <param name="value">The object to insert.</param>
        /// <returns>A new array with <paramref name="value"/> inserted at the <paramref name="index"/> position in the <paramref name="array"/>.</returns>
        public static T[] Insert<T>(T[] array, int index, T value)
        {
            Requires.NotNull(array, nameof(array));
            Requires.NonNegative(index, nameof(index));

            if (index > array.Length)
            {
                throw Error.Argument(Strings.IndexGreaterLength);
            }

            if (array.Length > 0)
            {
                T[] buffer = new T[array.Length + 1];

                if (index > 0)
                {
                    Array.Copy(array, 0, buffer, 0, index);
                }

                buffer[index] = value;

                if (index < array.Length)
                {
                    Array.Copy(array, index, buffer, index + 1, array.Length - index);
                }

                return buffer;
            }

            return Params.Array(value);
        }

        /// <summary>
        /// Returns a new array that contains the concatenated elements of the two specified input arrays.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the arrays.</typeparam>
        /// <param name="first">The first, one-dimensional, zero-based array to concatenate.</param>
        /// <param name="second">The one-dimensional, zero-based array to concatenate to the first array.</param>
        /// <returns>A new array that contains the concatenated elements of the two input arrays.</returns>
        public static T[] Concat<T>(T[] first, T[] second)
        {
            Requires.NotNull(first, nameof(first));
            Requires.NotNull(second, nameof(second));

            if (first.Length + second.Length > 0)
            {
                T[] buffer = new T[first.Length + second.Length];

                if (first.Length > 0)
                {
                    Array.Copy(first, 0, buffer, 0, first.Length);
                }

                if (second.Length > 0)
                {
                    Array.Copy(second, 0, buffer, first.Length, second.Length);
                }

                return buffer;
            }

            return Array.Empty<T>();
        }

        /// <summary>
        /// Returns a new array in which the element at the specified index in the specified array is removed.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array.</typeparam>
        /// <param name="array">The one-dimensional, zero-based array in which to remove the element.</param>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <returns>A new array without the element at the <paramref name="index"/> position in the <paramref name="array"/>.</returns>
        public static T[] RemoveAt<T>(T[] array, int index)
        {
            Requires.NotNull(array, nameof(array));
            Requires.NonNegative(index, nameof(index));

            if (index >= array.Length)
            {
                throw Error.Argument(Strings.IndexGreaterEqualLength);
            }

            if (array.Length > 1)
            {
                T[] buffer = new T[array.Length - 1];

                if (index > 0)
                {
                    Array.Copy(array, 0, buffer, 0, index);
                }

                if (index < array.Length - 1)
                {
                    Array.Copy(array, index + 1, buffer, index, array.Length - index - 1);
                }

                return buffer;
            }

            return Array.Empty<T>();
        }

        /// <summary>
        /// Returns a new array in which the specified number of elements starting at the specified index in the specified array are removed.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array.</typeparam>
        /// <param name="array">The one-dimensional, zero-based array in which to remove the range of elements.</param>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <returns>A new array without the range of <paramref name="count"/> elements starting at the <paramref name="index"/> position in the <paramref name="array"/>.</returns>
        public static T[] RemoveRange<T>(T[] array, int index, int count)
        {
            Requires.NotNull(array, nameof(array));
            Requires.NonNegative(index, nameof(index));
            Requires.NonNegative(count, nameof(count));

            if (index >= array.Length)
            {
                throw Error.Argument(Strings.IndexGreaterEqualLength);
            }

            if (count > array.Length - index)
            {
                throw Error.Argument(Strings.OffsetLengthInvalid);
            }

            if (array.Length > count)
            {
                T[] buffer = new T[array.Length - count];

                if (index > 0)
                {
                    Array.Copy(array, 0, buffer, 0, index);
                }

                if (index < array.Length - count)
                {
                    Array.Copy(array, index + count, buffer, index, array.Length - index - count);
                }

                return buffer;
            }

            return Array.Empty<T>();
        }

        #endregion
    }
}
