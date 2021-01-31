using System;
using Qube7.ComponentModel;
using Qube7.Threading;

namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Wraps the <see cref="INavigationService"/> providing weak navigation events notification.
    /// </summary>
    public class NavigationServiceProxy : INavigationService, IDisposable
    {
        #region Fields

        /// <summary>
        /// The wrapped <see cref="INavigationService"/>.
        /// </summary>
        private readonly INavigationService wrapped;

        /// <summary>
        /// The event listener.
        /// </summary>
        private readonly EventListener listener;

        /// <summary>
        /// A value indicating whether the <see cref="NavigationServiceProxy"/> is disposed.
        /// </summary>
        private int disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the content <see cref="ViewModel"/> that was last navigated to.
        /// </summary>
        /// <value>The <see cref="ViewModel"/> that was last navigated to, if available; otherwise, <c>null</c>.</value>
        public virtual ViewModel Current
        {
            get
            {
                CheckDisposed();

                return wrapped.Current;
            }
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
        /// Initializes a new instance of the <see cref="NavigationServiceProxy"/> class.
        /// </summary>
        /// <param name="wrapped">The <see cref="INavigationService"/> that this <see cref="NavigationServiceProxy"/> wraps.</param>
        public NavigationServiceProxy(INavigationService wrapped)
        {
            Requires.NotNull(wrapped, nameof(wrapped));

            this.wrapped = wrapped;

            listener = new EventListener(this);

            Navigation.AddListener(wrapped, listener);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Navigates to the content <see cref="ViewModel"/> that is represented by the specified route.
        /// </summary>
        /// <param name="route">The route of the <see cref="ViewModel"/> to navigate to.</param>
        /// <returns><c>true</c> if successfully navigated; otherwise, <c>false</c>.</returns>
        public virtual bool Navigate(string route)
        {
            CheckDisposed();

            return wrapped.Navigate(route);
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
        /// Throws an exception if the <see cref="NavigationServiceProxy"/> is disposed.
        /// </summary>
        protected void CheckDisposed()
        {
            if (Variable.Equals1(ref disposed))
            {
                throw Error.ObjectDisposed(this);
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="NavigationServiceProxy"/>.
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources used by the <see cref="NavigationServiceProxy"/>.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && Variable.Increment0(ref disposed))
            {
                Navigation.RemoveListener(wrapped, listener);
            }
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents a event listener that delegates the event-handling method.
        /// </summary>
        private class EventListener : EventListener<EventArgs, NavigationServiceProxy>
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="EventListener"/> class.
            /// </summary>
            /// <param name="handler">The event handler object.</param>
            internal EventListener(NavigationServiceProxy handler) : base(handler)
            {
            }

            #endregion

            #region Methods

            /// <summary>
            /// Handles the subscribed event using the specified handler object.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
            /// <param name="handler">The event handler object.</param>
            protected override void HandleEvent(object sender, EventArgs e, NavigationServiceProxy handler)
            {
                NavigatingEventArgs navigating = e as NavigatingEventArgs;
                if (navigating != null)
                {
                    handler.OnNavigating(navigating);

                    return;
                }

                NavigatedEventArgs navigated = e as NavigatedEventArgs;
                if (navigated != null)
                {
                    handler.OnNavigated(navigated);

                    return;
                }
            }

            #endregion
        }

        /// <summary>
        /// Provides a weak event implementation for the navigation events of the <see cref="INavigationService"/>.
        /// </summary>
        private class Navigation : WeakEvent<INavigationService, EventArgs, Navigation>
        {
            #region Methods

            /// <summary>
            /// Adds the specified listener to the specified source object for the events being managed.
            /// </summary>
            /// <param name="source">The source object to add listener to.</param>
            /// <param name="listener">The listener to add.</param>
            internal static void AddListener(INavigationService source, IEventListener<EventArgs> listener)
            {
                ProtectedAddListener(source, listener);
            }

            /// <summary>
            /// Removes the specified listener from the specified source object for the events being managed.
            /// </summary>
            /// <param name="source">The source object to remove listener from.</param>
            /// <param name="listener">The listener to remove.</param>
            internal static void RemoveListener(INavigationService source, IEventListener<EventArgs> listener)
            {
                ProtectedRemoveListener(source, listener);
            }

            /// <summary>
            /// Starts listening on the specified source object for the events being managed.
            /// </summary>
            /// <param name="source">The source object to start listening on.</param>
            protected override void StartListening(INavigationService source)
            {
                source.Navigating += OnEvent;
                source.Navigated += OnEvent;
            }

            /// <summary>
            /// Stops listening on the specified source object for the events being managed.
            /// </summary>
            /// <param name="source">The source object to stop listening on.</param>
            protected override void StopListening(INavigationService source)
            {
                source.Navigating -= OnEvent;
                source.Navigated -= OnEvent;
            }

            #endregion
        }

        #endregion
    }
}
