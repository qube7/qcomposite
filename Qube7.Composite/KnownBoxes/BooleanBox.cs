using System;

namespace Qube7.Composite.KnownBoxes
{
    /// <summary>
    /// Provides the <see cref="Boolean"/> boxed values.
    /// </summary>
    public static class BooleanBox
    {
        #region Fields

        /// <summary>
        /// Represents the <c>false</c> boxed value.
        /// </summary>
        public static readonly object False = false;

        /// <summary>
        /// Represents the <c>true</c> boxed value.
        /// </summary>
        public static readonly object True = true;

        #endregion

        #region Methods

        /// <summary>
        /// Returns a boxed representation of the specified value.
        /// </summary>
        /// <param name="value">The value to be boxed.</param>
        /// <returns>An object that wraps the <paramref name="value"/>.</returns>
        public static object FromValue(bool value)
        {
            return value ? True : False;
        }

        /// <summary>
        /// Returns a boxed representation of the specified value.
        /// </summary>
        /// <param name="value">The value to be boxed.</param>
        /// <returns>An object that wraps the <paramref name="value"/>.</returns>
        public static object FromValue(bool? value)
        {
            return value.HasValue ? FromValue(value.Value) : null;
        }

        #endregion
    }
}
