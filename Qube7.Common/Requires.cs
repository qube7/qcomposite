using System;
using System.Diagnostics;

namespace Qube7
{
    /// <summary>
    /// Specifies contract preconditions.
    /// </summary>
    public static class Requires
    {
        #region Methods

        /// <summary>
        /// Requires the not <c>null</c> value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The argument value.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is <c>null</c> value.</exception>
        [DebuggerStepThrough]
        public static void NotNull<T>(T value, string paramName)
        {
            if (value == null)
            {
                throw Error.ArgumentNull(paramName);
            }
        }

        /// <summary>
        /// Requires the not <c>null</c> or empty <see cref="String"/> value.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is <c>null</c> value.</exception>
        /// <exception cref="ArgumentException">The <paramref name="value"/> is empty <see cref="String"/> value.</exception>
        [DebuggerStepThrough]
        public static void NotNullOrEmpty(string value, string paramName)
        {
            NotNull(value, paramName);

            if (value.Length == 0)
            {
                throw Error.Argument(Strings.ArgumentEmptyString, paramName);
            }
        }

        /// <summary>
        /// Requires the non-negative number value.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value"/> is negative number value.</exception>
        [DebuggerStepThrough]
        public static void NonNegative(int value, string paramName)
        {
            if (value < 0)
            {
                throw Error.ArgumentOutOfRange(Strings.ArgumentNegative, paramName);
            }
        }

        /// <summary>
        /// Requires the <paramref name="type"/> to be safely assignable to the <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">The target type of the assignment.</typeparam>
        /// <param name="type">The source <see cref="Type"/> of the assignment.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c> value.</exception>
        /// <exception cref="ArgumentException">The <paramref name="type"/> is not assignable to the <typeparamref name="T"/> type.</exception>
        [DebuggerStepThrough]
        public static void Assignable<T>(Type type, string paramName)
        {
            NotNull(type, paramName);

            if (typeof(T).IsAssignableFrom(type))
            {
                return;
            }

            throw Error.Argument(Format.Current(Strings.ArgumentNotAssignable, type.FullName, typeof(T).FullName), paramName);
        }

        #endregion
    }
}
