using Qube7.ComponentModel;

namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Provides data for the <see cref="INavigationService.Navigating"/> event.
    /// </summary>
    public class NavigatingEventArgs : CancelableEventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="ViewModel"/> that is the origin for the navigation.
        /// </summary>
        /// <value>The current <see cref="ViewModel"/>.</value>
        public ViewModel Current { get; private set; }

        /// <summary>
        /// Gets the <see cref="ViewModel"/> that is the destination for the navigation.
        /// </summary>
        /// <value>The target <see cref="ViewModel"/>.</value>
        public ViewModel Target { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatingEventArgs"/> class.
        /// </summary>
        /// <param name="current">The current <see cref="ViewModel"/>.</param>
        /// <param name="target">The target <see cref="ViewModel"/>.</param>
        /// <param name="canCancel">A value indicating whether the event can be canceled.</param>
        public NavigatingEventArgs(ViewModel current, ViewModel target, bool canCancel) : base(canCancel)
        {
            Current = current;
            Target = target;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatingEventArgs"/> class.
        /// </summary>
        /// <param name="current">The current <see cref="ViewModel"/>.</param>
        /// <param name="target">The target <see cref="ViewModel"/>.</param>
        public NavigatingEventArgs(ViewModel current, ViewModel target) : this(current, target, true)
        {
        }

        #endregion
    }
}
