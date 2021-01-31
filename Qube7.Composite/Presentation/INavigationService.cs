using System;

namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Defines a service that manages the navigation for the application area.
    /// </summary>
    public interface INavigationService
    {
        #region Events

        /// <summary>
        /// Occurs when a new navigation is requested.
        /// </summary>
        event EventHandler<NavigatingEventArgs> Navigating;

        /// <summary>
        /// Occurs when the navigation has completed.
        /// </summary>
        event EventHandler<NavigatedEventArgs> Navigated;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the content <see cref="ViewModel"/> that was last navigated to.
        /// </summary>
        /// <value>The <see cref="ViewModel"/> that was last navigated to, if available; otherwise, <c>null</c>.</value>
        ViewModel Current { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Navigates to the content <see cref="ViewModel"/> that is represented by the specified route.
        /// </summary>
        /// <param name="route">The route of the <see cref="ViewModel"/> to navigate to.</param>
        /// <returns><c>true</c> if successfully navigated; otherwise, <c>false</c>.</returns>
        bool Navigate(string route);

        #endregion
    }
}
