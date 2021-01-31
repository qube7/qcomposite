using System;
using System.ComponentModel.Composition;

namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Specifies that a type, property, or field marked with the <see cref="ExportAttribute"/> provides particular exports of the object that supports navigation.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
    public sealed class NavigableAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets the navigation route of the exported navigable object.
        /// </summary>
        /// <value>The navigation route of the exported object.</value>
        public string Route { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigableAttribute"/> class.
        /// </summary>
        /// <param name="route">The navigation route of the exported object.</param>
        public NavigableAttribute(string route)
        {
            Route = route;
        }

        #endregion
    }
}
