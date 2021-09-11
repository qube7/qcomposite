namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Represents an object that supports navigation.
    /// </summary>
    public interface INavigable
    {
        #region Methods

        /// <summary>
        /// Called when the navigation to the current object within the specified region is requested.
        /// </summary>
        /// <param name="regionName">The name of the navigable region that received the request.</param>
        /// <param name="data">The data passed with the navigation request.</param>
        /// <returns><c>true</c> if this instance accepts the navigation request; otherwise, <c>false</c>.</returns>
        bool NavigatingTo(string regionName, object data) { return true; }

        /// <summary>
        /// Called when the navigation to the current object within the specified region has completed.
        /// </summary>
        /// <param name="regionName">The name of the navigable region that received the request.</param>
        /// <param name="data">The data passed with the navigation request.</param>
        void NavigatedTo(string regionName, object data) { }

        /// <summary>
        /// Called when the navigation from the current object within the specified region is requested.
        /// </summary>
        /// <param name="regionName">The name of the navigable region that received the request.</param>
        /// <returns><c>true</c> if this instance accepts the navigation request; otherwise, <c>false</c>.</returns>
        bool NavigatingFrom(string regionName) { return true; }

        /// <summary>
        /// Called when the navigation from the current object within the specified region has completed.
        /// </summary>
        /// <param name="regionName">The name of the navigable region that received the request.</param>
        void NavigatedFrom(string regionName) { }

        #endregion
    }
}
