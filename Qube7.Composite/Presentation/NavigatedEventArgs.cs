using System;

namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Provides data for the <see cref="INavigationService.Navigated"/> event.
    /// </summary>
    public class NavigatedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="ViewModel"/> that is the origin for the navigation.
        /// </summary>
        /// <value>The previous <see cref="ViewModel"/>.</value>
        public ViewModel Previous { get; private set; }

        /// <summary>
        /// Gets the <see cref="ViewModel"/> that is the destination for the navigation.
        /// </summary>
        /// <value>The current <see cref="ViewModel"/>.</value>
        public ViewModel Current { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatedEventArgs"/> class.
        /// </summary>
        /// <param name="previous">The previous <see cref="ViewModel"/>.</param>
        /// <param name="current">The current <see cref="ViewModel"/>.</param>
        public NavigatedEventArgs(ViewModel previous, ViewModel current)
        {
            Previous = previous;
            Current = current;
        }

        #endregion
    }
}
