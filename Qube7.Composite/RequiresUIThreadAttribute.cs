using System;
using System.ComponentModel.Composition;

namespace Qube7.Composite
{
    /// <summary>
    /// Specifies whether a method marked with the <see cref="ExportAttribute"/> provides particular exports of the delegate that expects to be invoked on the user interface thread.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class RequiresUIThreadAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether the method marked with this attribute requires to be called on the user interface thread.
        /// </summary>
        /// <value><c>true</c> if the method requires the user interface thread; otherwise, <c>false</c>.</value>
        public bool RequiresUIThread { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiresUIThreadAttribute"/> class with the <see cref="RequiresUIThread"/> property set to the given value.
        /// </summary>
        /// <param name="requiresUIThread"><c>true</c> if the method requires the user interface thread; otherwise, <c>false</c>.</param>
        public RequiresUIThreadAttribute(bool requiresUIThread)
        {
            RequiresUIThread = requiresUIThread;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiresUIThreadAttribute"/> class with the <see cref="RequiresUIThread"/> property set to <c>true</c>.
        /// </summary>
        public RequiresUIThreadAttribute() : this(true)
        {
        }

        #endregion
    }
}
