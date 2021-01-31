using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Qube7.ComponentModel
{
    /// <summary>
    /// Represents a cancelable <see cref="WeakEvent{TSource, T, TEvent}"/> subscription.
    /// </summary>
    /// <typeparam name="TSource">The type of the source object.</typeparam>
    /// <typeparam name="T">The type of the event data.</typeparam>
    /// <typeparam name="TEvent">The weak event implementing type.</typeparam>
    /// <threadsafety static="true" instance="true"/>
    public abstract class Subscription<TSource, T, TEvent> : IDisposable where TSource : class where T : EventArgs where TEvent : WeakEvent<TSource, T, TEvent>, new()
    {
        #region Fields

        /// <summary>
        /// The event listener.
        /// </summary>
        private IEventListener<T> listener;

        /// <summary>
        /// The source object handle.
        /// </summary>
        private GCHandle handle;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the source object referenced by the current <see cref="Subscription{TSource, T, TEvent}"/>.
        /// </summary>
        /// <value>The source object referenced by the current <see cref="Subscription{TSource, T, TEvent}"/>, if accessible; otherwise, <c>null</c>.</value>
        public TSource Source
        {
            get
            {
                try
                {
                    return handle.Target as TSource;
                }
                catch (InvalidOperationException)
                {
                    throw Error.ObjectDisposed(this);
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription{TSource, T, TEvent}"/> class.
        /// </summary>
        /// <param name="source">The source object to add listener to.</param>
        /// <param name="listener">The listener to add.</param>
        protected Subscription(TSource source, IEventListener<T> listener)
        {
            AddListener(source, listener);

            this.listener = listener;

            handle = GCHandle.Alloc(source, GCHandleType.Weak);
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the <see cref="Subscription{TSource, T, TEvent}"/> is reclaimed by garbage collection.
        /// </summary>
        ~Subscription()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified listener to the specified source object for the event being managed.
        /// </summary>
        /// <param name="source">The source object to add listener to.</param>
        /// <param name="listener">The listener to add.</param>
        private static void AddListener(TSource source, IEventListener<T> listener)
        {
            WeakEvent<TSource, T, TEvent>.ProtectedAddListener(source, listener);
        }

        /// <summary>
        /// Removes the specified listener from the specified source object for the event being managed.
        /// </summary>
        /// <param name="source">The source object to remove listener from.</param>
        /// <param name="listener">The listener to remove.</param>
        private static void RemoveListener(TSource source, IEventListener<T> listener)
        {
            WeakEvent<TSource, T, TEvent>.ProtectedRemoveListener(source, listener);
        }

        /// <summary>
        /// Discards current subscription and releases all resources used by the <see cref="Subscription{TSource, T, TEvent}"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Discards current subscription and releases resources used by the <see cref="Subscription{TSource, T, TEvent}"/>.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            IEventListener<T> current = Interlocked.Exchange(ref listener, null);
            if (current != null)
            {
                TSource source = handle.Target as TSource;

                handle.Free();

                if (source != null)
                {
                    RemoveListener(source, current);
                }
            }
        }

        #endregion
    }
}
