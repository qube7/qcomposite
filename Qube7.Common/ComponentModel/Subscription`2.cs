using System;
using System.Threading;

namespace Qube7.ComponentModel
{
    /// <summary>
    /// Represents a cancelable <see cref="WeakEvent{T, TEvent}"/> subscription.
    /// </summary>
    /// <typeparam name="T">The type of the event data.</typeparam>
    /// <typeparam name="TEvent">The weak event implementing type.</typeparam>
    /// <threadsafety static="true" instance="true"/>
    public abstract class Subscription<T, TEvent> : IDisposable where T : EventArgs where TEvent : WeakEvent<T, TEvent>, new()
    {
        #region Fields

        /// <summary>
        /// The event listener.
        /// </summary>
        private IEventListener<T> listener;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription{T, TEvent}"/> class.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        protected Subscription(IEventListener<T> listener)
        {
            AddListener(listener);

            this.listener = listener;
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the <see cref="Subscription{T, TEvent}"/> is reclaimed by garbage collection.
        /// </summary>
        ~Subscription()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified listener for the event being managed.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        private static void AddListener(IEventListener<T> listener)
        {
            WeakEvent<T, TEvent>.ProtectedAddListener(listener);
        }

        /// <summary>
        /// Removes the specified listener for the event being managed.
        /// </summary>
        /// <param name="listener">The listener to remove.</param>
        private static void RemoveListener(IEventListener<T> listener)
        {
            WeakEvent<T, TEvent>.ProtectedRemoveListener(listener);
        }

        /// <summary>
        /// Discards current subscription and releases all resources used by the <see cref="Subscription{T, TEvent}"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Discards current subscription and releases resources used by the <see cref="Subscription{T, TEvent}"/>.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            IEventListener<T> current = Interlocked.Exchange(ref listener, null);
            if (current != null)
            {
                RemoveListener(current);
            }
        }

        #endregion
    }
}
