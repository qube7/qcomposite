using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Qube7.Collections;
using Qube7.Threading;

namespace Qube7.ComponentModel
{
    /// <summary>
    /// Provides a base class for the weak instance event implementations.
    /// </summary>
    /// <typeparam name="TSource">The type of the source object.</typeparam>
    /// <typeparam name="T">The type of the event data.</typeparam>
    /// <typeparam name="TEvent">The weak event implementing type.</typeparam>
    /// <threadsafety static="true" instance="true"/>
    public abstract class WeakEvent<TSource, T, TEvent> where TSource : class where T : EventArgs where TEvent : WeakEvent<TSource, T, TEvent>, new()
    {
        #region Fields

        /// <summary>
        /// The event handlers table.
        /// </summary>
        private static readonly ConditionalWeakTable<TSource, TEvent> table = new ConditionalWeakTable<TSource, TEvent>();

        /// <summary>
        /// The synchronization lock.
        /// </summary>
        private static readonly Lock sync = new Lock();

        /// <summary>
        /// The subscriptions array.
        /// </summary>
        private volatile Subscription[] items;

        /// <summary>
        /// The source object.
        /// </summary>
        private TSource source;

        /// <summary>
        /// The current collection count.
        /// </summary>
        private int gen1;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the <see cref="WeakEvent{TSource, T, TEvent}"/> is not containing any subscriptions.
        /// </summary>
        /// <value><c>true</c> if the <see cref="WeakEvent{TSource, T, TEvent}"/> is not containing any subscriptions; otherwise, <c>false</c>.</value>
        private bool IsNull
        {
            get { return items.Length == 0; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEvent{TSource, T, TEvent}"/> class.
        /// </summary>
        protected WeakEvent()
        {
            items = Array.Empty<Subscription>();

            gen1 = GC.CollectionCount(1);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified listener to the specified source object for the event being managed.
        /// </summary>
        /// <param name="source">The source object to add listener to.</param>
        /// <param name="listener">The listener to add.</param>
        protected internal static void ProtectedAddListener(TSource source, IEventListener<T> listener)
        {
            Requires.NotNull(source, nameof(source));
            Requires.NotNull(listener, nameof(listener));

            using (sync.Read())
            {
                TEvent handler = table.GetValue(source, Create);

                handler.AddListener(listener);
            }
        }

        /// <summary>
        /// Removes the specified listener from the specified source object for the event being managed.
        /// </summary>
        /// <param name="source">The source object to remove listener from.</param>
        /// <param name="listener">The listener to remove.</param>
        protected internal static void ProtectedRemoveListener(TSource source, IEventListener<T> listener)
        {
            Requires.NotNull(source, nameof(source));
            Requires.NotNull(listener, nameof(listener));

            using (sync.Read())
            {
                if (table.TryGetValue(source, out TEvent handler))
                {
                    handler.RemoveListener(listener);
                }
            }
        }

        /// <summary>
        /// Removes inactive listeners from the specified source object for the event being managed.
        /// </summary>
        /// <param name="source">The source object to remove inactive listeners from.</param>
        private static void Cleanup(TSource source)
        {
            using (sync.Upgradeable())
            {
                if (table.TryGetValue(source, out TEvent handler))
                {
                    if (handler.Purge() > 0)
                    {
                        using (sync.Write())
                        {
                            if (handler.IsNull)
                            {
                                table.Remove(source);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates the handler instance for the specified source object.
        /// </summary>
        /// <param name="source">The source object to create handler for.</param>
        /// <returns>The handler for the <paramref name="source"/>.</returns>
        private static TEvent Create(TSource source)
        {
            TEvent handler = new TEvent();

            handler.source = source;

            return handler;
        }

        /// <summary>
        /// When overridden in a derived class, starts listening on the specified source object for the event being managed.
        /// </summary>
        /// <param name="source">The source object to start listening on.</param>
        protected abstract void StartListening(TSource source);

        /// <summary>
        /// When overridden in a derived class, stops listening on the specified source object for the event being managed.
        /// </summary>
        /// <param name="source">The source object to stop listening on.</param>
        protected abstract void StopListening(TSource source);

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
                    StartListening(source);

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
        /// <returns>The number of listeners removed.</returns>
        private int Purge()
        {
            int num = 0;

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

                            continue;
                        }

                        num++;
                    }

                    items = list.ToArray();

                    if (items.Length == 0)
                    {
                        StopListening(source);
                    }
                }

                gen1 = GC.CollectionCount(1);
            }

            return num;
        }

        /// <summary>
        /// Requests the purge of the inactive listeners.
        /// </summary>
        /// <param name="threshold">The collection count threshold.</param>
        private void ScheduleCleanup(int threshold)
        {
            if (Variable.LessExchange(ref gen1, int.MaxValue, threshold) < threshold)
            {
                Async.Start(Cleanup, source);
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
