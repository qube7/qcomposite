using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Qube7.Collections;
using Qube7.Threading;

namespace Qube7.ComponentModel
{
    /// <summary>
    /// Provides a base class for the weak static event implementations.
    /// </summary>
    /// <typeparam name="T">The type of the event data.</typeparam>
    /// <typeparam name="TEvent">The weak event implementing type.</typeparam>
    /// <threadsafety static="true" instance="true"/>
    public abstract class WeakEvent<T, TEvent> where TEvent : WeakEvent<T, TEvent>, new()
    {
        #region Fields

        /// <summary>
        /// The event handler instance.
        /// </summary>
        private static readonly TEvent handler = new TEvent();

        /// <summary>
        /// The subscriptions array.
        /// </summary>
        private volatile Subscription[] items;

        /// <summary>
        /// The current collection count.
        /// </summary>
        private int gen1;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEvent{T, TEvent}"/> class.
        /// </summary>
        protected WeakEvent()
        {
            items = Array.Empty<Subscription>();

            gen1 = GC.CollectionCount(1);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified listener for the event being managed.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        protected internal static void ProtectedAddListener(IEventListener<T> listener)
        {
            Requires.NotNull(listener, nameof(listener));

            handler.AddListener(listener);
        }

        /// <summary>
        /// Removes the specified listener for the event being managed.
        /// </summary>
        /// <param name="listener">The listener to remove.</param>
        protected internal static void ProtectedRemoveListener(IEventListener<T> listener)
        {
            Requires.NotNull(listener, nameof(listener));

            handler.RemoveListener(listener);
        }

        /// <summary>
        /// When overridden in a derived class, starts listening for the event being managed.
        /// </summary>
        protected abstract void StartListening();

        /// <summary>
        /// When overridden in a derived class, stops listening for the event being managed.
        /// </summary>
        protected abstract void StopListening();

        /// <summary>
        /// Called when the event being managed occurs.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <typeparamref name="T"/> that contains the event data.</param>
        protected void OnEvent(object sender, T e)
        {
            bool cleanup = false;

            Subscription[] array = items;
            for (int i = 0; i < array.Length; i++)
            {
                IEventListener<T> listener = array[i].Target;
                if (listener != null)
                {
                    listener.OnEvent(sender, e);

                    continue;
                }

                cleanup = true;
            }

            if (cleanup)
            {
                ScheduleCleanup(int.MaxValue);
            }
        }

        /// <summary>
        /// Adds the specified listener for the event being managed.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        private void AddListener(IEventListener<T> listener)
        {
            lock (this)
            {
                items = ArrayHelper.Append(items, new Subscription(listener));

                if (items.Length == 1)
                {
                    StartListening();

                    return;
                }
            }

            ScheduleCleanup(GC.CollectionCount(1));
        }

        /// <summary>
        /// Removes the specified listener for the event being managed.
        /// </summary>
        /// <param name="listener">The listener to remove.</param>
        private void RemoveListener(IEventListener<T> listener)
        {
            lock (this)
            {
                Subscription[] array = items;
                for (int i = array.Length - 1; i >= 0; i--)
                {
                    if (array[i].Target == listener)
                    {
                        array[i].Target = null;

                        ScheduleCleanup(int.MaxValue);

                        return;
                    }
                }
            }

            ScheduleCleanup(GC.CollectionCount(1));
        }

        /// <summary>
        /// Removes inactive listeners for the event being managed.
        /// </summary>
        private void Purge()
        {
            lock (this)
            {
                Subscription[] array = items;
                if (array.Length > 0)
                {
                    List<Subscription> list = new List<Subscription>();

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i].Target != null)
                        {
                            list.Add(array[i]);
                        }
                    }

                    items = list.ToArray();

                    if (items.Length == 0)
                    {
                        StopListening();
                    }
                }

                gen1 = GC.CollectionCount(1);
            }
        }

        /// <summary>
        /// Requests the purge of the inactive listeners.
        /// </summary>
        /// <param name="threshold">The collection count threshold.</param>
        private void ScheduleCleanup(int threshold)
        {
            if (ValueSafe.LessExchange(ref gen1, int.MaxValue, threshold) < threshold)
            {
                Async.Start(Purge);
            }
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents a weak reference to the target listener.
        /// </summary>
        private class Subscription
        {
            #region Fields

            /// <summary>
            /// The target listener handle.
            /// </summary>
            private GCHandle handle;

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets the listener referenced by the current <see cref="Subscription"/>.
            /// </summary>
            /// <value>The target listener, if accessible; otherwise, <c>null</c>.</value>
            internal IEventListener<T> Target
            {
                get { return handle.Target as IEventListener<T>; }
                set { handle.Target = value; }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Subscription"/> class.
            /// </summary>
            /// <param name="listener">The target listener.</param>
            internal Subscription(IEventListener<T> listener)
            {
                handle = GCHandle.Alloc(listener, GCHandleType.Weak);
            }

            #endregion

            #region Destructor

            /// <summary>
            /// Releases unmanaged resources and performs other cleanup operations before the <see cref="Subscription"/> is reclaimed by garbage collection.
            /// </summary>
            ~Subscription()
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }

            #endregion
        }

        #endregion
    }
}
