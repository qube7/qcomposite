using System.Globalization;

namespace Qube7
{
    /// <summary>
    /// Provides culture-specific string formatting.
    /// </summary>
    public static class Format
    {
        #region Methods

        /// <summary>
        /// Formats the specified string using culture associated with the current thread.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>The formatted string.</returns>
        public static string Current(string format, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        /// <summary>
        /// Formats the specified string using user interface culture associated with the current thread.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>The formatted string.</returns>
        public static string CurrentUI(string format, params object[] args)
        {
            return string.Format(CultureInfo.CurrentUICulture, format, args);
        }

        /// <summary>
        /// Formats the specified string using culture installed with the operating system.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>The formatted string.</returns>
        public static string InstalledUI(string format, params object[] args)
        {
            return string.Format(CultureInfo.InstalledUICulture, format, args);
        }

        /// <summary>
        /// Formats the specified string using invariant culture.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>The formatted string.</returns>
        public static string Invariant(string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        #endregion
    }
}
