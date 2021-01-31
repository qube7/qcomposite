using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Qube7.Collections;
using Qube7.ComponentModel;
using Qube7.Threading;

namespace Qube7.Composite
{
    /// <summary>
    /// Represents an listener of the property change notification.
    /// </summary>
    /// <threadsafety static="true" instance="true"/>
    public class PropertyObserver : IDisposable
    {
        #region Fields

        /// <summary>
        /// The trace category.
        /// </summary>
        private const string TraceCategory = "PropertyObserver";

        /// <summary>
        /// The subscribers table.
        /// </summary>
        private readonly ConditionalWeakTable<INotifyPropertyChanged, Subscriber> table;

        /// <summary>
        /// A value indicating whether the <see cref="PropertyObserver"/> is disposed.
        /// </summary>
        private int disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the <see cref="PropertyObserver"/> is disposed.
        /// </summary>
        /// <value><c>true</c> if the <see cref="PropertyObserver"/> is disposed; otherwise, <c>false</c>.</value>
        private bool Disposed
        {
            get { return Variable.Equals1(ref disposed); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyObserver"/> class.
        /// </summary>
        public PropertyObserver()
        {
            table = new ConditionalWeakTable<INotifyPropertyChanged, Subscriber>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new <see cref="PropertyObserver{T}"/> for the specified source object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The source object to observe.</param>
        /// <returns>A <see cref="PropertyObserver{T}"/> for the <paramref name="source"/>.</returns>
        public static PropertyObserver<T> Create<T>(T source) where T : class, INotifyPropertyChanged
        {
            return new PropertyObserver<T>(source);
        }

        /// <summary>
        /// Returns the property change notification subscriber for the specified source object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The source object to observe.</param>
        /// <returns>The subscriber for the <paramref name="source"/>.</returns>
        public ISubscriber<T> Observe<T>(T source) where T : class, INotifyPropertyChanged
        {
            Requires.NotNull(source, nameof(source));

            CheckDisposed();

            return new Subscriber<T>(source, this);
        }

        /// <summary>
        /// Removes from the <see cref="PropertyObserver"/> subscriptions associated with the specified source object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The source object to unobserve.</param>
        public void Unobserve<T>(T source) where T : class, INotifyPropertyChanged
        {
            Requires.NotNull(source, nameof(source));

            CheckDisposed();

            lock (table)
            {
                if (table.TryGetValue(source, out Subscriber subscriber))
                {
                    subscriber.Dispose();

                    PropertyChangedEvent.RemoveListener(source, subscriber);

                    table.Remove(source);
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="Subscriber"/> for the specified source object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The source object.</param>
        /// <returns>A <see cref="Subscriber"/> for the <paramref name="source"/>.</returns>
        private Subscriber CreateObserve<T>(T source) where T : class, INotifyPropertyChanged
        {
            Subscriber subscriber = Subscriber.Create(source, this);

            PropertyChangedEvent.AddListener(source, subscriber);

            return subscriber;
        }

        /// <summary>
        /// Subscribes the specified delegate for the change notification of the specified property of the specified source object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The source object to subscribe to.</param>
        /// <param name="propertyName">The name of the property to observe.</param>
        /// <param name="callback">The delegate to subscribe.</param>
        private void Subscribe<T>(T source, string propertyName, Action<T> callback) where T : class, INotifyPropertyChanged
        {
            ValidateProperty(source, propertyName);

            CheckDisposed();

            lock (table)
            {
                if (disposed == 0)
                {
                    Subscriber subscriber = table.GetValue(source, CreateObserve);

                    subscriber.Subscribe(propertyName, callback);
                }
            }
        }

        /// <summary>
        /// Subscribes the specified delegate for the change notification of the specified property of the specified source object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The source object to subscribe to.</param>
        /// <param name="propertyName">The name of the property to observe.</param>
        /// <param name="callback">The delegate to subscribe.</param>
        private void Subscribe<T>(T source, string propertyName, Action callback) where T : class, INotifyPropertyChanged
        {
            ValidateProperty(source, propertyName);

            CheckDisposed();

            lock (table)
            {
                if (disposed == 0)
                {
                    Subscriber subscriber = table.GetValue(source, CreateObserve);

                    subscriber.Subscribe<T>(propertyName, callback);
                }
            }
        }

        /// <summary>
        /// Subscribes the specified delegate for the change notification of any property of the specified source object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The source object to subscribe to.</param>
        /// <param name="callback">The delegate to subscribe.</param>
        private void Subscribe<T>(T source, Action<T> callback) where T : class, INotifyPropertyChanged
        {
            CheckDisposed();

            lock (table)
            {
                if (disposed == 0)
                {
                    Subscriber subscriber = table.GetValue(source, CreateObserve);

                    subscriber.Subscribe(callback);
                }
            }
        }

        /// <summary>
        /// Subscribes the specified delegate for the change notification of any property of the specified source object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The source object to subscribe to.</param>
        /// <param name="callback">The delegate to subscribe.</param>
        private void Subscribe<T>(T source, Action callback) where T : class, INotifyPropertyChanged
        {
            CheckDisposed();

            lock (table)
            {
                if (disposed == 0)
                {
                    Subscriber subscriber = table.GetValue(source, CreateObserve);

                    subscriber.Subscribe<T>(callback);
                }
            }
        }

        /// <summary>
        /// Unsubscribes the specified delegate from the change notification of the specified property of the specified source object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The source object to unsubscribe from.</param>
        /// <param name="propertyName">The name of the observed property.</param>
        /// <param name="callback">The delegate to unsubscribe.</param>
        private void Unsubscribe<T>(T source, string propertyName, Action<T> callback) where T : class, INotifyPropertyChanged
        {
            ValidateProperty(source, propertyName);

            CheckDisposed();

            lock (table)
            {
                if (table.TryGetValue(source, out Subscriber subscriber))
                {
                    subscriber.Unsubscribe(propertyName, callback);

                    if (subscriber.IsEmpty)
                    {
                        subscriber.Dispose();

                        PropertyChangedEvent.RemoveListener(source, subscriber);

                        table.Remove(source);
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribes the specified delegate from the change notification of the specified property of the specified source object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The source object to unsubscribe from.</param>
        /// <param name="propertyName">The name of the observed property.</param>
        /// <param name="callback">The delegate to unsubscribe.</param>
        private void Unsubscribe<T>(T source, string propertyName, Action callback) where T : class, INotifyPropertyChanged
        {
            ValidateProperty(source, propertyName);

            CheckDisposed();

            lock (table)
            {
                if (table.TryGetValue(source, out Subscriber subscriber))
                {
                    subscriber.Unsubscribe<T>(propertyName, callback);

                    if (subscriber.IsEmpty)
                    {
                        subscriber.Dispose();

                        PropertyChangedEvent.RemoveListener(source, subscriber);

                        table.Remove(source);
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribes the specified delegate from the change notification of any property of the specified source object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The source object to unsubscribe from.</param>
        /// <param name="callback">The delegate to unsubscribe.</param>
        private void Unsubscribe<T>(T source, Action<T> callback) where T : class, INotifyPropertyChanged
        {
            CheckDisposed();

            lock (table)
            {
                if (table.TryGetValue(source, out Subscriber subscriber))
                {
                    subscriber.Unsubscribe(callback);

                    if (subscriber.IsEmpty)
                    {
                        subscriber.Dispose();

                        PropertyChangedEvent.RemoveListener(source, subscriber);

                        table.Remove(source);
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribes the specified delegate from the change notification of any property of the specified source object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The source object to unsubscribe from.</param>
        /// <param name="callback">The delegate to unsubscribe.</param>
        private void Unsubscribe<T>(T source, Action callback) where T : class, INotifyPropertyChanged
        {
            CheckDisposed();

            lock (table)
            {
                if (table.TryGetValue(source, out Subscriber subscriber))
                {
                    subscriber.Unsubscribe<T>(callback);

                    if (subscriber.IsEmpty)
                    {
                        subscriber.Dispose();

                        PropertyChangedEvent.RemoveListener(source, subscriber);

                        table.Remove(source);
                    }
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
        /// Throws an exception if the <see cref="PropertyObserver"/> is disposed.
        /// </summary>
        private void CheckDisposed()
        {
            if (Variable.Equals1(ref disposed))
            {
                throw Error.ObjectDisposed(this);
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="PropertyObserver"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources used by the <see cref="PropertyObserver"/>.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && Variable.Increment0(ref disposed))
            {
                lock (table)
                {
                    foreach (KeyValuePair<INotifyPropertyChanged, Subscriber> pair in table)
                    {
                        Subscriber subscriber = pair.Value;

                        subscriber.Dispose();

                        PropertyChangedEvent.RemoveListener(pair.Key, subscriber);
                    }

                    table.Clear();
                }
            }
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents a subscriber that manages property change notification.
        /// </summary>
        private abstract class Subscriber : IEventListener<PropertyChangedEventArgs>, IDisposable
        {
            #region Fields

            /// <summary>
            /// The underlying observer.
            /// </summary>
            private PropertyObserver observer;

            #endregion

            #region Properties

            /// <summary>
            /// Gets a value indicating whether the <see cref="Subscriber"/> is empty.
            /// </summary>
            /// <value><c>true</c> if the <see cref="Subscriber"/> is empty; otherwise, <c>false</c>.</value>
            internal abstract bool IsEmpty { get; }

            #endregion

            #region Constructors

            /// <summary>
            /// Prevents a default instance of the <see cref="Subscriber"/> class from being created.
            /// </summary>
            private Subscriber()
            {
            }

            #endregion

            #region Methods

            /// <summary>
            /// Creates a new <see cref="Subscriber"/> for the specified source object.
            /// </summary>
            /// <typeparam name="T">The type of the source object.</typeparam>
            /// <param name="source">The source object.</param>
            /// <param name="observer">The underlying observer.</param>
            /// <returns>A <see cref="Subscriber"/> for the <paramref name="source"/>.</returns>
            internal static Subscriber Create<T>(T source, PropertyObserver observer) where T : class, INotifyPropertyChanged
            {
                Type type = source.GetType();

                if (type == typeof(T))
                {
                    return Create<T>(observer);
                }

                return Create(type, observer);
            }

            /// <summary>
            /// Creates a new <see cref="Subscriber"/> for the specified type of the source object.
            /// </summary>
            /// <typeparam name="T">The type of the source object.</typeparam>
            /// <param name="observer">The underlying observer.</param>
            /// <returns>A <see cref="Subscriber"/> for the <typeparamref name="T"/>.</returns>
            private static Subscriber Create<T>(PropertyObserver observer) where T : class, INotifyPropertyChanged
            {
                Subscriber subscriber = new TSubscriber<T>();

                subscriber.observer = observer;

                return subscriber;
            }

            /// <summary>
            /// Creates a new <see cref="Subscriber"/> for the specified type of the source object.
            /// </summary>
            /// <param name="type">The type of the source object.</param>
            /// <param name="observer">The underlying observer.</param>
            /// <returns>A <see cref="Subscriber"/> for the <paramref name="type"/>.</returns>
            private static Subscriber Create(Type type, PropertyObserver observer)
            {
                Subscriber subscriber = (Subscriber)Activator.CreateInstance(typeof(TSubscriber<>).MakeGenericType(type), true);

                subscriber.observer = observer;

                return subscriber;
            }

            /// <summary>
            /// Subscribes the specified delegate for the change notification of the specified property.
            /// </summary>
            /// <typeparam name="T">The type of the source object.</typeparam>
            /// <param name="propertyName">The name of the property to observe.</param>
            /// <param name="callback">The delegate to subscribe.</param>
            /// <returns>The current subscriber.</returns>
            internal ISubscriber<T> Subscribe<T>(string propertyName, Action<T> callback) where T : class, INotifyPropertyChanged
            {
                ISubscriber<T> subscriber = this as ISubscriber<T>;

                subscriber.Subscribe(propertyName, callback);

                return subscriber;
            }

            /// <summary>
            /// Subscribes the specified delegate for the change notification of the specified property.
            /// </summary>
            /// <typeparam name="T">The type of the source object.</typeparam>
            /// <param name="propertyName">The name of the property to observe.</param>
            /// <param name="callback">The delegate to subscribe.</param>
            /// <returns>The current subscriber.</returns>
            internal ISubscriber<T> Subscribe<T>(string propertyName, Action callback) where T : class, INotifyPropertyChanged
            {
                ISubscriber<T> subscriber = this as ISubscriber<T>;

                subscriber.Subscribe(propertyName, callback);

                return subscriber;
            }

            /// <summary>
            /// Subscribes the specified delegate for the change notification of any property.
            /// </summary>
            /// <typeparam name="T">The type of the source object.</typeparam>
            /// <param name="callback">The delegate to subscribe.</param>
            /// <returns>The current subscriber.</returns>
            internal ISubscriber<T> Subscribe<T>(Action<T> callback) where T : class, INotifyPropertyChanged
            {
                ISubscriber<T> subscriber = this as ISubscriber<T>;

                subscriber.Subscribe(callback);

                return subscriber;
            }

            /// <summary>
            /// Subscribes the specified delegate for the change notification of any property.
            /// </summary>
            /// <typeparam name="T">The type of the source object.</typeparam>
            /// <param name="callback">The delegate to subscribe.</param>
            /// <returns>The current subscriber.</returns>
            internal ISubscriber<T> Subscribe<T>(Action callback) where T : class, INotifyPropertyChanged
            {
                ISubscriber<T> subscriber = this as ISubscriber<T>;

                subscriber.Subscribe(callback);

                return subscriber;
            }

            /// <summary>
            /// Unsubscribes the specified delegate from the change notification of the specified property.
            /// </summary>
            /// <typeparam name="T">The type of the source object.</typeparam>
            /// <param name="propertyName">The name of the observed property.</param>
            /// <param name="callback">The delegate to unsubscribe.</param>
            /// <returns>The current subscriber.</returns>
            internal ISubscriber<T> Unsubscribe<T>(string propertyName, Action<T> callback) where T : class, INotifyPropertyChanged
            {
                ISubscriber<T> subscriber = this as ISubscriber<T>;

                subscriber.Unsubscribe(propertyName, callback);

                return subscriber;
            }

            /// <summary>
            /// Unsubscribes the specified delegate from the change notification of the specified property.
            /// </summary>
            /// <typeparam name="T">The type of the source object.</typeparam>
            /// <param name="propertyName">The name of the observed property.</param>
            /// <param name="callback">The delegate to unsubscribe.</param>
            /// <returns>The current subscriber.</returns>
            internal ISubscriber<T> Unsubscribe<T>(string propertyName, Action callback) where T : class, INotifyPropertyChanged
            {
                ISubscriber<T> subscriber = this as ISubscriber<T>;

                subscriber.Unsubscribe(propertyName, callback);

                return subscriber;
            }

            /// <summary>
            /// Unsubscribes the specified delegate from the change notification of any property.
            /// </summary>
            /// <typeparam name="T">The type of the source object.</typeparam>
            /// <param name="callback">The delegate to unsubscribe.</param>
            /// <returns>The current subscriber.</returns>
            internal ISubscriber<T> Unsubscribe<T>(Action<T> callback) where T : class, INotifyPropertyChanged
            {
                ISubscriber<T> subscriber = this as ISubscriber<T>;

                subscriber.Unsubscribe(callback);

                return subscriber;
            }

            /// <summary>
            /// Unsubscribes the specified delegate from the change notification of any property.
            /// </summary>
            /// <typeparam name="T">The type of the source object.</typeparam>
            /// <param name="callback">The delegate to unsubscribe.</param>
            /// <returns>The current subscriber.</returns>
            internal ISubscriber<T> Unsubscribe<T>(Action callback) where T : class, INotifyPropertyChanged
            {
                ISubscriber<T> subscriber = this as ISubscriber<T>;

                subscriber.Unsubscribe(callback);

                return subscriber;
            }

            /// <summary>
            /// Called when the subscribed event occurs.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">An <see cref="PropertyChangedEventArgs"/> that contains the event data.</param>
            public abstract void OnEvent(object sender, PropertyChangedEventArgs e);

            /// <summary>
            /// Releases all resources used by the <see cref="Subscriber"/>.
            /// </summary>
            public abstract void Dispose();

            #endregion

            #region Nested types

            /// <summary>
            /// Represents an implementation of the <see cref="Subscriber"/> class.
            /// </summary>
            /// <typeparam name="T">The type of the source object.</typeparam>
            private class TSubscriber<T> : Subscriber, ISubscriber<T> where T : class, INotifyPropertyChanged
            {
                #region Fields

                /// <summary>
                /// The subscriptions array.
                /// </summary>
                private volatile Subscription<T>[] items;

                /// <summary>
                /// A value indicating whether the <see cref="TSubscriber{T}"/> is disposed.
                /// </summary>
                private volatile bool disposed;

                #endregion

                #region Properties

                /// <summary>
                /// Gets a value indicating whether the <see cref="TSubscriber{T}"/> is empty.
                /// </summary>
                /// <value><c>true</c> if the <see cref="TSubscriber{T}"/> is empty; otherwise, <c>false</c>.</value>
                internal override bool IsEmpty
                {
                    get { return items.Length == 0; }
                }

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="TSubscriber{T}"/> class.
                /// </summary>
                internal TSubscriber()
                {
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
                public ISubscriber<T> Subscribe(string propertyName, Action<T> callback)
                {
                    items = ArrayHelper.Append(items, Subscription.Create(propertyName, callback));

                    return this;
                }

                /// <summary>
                /// Subscribes the specified delegate for the change notification of the specified property.
                /// </summary>
                /// <param name="propertyName">The name of the property to observe.</param>
                /// <param name="callback">The delegate to subscribe.</param>
                /// <returns>The current subscriber.</returns>
                public ISubscriber<T> Subscribe(string propertyName, Action callback)
                {
                    items = ArrayHelper.Append(items, Subscription.Create<T>(propertyName, callback));

                    return this;
                }

                /// <summary>
                /// Subscribes the specified delegate for the change notification of any property.
                /// </summary>
                /// <param name="callback">The delegate to subscribe.</param>
                /// <returns>The current subscriber.</returns>
                public ISubscriber<T> Subscribe(Action<T> callback)
                {
                    items = ArrayHelper.Append(items, Subscription.Create(callback));

                    return this;
                }

                /// <summary>
                /// Subscribes the specified delegate for the change notification of any property.
                /// </summary>
                /// <param name="callback">The delegate to subscribe.</param>
                /// <returns>The current subscriber.</returns>
                public ISubscriber<T> Subscribe(Action callback)
                {
                    items = ArrayHelper.Append(items, Subscription.Create<T>(callback));

                    return this;
                }

                /// <summary>
                /// Unsubscribes the specified delegate from the change notification of the specified property.
                /// </summary>
                /// <param name="propertyName">The name of the observed property.</param>
                /// <param name="callback">The delegate to unsubscribe.</param>
                /// <returns>The current subscriber.</returns>
                public ISubscriber<T> Unsubscribe(string propertyName, Action<T> callback)
                {
                    Subscription<T>[] array = items;
                    for (int i = array.Length - 1; i >= 0; i--)
                    {
                        if (array[i].Unsubscribe(propertyName, callback))
                        {
                            items = ArrayHelper.RemoveAt(array, i);

                            break;
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
                public ISubscriber<T> Unsubscribe(string propertyName, Action callback)
                {
                    Subscription<T>[] array = items;
                    for (int i = array.Length - 1; i >= 0; i--)
                    {
                        if (array[i].Unsubscribe(propertyName, callback))
                        {
                            items = ArrayHelper.RemoveAt(array, i);

                            break;
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
                    Subscription<T>[] array = items;
                    for (int i = array.Length - 1; i >= 0; i--)
                    {
                        if (array[i].Unsubscribe(callback))
                        {
                            items = ArrayHelper.RemoveAt(array, i);

                            break;
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
                    Subscription<T>[] array = items;
                    for (int i = array.Length - 1; i >= 0; i--)
                    {
                        if (array[i].Unsubscribe(callback))
                        {
                            items = ArrayHelper.RemoveAt(array, i);

                            break;
                        }
                    }

                    return this;
                }

                /// <summary>
                /// Called when the subscribed event occurs.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">An <see cref="PropertyChangedEventArgs"/> that contains the event data.</param>
                public override void OnEvent(object sender, PropertyChangedEventArgs e)
                {
                    T source = sender as T;
                    if (source != null)
                    {
                        string propertyName = e.PropertyName ?? string.Empty;

                        Subscription<T>[] array = items;
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (disposed || observer.Disposed)
                            {
                                return;
                            }

                            array[i].ProcessEvent(source, propertyName);
                        }
                    }
                }

                /// <summary>
                /// Releases all resources used by the <see cref="TSubscriber{T}"/>.
                /// </summary>
                public override void Dispose()
                {
                    disposed = true;

                    items = Array.Empty<Subscription<T>>();
                }

                #endregion
            }

            #endregion
        }

        /// <summary>
        /// Represents a subscriber that delegates the subscribe and unsubscribe operations.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        private class Subscriber<T> : ISubscriber<T> where T : class, INotifyPropertyChanged
        {
            #region Fields

            /// <summary>
            /// The source object.
            /// </summary>
            private readonly T source;

            /// <summary>
            /// The underlying observer.
            /// </summary>
            private readonly PropertyObserver observer;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Subscriber{T}"/> class.
            /// </summary>
            /// <param name="source">The source object.</param>
            /// <param name="observer">The underlying observer.</param>
            internal Subscriber(T source, PropertyObserver observer)
            {
                this.source = source;
                this.observer = observer;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Subscribes the specified delegate for the change notification of the specified property.
            /// </summary>
            /// <param name="propertyName">The name of the property to observe.</param>
            /// <param name="callback">The delegate to subscribe.</param>
            /// <returns>The current subscriber.</returns>
            public ISubscriber<T> Subscribe(string propertyName, Action<T> callback)
            {
                Requires.NotNull(callback, nameof(callback));

                observer.Subscribe(source, propertyName ?? string.Empty, callback);

                return this;
            }

            /// <summary>
            /// Subscribes the specified delegate for the change notification of the specified property.
            /// </summary>
            /// <param name="propertyName">The name of the property to observe.</param>
            /// <param name="callback">The delegate to subscribe.</param>
            /// <returns>The current subscriber.</returns>
            public ISubscriber<T> Subscribe(string propertyName, Action callback)
            {
                Requires.NotNull(callback, nameof(callback));

                observer.Subscribe(source, propertyName ?? string.Empty, callback);

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

                observer.Subscribe(source, callback);

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

                observer.Subscribe(source, callback);

                return this;
            }

            /// <summary>
            /// Unsubscribes the specified delegate from the change notification of the specified property.
            /// </summary>
            /// <param name="propertyName">The name of the observed property.</param>
            /// <param name="callback">The delegate to unsubscribe.</param>
            /// <returns>The current subscriber.</returns>
            public ISubscriber<T> Unsubscribe(string propertyName, Action<T> callback)
            {
                Requires.NotNull(callback, nameof(callback));

                observer.Unsubscribe(source, propertyName ?? string.Empty, callback);

                return this;
            }

            /// <summary>
            /// Unsubscribes the specified delegate from the change notification of the specified property.
            /// </summary>
            /// <param name="propertyName">The name of the observed property.</param>
            /// <param name="callback">The delegate to unsubscribe.</param>
            /// <returns>The current subscriber.</returns>
            public ISubscriber<T> Unsubscribe(string propertyName, Action callback)
            {
                Requires.NotNull(callback, nameof(callback));

                observer.Unsubscribe(source, propertyName ?? string.Empty, callback);

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

                observer.Unsubscribe(source, callback);

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

                observer.Unsubscribe(source, callback);

                return this;
            }

            #endregion
        }

        #endregion
    }
}
