using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Qube7.Composite.Threading;

namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Provides extension methods for navigating the user interface.
    /// </summary>
    public static class NavigationService
    {
        #region Methods

        /// <summary>
        /// Navigates to the object that is represented by the specified route.
        /// </summary>
        /// <param name="service">The navigation service.</param>
        /// <param name="route">The route of the object to navigate to.</param>
        /// <returns><c>true</c> if successfully navigated; otherwise, <c>false</c>.</returns>
        public static bool Navigate(this INavigationService service, string route)
        {
            Requires.NotNull(service, nameof(service));

            return service.Navigate(route, null);
        }

        /// <summary>
        /// Navigates to the object that is represented by the specified route.
        /// </summary>
        /// <param name="service">The navigation service.</param>
        /// <param name="route">The route of the object to navigate to.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task{TResult}"/> that represents the asynchronous operation.</returns>
        public static Task<bool> NavigateAsync(this INavigationService service, string route, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(service, nameof(service));

            return service.NavigateAsync(route, null, cancellationToken);
        }

        /// <summary>
        /// Creates a <see cref="INavigationService"/> wrapper over the <see cref="NavigableCollection{T, TMetadata}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the navigable objects in the collection.</typeparam>
        /// <typeparam name="TMetadata">The type of the metadata of the navigable objects.</typeparam>
        /// <param name="collection">The <see cref="NavigableCollection{T, TMetadata}"/> to wrap.</param>
        /// <returns>The <see cref="INavigationService"/> wrapper over the <paramref name="collection"/>.</returns>
        public static INavigationService CreateService<T, TMetadata>(this NavigableCollection<T, TMetadata> collection) where T : class where TMetadata : INavigationMetadata
        {
            Requires.NotNull(collection, nameof(collection));

            return new NavigableCollectionService<T, TMetadata>(collection);
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Adapts the <see cref="NavigableCollection{T, TMetadata}"/> to the <see cref="INavigationService"/>.
        /// </summary>
        /// <typeparam name="T">The type of the navigable objects in the collection.</typeparam>
        /// <typeparam name="TMetadata">The type of the metadata of the navigable objects.</typeparam>
        private class NavigableCollectionService<T, TMetadata> : INavigationService where T : class where TMetadata : INavigationMetadata
        {
            #region Fields

            /// <summary>
            /// The underlying collection.
            /// </summary>
            private readonly NavigableCollection<T, TMetadata> collection;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the object in the region that was last navigated to.
            /// </summary>
            /// <value>The object that was last navigated to, if available; otherwise, <c>null</c>.</value>
            public object Current
            {
                get { return collection.Current; }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="NavigableCollectionService{T, TMetadata}"/> class.
            /// </summary>
            /// <param name="collection">The underlying collection.</param>
            internal NavigableCollectionService(NavigableCollection<T, TMetadata> collection)
            {
                this.collection = collection;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Navigates to the object that is represented by the specified route.
            /// </summary>
            /// <param name="route">The route of the object to navigate to.</param>
            /// <param name="data">The data to pass with the navigation request.</param>
            /// <returns><c>true</c> if successfully navigated; otherwise, <c>false</c>.</returns>
            public bool Navigate(string route, object data)
            {
                if (UIThread.CheckAccess())
                {
                    return collection.Navigate(route, data);
                }

                return UIThread.Invoke(() => collection.Navigate(route, data), DispatcherPriority.Normal);
            }

            /// <summary>
            /// Navigates to the object that is represented by the specified route.
            /// </summary>
            /// <param name="route">The route of the object to navigate to.</param>
            /// <param name="data">The data to pass with the navigation request.</param>
            /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
            /// <returns>The <see cref="Task{TResult}"/> that represents the asynchronous operation.</returns>
            public async Task<bool> NavigateAsync(string route, object data, CancellationToken cancellationToken = default)
            {
                if (UIThread.CheckAccess())
                {
                    return collection.Navigate(route, data);
                }

                return await UIThread.InvokeAsync(() => collection.Navigate(route, data), DispatcherPriority.Normal, cancellationToken);
            }

            #endregion
        }

        #endregion
    }
}
