namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Specifies the metadata associated with the object that supports navigation.
    /// </summary>
    public interface INavigationMetadata
    {
        #region Properties

        /// <summary>
        /// Gets the navigation route of the exported navigable object.
        /// </summary>
        /// <value>The navigation route of the exported object.</value>
        string Route { get; }

        #endregion
    }
}
