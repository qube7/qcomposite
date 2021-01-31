using System;

namespace Qube7.Composite.Hosting
{
    /// <summary>
    /// Provides helpers for the module management.
    /// </summary>
    internal static class ModuleHelper
    {
        #region Fields

        /// <summary>
        /// The type of the <see cref="Module"/>.
        /// </summary>
        private static readonly Type module = typeof(Module);

        #endregion

        #region Methods

        /// <summary>
        /// Verifies whether the specified type is valid type of the composite application module.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <remarks>The module type must be concrete type, assignable to the <see cref="Module"/> type, that has public parameterless constructor.</remarks>
        internal static void VerifyModule(Type type)
        {
            if (MatchModule(type))
            {
                return;
            }

            throw Error.InvalidOperation(Format.Current(Strings.ModuleTypeInvalid, type, typeof(Module)));
        }

        /// <summary>
        /// Determines whether the specified type is valid type of the composite application module.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <returns><c>true</c> if the constraints for the module type are satisfied; otherwise, <c>false</c>.</returns>
        /// <remarks>The module type must be concrete type, assignable to the <see cref="Module"/> type, that has public parameterless constructor.</remarks>
        internal static bool MatchModule(Type type)
        {
            Requires.NotNull(type, nameof(type));

            if (type.IsAbstract || type.ContainsGenericParameters)
            {
                return false;
            }

            return module.IsAssignableFrom(type) && type.GetConstructor(Array.Empty<Type>()) != null;
        }

        #endregion
    }
}
