namespace Qube7.Composite
{
    /// <summary>
    /// Contains constant metadata keys used by the composition system.
    /// </summary>
    internal static class MetadataName
    {
        #region Fields

        /// <summary>
        /// Specifies the metadata key created by the composition system to mark an export scope.
        /// </summary>
        internal const string ExportScope = nameof(ExportScopeAttribute.ExportScope);

        /// <summary>
        /// Specifies the metadata key created by the composition system to mark a method that requires the user interface thread.
        /// </summary>
        internal const string RequiresUIThread = nameof(RequiresUIThreadAttribute.RequiresUIThread);

        #endregion
    }
}
