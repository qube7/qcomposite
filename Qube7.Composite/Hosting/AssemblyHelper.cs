using System;
using System.Diagnostics;
using System.Reflection;

namespace Qube7.Composite.Hosting
{
    /// <summary>
    /// Provides helpers for the assembly management.
    /// </summary>
    internal static class AssemblyHelper
    {
        #region Methods

        /// <summary>
        /// Verifies whether the specified assembly file matches the expected identity.
        /// </summary>
        /// <param name="assemblyPath">The location of the assembly file to test.</param>
        /// <param name="expected">The expected identity of the assembly.</param>
        internal static void VerifyAssemblyName(string assemblyPath, AssemblyName expected)
        {
            if (expected.Name == null)
            {
                return;
            }

            AssemblyName actual = AssemblyName.GetAssemblyName(assemblyPath);

            actual.CodeBase = assemblyPath;

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

            throw Error.InvalidOperation(Format.Current(Strings.AssemblyNameMismatch, actual.CodeBase, expected, actual));
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

                    throw Error.InvalidOperation(Format.Current(Strings.AssemblyNameMismatch, actual.CodeBase, expected, actual));
                }

                return;
            }

            throw Error.InvalidOperation(Format.Current(Strings.AssemblyNameMismatch, actual.CodeBase, expected, actual));
        }

        /// <summary>
        /// Determines whether the specified assembly file matches the expected identity.
        /// </summary>
        /// <param name="assemblyPath">The location of the assembly file to test.</param>
        /// <param name="expected">The expected identity of the assembly.</param>
        /// <returns><c>true</c> if the identity of the specified assembly matches the <paramref name="expected"/> identity; otherwise, <c>false</c>.</returns>
        internal static bool MatchAssemblyName(string assemblyPath, AssemblyName expected)
        {
            AssemblyName actual = AssemblyName.GetAssemblyName(assemblyPath);

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
