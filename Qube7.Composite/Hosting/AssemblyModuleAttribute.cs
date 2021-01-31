using System;

namespace Qube7.Composite.Hosting
{
    /// <summary>
    /// Specifies a composite application module provided by an assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class AssemblyModuleAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets the type of the module provided by an assembly.
        /// </summary>
        /// <value>The type of the provided module.</value>
        public Type ModuleType { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyModuleAttribute"/> class.
        /// </summary>
        /// <param name="moduleType">The type of the provided module.</param>
        /// <remarks>The value of the <paramref name="moduleType"/> must be concrete type, assignable to the <see cref="Module"/> type, that has public parameterless constructor.</remarks>
        public AssemblyModuleAttribute(Type moduleType)
        {
            ModuleType = moduleType;
        }

        #endregion
    }
}
