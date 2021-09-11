using System.Threading;

namespace Qube7.Threading
{
    /// <summary>
    /// Provides helper methods for performing thread-safe memory operations.
    /// </summary>
    public static class ValueSafe
    {
        #region Methods

        /// <summary>
        /// Determines whether the value of the specified variable is equal to zero.
        /// </summary>
        /// <param name="location">The variable whose value is to be tested.</param>
        /// <returns><c>true</c> if the value is equal to zero; otherwise, <c>false</c>.</returns>
        public static bool Equals0(ref int location)
        {
            return Volatile.Read(ref location) == 0;
        }

        /// <summary>
        /// Determines whether the value of the specified variable is equal to one.
        /// </summary>
        /// <param name="location">The variable whose value is to be tested.</param>
        /// <returns><c>true</c> if the value is equal to one; otherwise, <c>false</c>.</returns>
        public static bool Equals1(ref int location)
        {
            return Volatile.Read(ref location) == 1;
        }

        /// <summary>
        /// Determines whether the value of the specified variable is equal to two.
        /// </summary>
        /// <param name="location">The variable whose value is to be tested.</param>
        /// <returns><c>true</c> if the value is equal to two; otherwise, <c>false</c>.</returns>
        public static bool Equals2(ref int location)
        {
            return Volatile.Read(ref location) == 2;
        }

        /// <summary>
        /// Determines whether the value of the specified variable is equal to three.
        /// </summary>
        /// <param name="location">The variable whose value is to be tested.</param>
        /// <returns><c>true</c> if the value is equal to three; otherwise, <c>false</c>.</returns>
        public static bool Equals3(ref int location)
        {
            return Volatile.Read(ref location) == 3;
        }

        /// <summary>
        /// Increments a specified variable if its value is equal to zero.
        /// </summary>
        /// <param name="location">The variable whose value is to be incremented.</param>
        /// <returns><c>true</c> if the value has been incremented; otherwise, <c>false</c>.</returns>
        public static bool Increment0(ref int location)
        {
            return Interlocked.CompareExchange(ref location, 1, 0) == 0;
        }

        /// <summary>
        /// Increments a specified variable if its value is equal to one.
        /// </summary>
        /// <param name="location">The variable whose value is to be incremented.</param>
        /// <returns><c>true</c> if the value has been incremented; otherwise, <c>false</c>.</returns>
        public static bool Increment1(ref int location)
        {
            return Interlocked.CompareExchange(ref location, 2, 1) == 1;
        }

        /// <summary>
        /// Increments a specified variable if its value is equal to two.
        /// </summary>
        /// <param name="location">The variable whose value is to be incremented.</param>
        /// <returns><c>true</c> if the value has been incremented; otherwise, <c>false</c>.</returns>
        public static bool Increment2(ref int location)
        {
            return Interlocked.CompareExchange(ref location, 3, 2) == 2;
        }

        /// <summary>
        /// Decrements a specified variable if its value is equal to one.
        /// </summary>
        /// <param name="location">The variable whose value is to be decremented.</param>
        /// <returns><c>true</c> if the value has been decremented; otherwise, <c>false</c>.</returns>
        public static bool Decrement1(ref int location)
        {
            return Interlocked.CompareExchange(ref location, 0, 1) == 1;
        }

        /// <summary>
        /// Decrements a specified variable if its value is equal to two.
        /// </summary>
        /// <param name="location">The variable whose value is to be decremented.</param>
        /// <returns><c>true</c> if the value has been decremented; otherwise, <c>false</c>.</returns>
        public static bool Decrement2(ref int location)
        {
            return Interlocked.CompareExchange(ref location, 1, 2) == 2;
        }

        /// <summary>
        /// Decrements a specified variable if its value is equal to three.
        /// </summary>
        /// <param name="location">The variable whose value is to be decremented.</param>
        /// <returns><c>true</c> if the value has been decremented; otherwise, <c>false</c>.</returns>
        public static bool Decrement3(ref int location)
        {
            return Interlocked.CompareExchange(ref location, 2, 3) == 3;
        }

