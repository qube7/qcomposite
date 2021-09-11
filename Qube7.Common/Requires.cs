using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Qube7
{
    /// <summary>
    /// Specifies contract preconditions.
    /// </summary>
    [DebuggerStepThrough]
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
        public static void NotNull<T>(T value, string paramName) where T : class
        {
            if (value is null)
            {
                throw Error.ArgumentNull(paramName);
            }
        }

        /// <summary>
        /// Requires the not <c>null</c> or empty <see cref="IEnumerable{T}"/> collection.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="values"/>.</typeparam>
        /// <param name="values">The argument collection.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="values"/> is <c>null</c> collection.</exception>
        /// <exception cref="ArgumentException">The <paramref name="values"/> is empty <see cref="IEnumerable{T}"/>.</exception>
        public static void NotNullOrEmpty<T>(IEnumerable<T> values, string paramName)
        {
            NotNull(values, paramName);

            if (values.Any())
            {
                return;
            }

            throw Error.Argument(Strings.ArgumentEmpty, paramName);
        }

        /// <summary>
        /// Requires the <see cref="IEnumerable{T}"/> collection to not contain <c>null</c> elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="values"/>.</typeparam>
        /// <param name="values">The argument collection.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentException">The <paramref name="values"/> contains <c>null</c> element.</exception>
        public static void NotNullElements<T>(IEnumerable<T> values, string paramName) where T : class
        {
            if (values is null)
            {
                return;
            }

            foreach (T value in values)
            {
                if (value is null)
                {
                    throw Error.Argument(Strings.ArgumentNullElement, paramName);
                }
            }
        }

        /// <summary>
        /// Requires the not <c>null</c> or empty <see cref="String"/> value.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is <c>null</c> value.</exception>
        /// <exception cref="ArgumentException">The <paramref name="value"/> is empty <see cref="String"/> value.</exception>
        public static void NotNullOrEmpty(string value, string paramName)
        {
            NotNull(value, paramName);

            if (value.Length == 0)
            {
                throw Error.Argument(Strings.ArgumentEmptyString, paramName);
            }
        }

        /// <summary>
        /// Requires the not <c>null</c> or empty, or consisting only of white-space characters <see cref="String"/> value.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is <c>null</c> value.</exception>
        /// <exception cref="ArgumentException">The <paramref name="value"/> is empty, or consisting only of white-space characters <see cref="String"/> value.</exception>
        public static void NotNullOrWhiteSpace(string value, string paramName)
        {
            NotNullOrEmpty(value, paramName);

            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsWhiteSpace(value[i]))
                {
                    continue;
                }

                return;
            }

            throw Error.Argument(Strings.ArgumentWhiteSpace, paramName);
        }

        /// <summary>
        /// Requires the non-negative number value.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value"/> is negative number value.</exception>
        public static void NonNegative(int value, string paramName)
        {
            if (value < 0)
            {
                throw Error.ArgumentOutOfRange(Strings.ArgumentNegative, paramName);
            }
        }

        /// <summary>
        /// Requires the <see cref="Type"/> value, an instance of which can be assigned to a variable of the <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">The target type of the assignment.</typeparam>
        /// <param name="value">The argument value.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is <c>null</c> value.</exception>
        /// <exception cref="ArgumentException">The <paramref name="value"/> is not assignable to the <typeparamref name="T"/> type.</exception>
        public static void AssignableTo<T>(Type value, string paramName)
        {
            NotNull(value, paramName);

            if (typeof(T).IsAssignableFrom(value))
            {
                return;
            }

            throw Error.Argument(string.Format(Strings.ArgumentNotAssignable, value.FullName, typeof(T).FullName), paramName);
        }

        #endregion
    }
}
