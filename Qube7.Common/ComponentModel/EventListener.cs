using System;

namespace Qube7.ComponentModel
{
    /// <summary>
    /// Provides helpers for the <see cref="IEventListener{T}"/>.
    /// </summary>
    public static class EventListener
    {
        #region Methods

        /// <summary>
        /// Returns a <see cref="IEventListener{T}"/> wrapper for the specified handler delegate.
        /// </summary>
        /// <param name="handler">The handler delegate to wrap.</param>
        /// <returns>A <see cref="IEventListener{T}"/> wrapper.</returns>
        public static IEventListener<EventArgs> FromHandler(EventHandler handler)
        {
            Requires.NotNull(handler, nameof(handler));

            return new DelegateListener(handler);
        }

        /// <summary>
        /// Returns a <see cref="IEventListener{T}"/> wrapper for the specified handler delegate.
        /// </summary>
        /// <typeparam name="T">The type of the event data.</typeparam>
        /// <param name="handler">The handler delegate to wrap.</param>
        /// <returns>A <see cref="IEventListener{T}"/> wrapper.</returns>
        public static IEventListener<T> FromHandler<T>(EventHandler<T> handler) where T : EventArgs
        {
            Requires.NotNull(handler, nameof(handler));

            return new DelegateListener<T>(handler);
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents a event listener that delegates the event-handling method.
        /// </summary>
        private class DelegateListener : EventListener<EventArgs, EventHandler>
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="DelegateListener"/> class.
            /// </summary>
            /// <param name="handler">The event handler delegate.</param>
            internal DelegateListener(EventHandler handler) : base(handler)
            {
            }

            #endregion

            #region Methods

            /// <summary>
            /// Handles the subscribed event using the specified handler delegate.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
            /// <param name="handler">The event handler delegate.</param>
            protected override void HandleEvent(object sender, EventArgs e, EventHandler handler)
            {
                handler(sender, e);
            }

            #endregion
        }

        /// <summary>
        /// Represents a event listener that delegates the event-handling method.
        /// </summary>
        /// <typeparam name="T">The type of the event data.</typeparam>
        private class DelegateListener<T> : EventListener<T, EventHandler<T>> where T : EventArgs
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="DelegateListener{T}"/> class.
            /// </summary>
            /// <param name="handler">The event handler delegate.</param>
            internal DelegateListener(EventHandler<T> handler) : base(handler)
            {
            }

            #endregion

            #region Methods

            /// <summary>
            /// Handles the subscribed event using the specified handler delegate.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">An <typeparamref name="T"/> that contains the event data.</param>
            /// <param name="handler">The event handler delegate.</param>
            protected override void HandleEvent(object sender, T e, EventHandler<T> handler)
            {
                handler(sender, e);
            }

            #endregion
        }

        #endregion
    }
}
