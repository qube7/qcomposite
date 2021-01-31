using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Manages the navigation between the navigable <see cref="ViewModel"/> elements.
    /// </summary>
    /// <typeparam name="T">The type of the navigable <see cref="ViewModel"/> elements.</typeparam>
    /// <typeparam name="TMetadata">The type of the metadata of the navigable <see cref="ViewModel"/> elements.</typeparam>
    /// <threadsafety static="true" instance="true"/>
    public class Navigable<T, TMetadata> : IEnumerable<Lazy<T, TMetadata>>, INavigationService where T : ViewModel where TMetadata : INavigationMetadata
    {
        #region Fields

        /// <summary>
        /// The current navigator.
        /// </summary>
        private volatile Navigator navigator = Navigator.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the content <see cref="ViewModel"/> that was last navigated to.
        /// </summary>
        /// <value>The <see cref="ViewModel"/> that was last navigated to, if available; otherwise, <c>null</c>.</value>
        public T Current
        {
            get { return navigator.Current; }
        }

        /// <summary>
        /// Gets the content <see cref="ViewModel"/> that was last navigated to.
        /// </summary>
        /// <value>The <see cref="ViewModel"/> that was last navigated to, if available; otherwise, <c>null</c>.</value>
        ViewModel INavigationService.Current
        {
            get { return Current; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a new navigation is requested.
        /// </summary>
        public event EventHandler<NavigatingEventArgs> Navigating;

        /// <summary>
        /// Occurs when the navigation has completed.
        /// </summary>
        public event EventHandler<NavigatedEventArgs> Navigated;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Navigable{T, TMetadata}"/> class.
        /// </summary>
        public Navigable()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Replaces elements of current <see cref="Navigable{T, TMetadata}"/> with the elements of the specified collection.
        /// </summary>
        /// <param name="source">The collection of replacement elements.</param>
        public void Recompose(IEnumerable<Lazy<T, TMetadata>> source)
        {
            Requires.NotNull(source, nameof(source));

            Lazy<T, TMetadata>[] items = source.ToArray();

            Navigator recompose;
            Navigator comparand;
            Navigator location = navigator;
            do
            {
                comparand = location;

                recompose = comparand.Recompose(items);

                location = Interlocked.CompareExchange(ref navigator, recompose, comparand);
            }
            while (location != comparand);

            Navigator navigate = recompose.Navigate();
            if (navigate != null)
            {
                try
                {
                    OnNavigating(new NavigatingEventArgs(recompose.Current, navigate.Current, false));
                }
                finally
                {
                    location = Interlocked.CompareExchange(ref navigator, navigate, recompose);
                }

                if (location != recompose)
                {
                    return;
                }

                OnNavigated(new NavigatedEventArgs(recompose.Current, navigate.Current));
            }
        }

        /// <summary>
        /// Navigates to the content <see cref="ViewModel"/> that is represented by the specified route.
        /// </summary>
        /// <param name="route">The route of the <see cref="ViewModel"/> to navigate to.</param>
        /// <returns><c>true</c> if successfully navigated; otherwise, <c>false</c>.</returns>
        public bool Navigate(string route)
        {
            Navigator location = navigator;

            Navigator navigate = location.Navigate(route);
            if (navigate != null)
            {
                NavigatingEventArgs args = new NavigatingEventArgs(location.Current, navigate.Current);

                OnNavigating(args);

                if (args.IsCanceled)
                {
                    return false;
                }

                Navigator comparand = location;

                location = Interlocked.CompareExchange(ref navigator, navigate, comparand);

                if (location != comparand)
                {
                    do
                    {
                        if (ReferenceEquals(location.Current, comparand.Current))
                        {
                            navigate = location.Navigate(route, navigate.Current);
                            if (navigate != null)
                            {
                                comparand = location;

                                location = Interlocked.CompareExchange(ref navigator, navigate, comparand);

                                continue;
                            }
                        }

                        return false;
                    }
                    while (location != comparand);
                }

                OnNavigated(new NavigatedEventArgs(location.Current, navigate.Current));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Raises the <see cref="Navigating"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigatingEventArgs"/> instance containing the event data.</param>
        protected virtual void OnNavigating(NavigatingEventArgs e)
        {
            Event.Raise(Navigating, this, e);
        }

        /// <summary>
        /// Raises the <see cref="Navigated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigatedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnNavigated(NavigatedEventArgs e)
        {
            Event.Raise(Navigated, this, e);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Navigable{T, TMetadata}"/>.
        /// </summary>
        /// <returns>A enumerator that can be used to iterate through the <see cref="Navigable{T, TMetadata}"/>.</returns>
        public IEnumerator<Lazy<T, TMetadata>> GetEnumerator()
        {
            return navigator.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Navigable{T, TMetadata}"/>.
        /// </summary>
        /// <returns>A enumerator that can be used to iterate through the <see cref="Navigable{T, TMetadata}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Manages the navigation of the <see cref="Navigable{T, TMetadata}"/>.
        /// </summary>
        private class Navigator : IEnumerable<Lazy<T, TMetadata>>
        {
            #region Fields

            /// <summary>
            /// The empty navigator.
            /// </summary>
            internal static readonly Navigator Empty = new Navigator(Array.Empty<Lazy<T, TMetadata>>(), null);

            /// <summary>
            /// The navigable items collection.
            /// </summary>
            private readonly IEnumerable<Lazy<T, TMetadata>> items;

            /// <summary>
            /// The current navigable.
            /// </summary>
            private readonly Lazy<T, TMetadata> current;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the current content <see cref="ViewModel"/>.
            /// </summary>
            /// <value>The current <see cref="ViewModel"/>, if available; otherwise, <c>null</c>.</value>
            internal T Current
            {
                get { return current?.Value; }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Navigator"/> class.
            /// </summary>
            /// <param name="items">The navigable items collection.</param>
            /// <param name="current">The current navigable.</param>
            private Navigator(IEnumerable<Lazy<T, TMetadata>> items, Lazy<T, TMetadata> current)
            {
                this.items = items;
                this.current = current;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Creates a new <see cref="Navigator"/> for the specified navigable items collection.
            /// </summary>
            /// <param name="items">The navigable items collection.</param>
            /// <returns>A <see cref="Navigator"/> for the <paramref name="items"/>.</returns>
            internal Navigator Recompose(IEnumerable<Lazy<T, TMetadata>> items)
            {
                return new Navigator(items, current);
            }

            /// <summary>
            /// Creates a new <see cref="Navigator"/> for the current navigable <see cref="ViewModel"/>.
            /// </summary>
            /// <returns>A <see cref="Navigator"/> for the current navigable, if available; otherwise, <c>null</c>.</returns>
            internal Navigator Navigate()
            {
                if (current == null)
                {
                    return null;
                }

                string route = current.Metadata.Route;

                foreach (Lazy<T, TMetadata> item in items)
                {
                    if (item != null)
                    {
                        TMetadata metadata = item.Metadata;
                        if (metadata != null && metadata.Route == route && ReferenceEquals(item.Value, current.Value))
                        {
                            return null;
                        }
                    }
                }

                return new Navigator(items, null);
            }

            /// <summary>
            /// Creates a new <see cref="Navigator"/> for the target navigable <see cref="ViewModel"/> that is represented by the specified route.
            /// </summary>
            /// <param name="route">The route of the <see cref="ViewModel"/> to navigate to.</param>
            /// <param name="target">The target <see cref="ViewModel"/> to navigate to.</param>
            /// <returns>A <see cref="Navigator"/> for the <paramref name="route"/>, if available; otherwise, <c>null</c>.</returns>
            internal Navigator Navigate(string route, T target = null)
            {
                if (route == null)
                {
                    if (current == null)
                    {
                        return null;
                    }

                    return new Navigator(items, null);
                }

                foreach (Lazy<T, TMetadata> item in items)
                {
                    if (item != null)
                    {
                        TMetadata metadata = item.Metadata;
                        if (metadata != null && metadata.Route == route && (target == null || ReferenceEquals(item.Value, target)))
                        {
                            return new Navigator(items, item);
                        }
                    }
                }

                return null;
            }

            /// <summary>
            /// Returns an enumerator that iterates through the <see cref="Navigator"/>.
            /// </summary>
            /// <returns>A enumerator that can be used to iterate through the <see cref="Navigator"/>.</returns>
            public IEnumerator<Lazy<T, TMetadata>> GetEnumerator()
            {
                return items.GetEnumerator();
            }

            /// <summary>
            /// Returns an enumerator that iterates through the <see cref="Navigator"/>.
            /// </summary>
            /// <returns>A enumerator that can be used to iterate through the <see cref="Navigator"/>.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }

        #endregion
    }
}
