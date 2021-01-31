using System.Collections.Generic;
using System.IO;

namespace Qube7.Composite.Design.Wizards
{
    /// <summary>
    /// Provides the naming convention helpers.
    /// </summary>
    internal static class Convention
    {
        #region Fields

        /// <summary>
        /// The zero-index format item string.
        /// </summary>
        private const string Format = "{0}";

        #endregion

        #region Methods

        /// <summary>
        /// Creates the file name convention format using the specified naming part.
        /// </summary>
        /// <param name="dictionary">The string replacements dictionary.</param>
        /// <param name="part">The naming convention part.</param>
        /// <returns>The created naming convention format.</returns>
        internal static string FileName(Dictionary<string, string> dictionary, string part)
        {
            if (dictionary.TryGetValue("$rootname$", out string value))
            {
                value = Path.GetFileNameWithoutExtension(value);

                int index = value.LastIndexOf(part);
                if (index >= 0)
                {
                    return string.Concat(value.Substring(0, index), Format, value.Substring(index + part.Length));
                }

                return string.Concat(value, Format);
            }

            return null;
        }

        /// <summary>
        /// Creates the safe name convention format using the specified naming part.
        /// </summary>
        /// <param name="dictionary">The string replacements dictionary.</param>
        /// <param name="part">The naming convention part.</param>
        /// <returns>The created naming convention format.</returns>
        internal static string SafeName(Dictionary<string, string> dictionary, string part)
        {
            if (dictionary.TryGetValue("$safeitemname$", out string value))
            {
                int index = value.LastIndexOf(part);
                if (index >= 0)
                {
                    return string.Concat(value.Substring(0, index), Format, value.Substring(index + part.Length));
                }

                return string.Concat(value, Format);
            }

            return null;
        }

        #endregion
    }
}
