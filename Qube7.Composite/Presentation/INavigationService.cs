using System.Threading;
using System.Threading.Tasks;

namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Defines a service that manages navigation within a region in the user interface.
    /// </summary>
    public interface INavigationService
    {
        #region Properties

        /// <summary>
        /// Gets the object in the region that was last navigated to.
        /// </summary>
        /// <value>The object that was last navigated to, if available; otherwise, <c>null</c>.</value>
        object Current { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Navigates to the object that is represented by the specified route.
        /// </summary>
        /// <param name="route">The route of the object to navigate to.</param>
        /// <param name="data">The data to pass with the navigation request.</param>
        /// <returns><c>true</c> if successfully navigated; otherwise, <c>false</c>.</returns>
        bool Navigate(string route, object data);

        /// <summary>
        /// Navigates to the object that is represented by the specified route.
        /// </summary>
        /// <param name="route">The route of the object to navigate to.</param>
        /// <param name="data">The data to pass with the navigation request.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task{TResult}"/> that represents the asynchronous operation.</returns>
        Task<bool> NavigateAsync(string route, object data, CancellationToken cancellationToken = default);

        #endregion
    }
}
