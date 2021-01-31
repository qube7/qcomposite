using System.Windows;

namespace Qube7.Composite.KnownBoxes
{
    /// <summary>
    /// Provides the <see cref="Visibility"/> boxed values.
    /// </summary>
    public static class VisibilityBox
    {
        #region Fields

        /// <summary>
        /// Represents the <see cref="Visibility.Visible"/> boxed value.
        /// </summary>
        public static readonly object Visible = Visibility.Visible;

        /// <summary>
        /// Represents the <see cref="Visibility.Hidden"/> boxed value.
        /// </summary>
        public static readonly object Hidden = Visibility.Hidden;

        /// <summary>
        /// Represents the <see cref="Visibility.Collapsed"/> boxed value.
        /// </summary>
        public static readonly object Collapsed = Visibility.Collapsed;

        #endregion

        #region Methods

        /// <summary>
        /// Returns a boxed representation of the specified value.
        /// </summary>
        /// <param name="value">The value to be boxed.</param>
        /// <returns>An object that wraps the <paramref name="value"/>.</returns>
        public static object FromValue(Visibility value)
        {
            switch (value)
            {
                case Visibility.Hidden:
                    return Hidden;
                case Visibility.Collapsed:
                    return Collapsed;
            }

            return Visible;
        }

        /// <summary>
        /// Returns a boxed representation of the specified value.
        /// </summary>
        /// <param name="value">The value to be boxed.</param>
        /// <returns>An object that wraps the <paramref name="value"/>.</returns>
        public static object FromValue(Visibility? value)
        {
            return value.HasValue ? FromValue(value.Value) : null;
        }

        #endregion
    }
}
