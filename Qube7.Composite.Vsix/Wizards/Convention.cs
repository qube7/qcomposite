using System.Collections.Generic;
using System.IO;

namespace Qube7.Composite.Design.Wizards
{
    /// <summary>
    /// Provides extension methods for template replacement parameters.
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
        /// <param name="fileName">When this method returns, contains the created naming convention format.</param>
        /// <returns><c>true</c> if <paramref name="fileName"/> is created successfully; otherwise, <c>false</c>.</returns>
        internal static bool FileName(this Dictionary<string, string> dictionary, string part, out string fileName)
        {
            if (dictionary.TryGetValue("$rootname$", out string value))
            {
                value = Path.GetFileNameWithoutExtension(value);

                int index = value.LastIndexOf(part);
                if (index >= 0)
                {
                    fileName = string.Concat(value.Substring(0, index), Format, value.Substring(index + part.Length));

                    return true;
                }

                fileName = string.Concat(value, Format);

                return true;
            }

            fileName = null;

            return false;
        }

        /// <summary>
        /// Creates the safe name convention format using the specified naming part.
        /// </summary>
        /// <param name="dictionary">The string replacements dictionary.</param>
        /// <param name="part">The naming convention part.</param>
        /// <param name="safeName">When this method returns, contains the created naming convention format.</param>
        /// <returns><c>true</c> if <paramref name="safeName"/> is created successfully; otherwise, <c>false</c>.</returns>
        internal static bool SafeName(this Dictionary<string, string> dictionary, string part, out string safeName)
        {
            if (dictionary.TryGetValue("$safeitemname$", out string value))
            {
                int index = value.LastIndexOf(part);
                if (index >= 0)
                {
                    safeName = string.Concat(value.Substring(0, index), Format, value.Substring(index + part.Length));

                    return true;
                }

                safeName = string.Concat(value, Format);

                return true;
            }

            safeName = null;

            return false;
        }

        /// <summary>
        /// Retrieves the safe item name replacement parameter.
        /// </summary>
        /// <param name="dictionary">The string replacements dictionary.</param>
        /// <param name="itemName">When this method returns, contains the retrieved parameter.</param>
        /// <returns><c>true</c> if <paramref name="itemName"/> is retrieved successfully; otherwise, <c>false</c>.</returns>
        internal static bool ItemName(this Dictionary<string, string> dictionary, out string itemName)
        {
            return dictionary.TryGetValue("$safeitemname$", out itemName);
        }

        #endregion
    }
}
