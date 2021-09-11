using System;

namespace Qube7.Composite.Hosting
{
    /// <summary>
    /// Provides support for creating <see cref="Module"/> objects.
    /// </summary>
    internal static class ModuleFactory
    {
        #region Fields

        /// <summary>
        /// The type of the <see cref="Module"/>.
        /// </summary>
        private static readonly Type BaseType = typeof(Module);

        #endregion

        #region Methods

        /// <summary>
        /// Creates the module instance of the specified type.
        /// </summary>
        /// <param name="type">The type of the module to create.</param>
        /// <returns>The created module object instance.</returns>
        internal static Module CreateModule(Type type)
        {
            Requires.NotNull(type, nameof(type));

            if (CheckModule(type))
            {
                return Activator.CreateInstance(type) as Module;
            }

            throw Error.InvalidOperation(string.Format(Strings.ModuleTypeInvalid, type, typeof(Module)));
        }

        /// <summary>
        /// Determines whether the specified type is valid type of the module.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <returns><c>true</c> if the constraints for the module type are satisfied; otherwise, <c>false</c>.</returns>
        private static bool CheckModule(Type type)
        {
            if (type.IsAbstract || type.ContainsGenericParameters)
            {
                return false;
            }

            return BaseType.IsAssignableFrom(type) && type.GetConstructor(Array.Empty<Type>()) != null;
        }

        #endregion
    }
}
