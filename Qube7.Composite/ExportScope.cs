namespace Qube7.Composite
{
    /// <summary>
    /// Specifies the scope of visibility of an export.
    /// </summary>
    public enum ExportScope
    {
        /// <summary>
        /// The export is visible at an application scope. This is the default value.
        /// </summary>
        Public,
        /// <summary>
        /// The export is shared within a current module.
        /// </summary>
        Internal,
        /// <summary>
        /// The export is shared within a current controller subtree.
        /// </summary>
        Protected,
        /// <summary>
        /// The export is visible locally within a controller.
        /// </summary>
        Private
    }
}
