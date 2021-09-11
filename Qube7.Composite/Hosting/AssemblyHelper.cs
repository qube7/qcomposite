using System;
using System.Diagnostics;
using System.Reflection;

namespace Qube7.Composite.Hosting
{
    /// <summary>
    /// Provides helper methods for checking identity of the assembly.
    /// </summary>
    internal static class AssemblyHelper
    {
        #region Methods

        /// <summary>
        /// Verifies whether the specified <see cref="AssemblyName"/> matches the expected identity.
        /// </summary>
        /// <param name="expected">The expected identity of the assembly.</param>
        /// <param name="actual">The actual <see cref="AssemblyName"/> to test.</param>
        internal static void VerifyAssemblyName(AssemblyName expected, AssemblyName actual)
        {
            if (expected.Name == null)
            {
                return;
            }

            if (string.Equals(expected.Name, actual.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (expected.Version == null)
                {
                    return;
                }

                if (expected.Version == actual.Version)
                {
                    VerifyPublicKeyToken(expected, actual);

                    return;
                }
            }

            throw Error.InvalidOperation(string.Format(Strings.AssemblyNameMismatch, actual.CodeBase, expected, actual));
        }

        /// <summary>
        /// Verifies whether the specified <see cref="AssemblyName"/> matches the expected public key token identity.
        /// </summary>
        /// <param name="expected">The expected identity of the assembly.</param>
        /// <param name="actual">The actual <see cref="AssemblyName"/> to test.</param>
        [Conditional("RELEASE")]
        private static void VerifyPublicKeyToken(AssemblyName expected, AssemblyName actual)
        {
            byte[] token1 = expected.GetPublicKeyToken();

            if (token1 == null)
            {
                return;
            }

            byte[] token2 = actual.GetPublicKeyToken();

            if (token2 != null && token2.Length == token1.Length)
            {
                for (int i = 0; i < token1.Length; i++)
                {
                    if (token1[i] == token2[i])
                    {
                        continue;
                    }

                    throw Error.InvalidOperation(string.Format(Strings.AssemblyNameMismatch, actual.CodeBase, expected, actual));
                }

                return;
            }

            throw Error.InvalidOperation(string.Format(Strings.AssemblyNameMismatch, actual.CodeBase, expected, actual));
        }

        /// <summary>
        /// Determines whether the specified <see cref="AssemblyName"/> matches the expected identity.
        /// </summary>
        /// <param name="expected">The expected identity of the assembly.</param>
        /// <param name="actual">The actual <see cref="AssemblyName"/> to test.</param>
        /// <returns><c>true</c> if the <paramref name="actual"/> assembly identity matches the <paramref name="expected"/> identity; otherwise, <c>false</c>.</returns>
        internal static bool MatchAssemblyName(AssemblyName expected, AssemblyName actual)
        {
            return string.Equals(expected.Name, actual.Name, StringComparison.OrdinalIgnoreCase) && (expected.Version == null || (expected.Version == actual.Version && MatchPublicKeyToken(expected, actual)));
        }

        /// <summary>
        /// Determines whether the specified <see cref="AssemblyName"/> matches the expected public key token identity.
        /// </summary>
        /// <param name="expected">The expected identity of the assembly.</param>
        /// <param name="actual">The actual <see cref="AssemblyName"/> to test.</param>
        /// <returns><c>true</c> if the <paramref name="actual"/> assembly identity matches the <paramref name="expected"/> public key token identity; otherwise, <c>false</c>.</returns>
        private static bool MatchPublicKeyToken(AssemblyName expected, AssemblyName actual)
        {
            byte[] token1 = expected.GetPublicKeyToken();

            if (token1 == null)
            {
                return true;
            }

            byte[] token2 = actual.GetPublicKeyToken();

            if (token2 != null && token2.Length == token1.Length)
            {
                for (int i = 0; i < token1.Length; i++)
                {
                    if (token1[i] == token2[i])
                    {
                        continue;
                    }

                    return false;
                }

                return true;
            }

            return false;
        }

        #endregion
    }
}
