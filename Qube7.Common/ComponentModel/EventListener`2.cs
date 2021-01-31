using System;

namespace Qube7.ComponentModel
{
    /// <summary>
    /// Represents a event listener that delegates the event-handling method.
    /// </summary>
    /// <typeparam name="T">The type of the event data.</typeparam>
    /// <typeparam name="THandler">The type of the event handler object.</typeparam>
    public abstract class EventListener<T, THandler> : IEventListener<T> where T : EventArgs
    {
        #region Fields

        /// <summary>
        /// The event handler object.
        /// </summary>
        private readonly THandler handler;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EventListener{T, THandler}"/> class.
        /// </summary>
        /// <param name="handler">The event handler object.</param>
        protected EventListener(THandler handler)
        {
            this.handler = handler;
        }

        #endregion

        #region Methods

        /// <summary>
        /// When overridden in a derived class, handles the subscribed event using the specified handler object.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <typeparamref name="T"/> that contains the event data.</param>
        /// <param name="handler">The event handler object.</param>
        protected abstract void HandleEvent(object sender, T e, THandler handler);

        /// <summary>
        /// Called when the subscribed event occurs.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <typeparamref name="T"/> that contains the event data.</param>
        void IEventListener<T>.OnEvent(object sender, T e)
        {
            HandleEvent(sender, e, handler);
        }

        #endregion
    }
}
