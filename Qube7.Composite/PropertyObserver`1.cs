using System;
using System.ComponentModel;
using System.Diagnostics;
using Qube7.Collections;
using Qube7.ComponentModel;
using Qube7.Threading;

namespace Qube7.Composite
{
    /// <summary>
    /// Represents an listener of the property change notification on the given source object.
    /// </summary>
    /// <typeparam name="T">The type of the source object.</typeparam>
    /// <threadsafety static="true" instance="true"/>
    public class PropertyObserver<T> : ISubscriber<T>, IDisposable where T : class, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// The trace category.
        /// </summary>
        private const string TraceCategory = "PropertyObserver";

        /// <summary>
        /// The weak source object.
        /// </summary>
        private readonly Weak<T> weak;

        /// <summary>
        /// The event listener.
        /// </summary>
        private readonly EventListener listener;

        /// <summary>
        /// The subscriptions array.
        /// </summary>
        private volatile Subscription<T>[] items;

        /// <summary>
        /// A value indicating whether the <see cref="PropertyObserver{T}"/> is disposed.
        /// </summary>
        private int disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the source object observed by the current <see cref="PropertyObserver{T}"/>.
        /// </summary>
        /// <value>The source object if accessible; otherwise, <c>null</c>.</value>
        public T Source
        {
            get { return weak.Target; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyObserver{T}"/> class.
        /// </summary>
        /// <param name="source">The source object to observe.</param>
        public PropertyObserver(T source)
        {
            Requires.NotNull(source, nameof(source));

            weak = Weak.Create(source);

            listener = new EventListener(this);

            items = Array.Empty<Subscription<T>>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Subscribes the specified delegate for the change notification of the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property to observe.</param>
        /// <param name="callback">The delegate to subscribe.</param>
        /// <returns>The current subscriber.</returns>
        /// <remarks>The <c>null</c> or <see cref="String.Empty"/> used for the <paramref name="propertyName"/> indicate subscription for notification when all properties on the source object have changed.</remarks>
        public ISubscriber<T> Subscribe(string propertyName, Action<T> callback)
        {
            Requires.NotNull(callback, nameof(callback));

            CheckDisposed();

            T source = weak.Target;
            if (source != null)
            {
                propertyName = propertyName ?? string.Empty;

                ValidateProperty(source, propertyName);

                lock (weak)
                {
                    if (disposed == 0)
                    {
                        items = ArrayHelper.Append(items, Subscription.Create(propertyName, callback));

                        if (items.Length == 1)
                        {
                            PropertyChangedEvent.AddListener(source, listener);
                        }
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Subscribes the specified delegate for the change notification of the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property to observe.</param>
        /// <param name="callback">The delegate to subscribe.</param>
        /// <returns>The current subscriber.</returns>
        /// <remarks>The <c>null</c> or <see cref="String.Empty"/> used for the <paramref name="propertyName"/> indicate subscription for notification when all properties on the source object have changed.</remarks>
        public ISubscriber<T> Subscribe(string propertyName, Action callback)
        {
            Requires.NotNull(callback, nameof(callback));

            CheckDisposed();

            T source = weak.Target;
            if (source != null)
            {
                propertyName = propertyName ?? string.Empty;

                ValidateProperty(source, propertyName);

                lock (weak)
                {
                    if (disposed == 0)
                    {
                        items = ArrayHelper.Append(items, Subscription.Create<T>(propertyName, callback));

                        if (items.Length == 1)
                        {
                            PropertyChangedEvent.AddListener(source, listener);
                        }
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Subscribes the specified delegate for the change notification of any property.
        /// </summary>
        /// <param name="callback">The delegate to subscribe.</param>
        /// <returns>The current subscriber.</returns>
        public ISubscriber<T> Subscribe(Action<T> callback)
        {
            Requires.NotNull(callback, nameof(callback));

            CheckDisposed();

            T source = weak.Target;
            if (source != null)
            {
                lock (weak)
                {
                    if (disposed == 0)
                    {
                        items = ArrayHelper.Append(items, Subscription.Create(callback));

                        if (items.Length == 1)
                        {
                            PropertyChangedEvent.AddListener(source, listener);
                        }
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Subscribes the specified delegate for the change notification of any property.
        /// </summary>
        /// <param name="callback">The delegate to subscribe.</param>
        /// <returns>The current subscriber.</returns>
        public ISubscriber<T> Subscribe(Action callback)
        {
            Requires.NotNull(callback, nameof(callback));

            CheckDisposed();

            T source = weak.Target;
            if (source != null)
            {
                lock (weak)
                {
                    if (disposed == 0)
                    {
                        items = ArrayHelper.Append(items, Subscription.Create<T>(callback));

                        if (items.Length == 1)
                        {
                            PropertyChangedEvent.AddListener(source, listener);
                        }
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Unsubscribes the specified delegate from the change notification of the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the observed property.</param>
        /// <param name="callback">The delegate to unsubscribe.</param>
        /// <returns>The current subscriber.</returns>
        /// <remarks>The <c>null</c> or <see cref="String.Empty"/> used for the <paramref name="propertyName"/> indicate subscription for notification when all properties on the source object have changed.</remarks>
        public ISubscriber<T> Unsubscribe(string propertyName, Action<T> callback)
        {
            Requires.NotNull(callback, nameof(callback));

            CheckDisposed();

            T source = weak.Target;
            if (source != null)
            {
                propertyName = propertyName ?? string.Empty;

                ValidateProperty(source, propertyName);

                lock (weak)
                {
                    Subscription<T>[] array = items;
                    for (int i = array.Length - 1; i >= 0; i--)
                    {
                        if (array[i].Unsubscribe(propertyName, callback))
                        {
                            items = ArrayHelper.RemoveAt(array, i);

                            if (items.Length == 0)
                            {
                                PropertyChangedEvent.RemoveListener(source, listener);
                            }

                            break;
                        }
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Unsubscribes the specified delegate from the change notification of the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the observed property.</param>
        /// <param name="callback">The delegate to unsubscribe.</param>
        /// <returns>The current subscriber.</returns>
        /// <remarks>The <c>null</c> or <see cref="String.Empty"/> used for the <paramref name="propertyName"/> indicate subscription for notification when all properties on the source object have changed.</remarks>
        public ISubscriber<T> Unsubscribe(string propertyName, Action callback)
        {
            Requires.NotNull(callback, nameof(callback));

            CheckDisposed();

            T source = weak.Target;
            if (source != null)
            {
                propertyName = propertyName ?? string.Empty;

                ValidateProperty(source, propertyName);

                lock (weak)
                {
                    Subscription<T>[] array = items;
                    for (int i = array.Length - 1; i >= 0; i--)
                    {
                        if (array[i].Unsubscribe(propertyName, callback))
                        {
                            items = ArrayHelper.RemoveAt(array, i);

                            if (items.Length == 0)
                            {
                                PropertyChangedEvent.RemoveListener(source, listener);
                            }

                            break;
                        }
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Unsubscribes the specified delegate from the change notification of any property.
        /// </summary>
        /// <param name="callback">The delegate to unsubscribe.</param>
        /// <returns>The current subscriber.</returns>
        public ISubscriber<T> Unsubscribe(Action<T> callback)
        {
            Requires.NotNull(callback, nameof(callback));

            CheckDisposed();

            T source = weak.Target;
            if (source != null)
            {
                lock (weak)
                {
                    Subscription<T>[] array = items;
                    for (int i = array.Length - 1; i >= 0; i--)
                    {
                        if (array[i].Unsubscribe(callback))
                        {
                            items = ArrayHelper.RemoveAt(array, i);

                            if (items.Length == 0)
                            {
                                PropertyChangedEvent.RemoveListener(source, listener);
                            }

                            break;
                        }
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Unsubscribes the specified delegate from the change notification of any property.
        /// </summary>
        /// <param name="callback">The delegate to unsubscribe.</param>
        /// <returns>The current subscriber.</returns>
        public ISubscriber<T> Unsubscribe(Action callback)
        {
            Requires.NotNull(callback, nameof(callback));

            CheckDisposed();

            T source = weak.Target;
            if (source != null)
            {
                lock (weak)
                {
                    Subscription<T>[] array = items;
                    for (int i = array.Length - 1; i >= 0; i--)
                    {
                        if (array[i].Unsubscribe(callback))
                        {
                            items = ArrayHelper.RemoveAt(array, i);

                            if (items.Length == 0)
                            {
                                PropertyChangedEvent.RemoveListener(source, listener);
                            }

                            break;
                        }
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Called when any property of the source object was changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="PropertyChangedEventArgs"/> that contains the event data.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            T source = sender as T;
            if (source != null)
            {
                string propertyName = e.PropertyName ?? string.Empty;

                Subscription<T>[] array = items;
                for (int i = 0; i < array.Length; i++)
                {
                    if (Variable.Equals1(ref disposed))
                    {
                        return;
                    }

                    array[i].ProcessEvent(source, propertyName);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified source object declares an instance property with the specified name.
        /// </summary>
        /// <param name="source">The source object to validate against.</param>
        /// <param name="propertyName">The name of the property being tested.</param>
        [Conditional("DEBUG")]
        private void ValidateProperty(object source, string propertyName)
        {
            if (propertyName.Length > 0 && TypeDescriptor.GetProperties(source)[propertyName] == null)
            {
                Trace.WriteLine(Format.Current(Strings.PropertyNotFound, propertyName, source.GetType()), TraceCategory);
            }
        }

        /// <summary>
        /// Throws an exception if the <see cref="PropertyObserver{T}"/> is disposed.
        /// </summary>
        private void CheckDisposed()
        {
            if (Variable.Equals1(ref disposed))
            {
                throw Error.ObjectDisposed(this);
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="PropertyObserver{T}"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources used by the <see cref="PropertyObserver{T}"/>.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && Variable.Increment0(ref disposed))
            {
                lock (weak)
                {
                    T source = weak.Target;
                    if (source != null && items.Length > 0)
                    {
                        PropertyChangedEvent.RemoveListener(source, listener);
                    }

                    items = Array.Empty<Subscription<T>>();
                }

                weak.Dispose();
            }
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents a event listener that delegates the event-handling method.
        /// </summary>
        private class EventListener : EventListener<PropertyChangedEventArgs, PropertyObserver<T>>
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="EventListener"/> class.
            /// </summary>
            /// <param name="handler">The event handler object.</param>
            internal EventListener(PropertyObserver<T> handler) : base(handler)
            {
            }

            #endregion

            #region Methods

            /// <summary>
            /// Handles the subscribed event using the specified handler object.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">An <see cref="PropertyChangedEventArgs"/> that contains the event data.</param>
            /// <param name="handler">The event handler object.</param>
            protected override void HandleEvent(object sender, PropertyChangedEventArgs e, PropertyObserver<T> handler)
            {
                handler.OnPropertyChanged(sender, e);
            }

            #endregion
        }

        #endregion
    }
}
