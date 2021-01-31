using System;

namespace Qube7
{
    /// <summary>
    /// Provides helpers for the prime numbers.
    /// </summary>
    public static class Prime
    {
        #region Methods

        /// <summary>
        /// Determines whether the specified value is an prime number.
        /// </summary>
        /// <param name="candidate">The candidate to test.</param>
        /// <returns><c>true</c> if the <paramref name="candidate"/> is an prime number; otherwise, <c>false</c>.</returns>
        public static bool Test(int candidate)
        {
            if ((candidate & 1) == 0)
            {
                return candidate == 2;
            }

            int sqrt = (int)Math.Sqrt(candidate);

            for (int i = 3; i <= sqrt; i += 2)
            {
                if ((candidate % i) == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Searches for the prime number that is equal or greater than the specified value.
        /// </summary>
        /// <param name="min">The starting value of the search.</param>
        /// <returns>The prime number, if found; otherwise, 0.</returns>
        public static int FindNext(int min)
        {
            for (int i = min | 1; i < int.MaxValue; i += 2)
            {
                if (Test(i))
                {
                    return i;
                }
            }

            return 0;
        }

        #endregion
    }
}
