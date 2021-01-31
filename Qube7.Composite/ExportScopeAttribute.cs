using System;
using System.ComponentModel.Composition;

namespace Qube7.Composite
{
    /// <summary>
    /// Specifies that a type, property, field, or method marked with the <see cref="ExportAttribute"/> provides particular exports that are available in the given scope.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
    public sealed class ExportScopeAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets the scope of visibility of the exports provided by the type or member marked with this attribute.
        /// </summary>
        /// <value>The scope of visibility of the provided exports.</value>
        public ExportScope ExportScope { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportScopeAttribute"/> class.
        /// </summary>
        /// <param name="exportScope">The scope of visibility of the provided exports.</param>
        public ExportScopeAttribute(ExportScope exportScope)
        {
            ExportScope = exportScope;
        }

        #endregion
    }
}