        /// <summary>
        /// Sets a specified variable to a specified value if its current value is not equal to the comparand.
        /// </summary>
        /// <param name="location">The variable whose value is to be set.</param>
        /// <param name="value">The value to which the <paramref name="location"/> is set.</param>
        /// <param name="comparand">The value that is compared to the value at <paramref name="location"/>.</param>
        /// <returns>The original value in <paramref name="location"/>.</returns>
        public static int NotEqualExchange(ref int location, int value, int comparand)
        {
            int result;

            int current = location;
            do
            {
                result = current;

                current = Interlocked.CompareExchange(ref location, result != comparand ? value : result, result);
            }
            while (current != result);

            return result;
        }

        /// <summary>
        /// Sets a specified variable to a specified value if its current value is not equal to the comparand.
        /// </summary>
        /// <typeparam name="T">The type to be used for <paramref name="location"/>, <paramref name="value"/>, and <paramref name="comparand"/>.</typeparam>
        /// <param name="location">The variable whose value is to be set.</param>
        /// <param name="value">The value to which the <paramref name="location"/> is set.</param>
        /// <param name="comparand">The value that is compared to the value at <paramref name="location"/>.</param>
        /// <returns>The original value in <paramref name="location"/>.</returns>
        public static T NotEqualExchange<T>(ref T location, T value, T comparand) where T : class
        {
            T result;

            T current = location;
            do
            {
                result = current;

                current = Interlocked.CompareExchange(ref location, result != comparand ? value : result, result);
            }
            while (current != result);

            return result;
        }

        /// <summary>
        /// Sets a specified variable to a specified value if its current value is less than or equal to the comparand.
        /// </summary>
        /// <param name="location">The variable whose value is to be set.</param>
        /// <param name="value">The value to which the <paramref name="location"/> is set.</param>
        /// <param name="comparand">The value that is compared to the value at <paramref name="location"/>.</param>
        /// <returns>The original value in <paramref name="location"/>.</returns>
        public static int LessEqualExchange(ref int location, int value, int comparand)
        {
            int result;

            int current = location;
            do
            {
                result = current;

                current = Interlocked.CompareExchange(ref location, result <= comparand ? value : result, result);
            }
            while (current != result);

            return result;
        }

        /// <summary>
        /// Sets a specified variable to a specified value if its current value is less than the comparand.
        /// </summary>
        /// <param name="location">The variable whose value is to be set.</param>
        /// <param name="value">The value to which the <paramref name="location"/> is set.</param>
        /// <param name="comparand">The value that is compared to the value at <paramref name="location"/>.</param>
        /// <returns>The original value in <paramref name="location"/>.</returns>
        public static int LessExchange(ref int location, int value, int comparand)
        {
            int result;

            int current = location;
            do
            {
                result = current;

                current = Interlocked.CompareExchange(ref location, result < comparand ? value : result, result);
            }
            while (current != result);

            return result;
        }

        /// <summary>
        /// Sets a specified variable to a specified value if its current value is greater than or equal to the comparand.
        /// </summary>
        /// <param name="location">The variable whose value is to be set.</param>
        /// <param name="value">The value to which the <paramref name="location"/> is set.</param>
        /// <param name="comparand">The value that is compared to the value at <paramref name="location"/>.</param>
        /// <returns>The original value in <paramref name="location"/>.</returns>
        public static int GreaterEqualExchange(ref int location, int value, int comparand)
        {
            int result;

            int current = location;
            do
            {
                result = current;

                current = Interlocked.CompareExchange(ref location, result >= comparand ? value : result, result);
            }
            while (current != result);

            return result;
        }

        /// <summary>
        /// Sets a specified variable to a specified value if its current value is greater than the comparand.
        /// </summary>
        /// <param name="location">The variable whose value is to be set.</param>
        /// <param name="value">The value to which the <paramref name="location"/> is set.</param>
        /// <param name="comparand">The value that is compared to the value at <paramref name="location"/>.</param>
        /// <returns>The original value in <paramref name="location"/>.</returns>
        public static int GreaterExchange(ref int location, int value, int comparand)
        {
            int result;

            int current = location;
            do
            {
                result = current;

                current = Interlocked.CompareExchange(ref location, result > comparand ? value : result, result);
            }
            while (current != result);

            return result;
        }

        #endregion
    }
}
