using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Represents a collection of navigable objects and performs navigation between those objects.
    /// </summary>
    /// <typeparam name="T">The type of the navigable objects in the collection.</typeparam>
    /// <typeparam name="TMetadata">The type of the metadata of the navigable objects.</typeparam>
    public class NavigableCollection<T, TMetadata> : ReadOnlyCollection<Lazy<T, TMetadata>>, INotifyCollectionChanged, INotifyPropertyChanged where T : class where TMetadata : INavigationMetadata
    {
        #region Fields

        /// <summary>
        /// The cached instance of <see cref="PropertyChangedEventArgs"/> indicating that the <see cref="Current"/> property has changed.
        /// </summary>
        private static readonly PropertyChangedEventArgs CurrentPropertyChanged = new PropertyChangedEventArgs(nameof(Current));

        /// <summary>
        /// The value indicating the initial navigation state.
        /// </summary>
        private static readonly Tuple<T, string> UnsetTuple = new Tuple<T, string>(null, null);

        /// <summary>
        /// The underlying collection.
        /// </summary>
        private readonly ObservableCollection<Lazy<T, TMetadata>> collection;

        /// <summary>
        /// The name of the encapsulated region.
        /// </summary>
        private readonly string regionName;

        /// <summary>
        /// The collection changed event handler.
        /// </summary>
        private NotifyCollectionChangedEventHandler collectionChanged;

        /// <summary>
        /// The property changed event handler.
        /// </summary>
        private PropertyChangedEventHandler propertyChanged;

        /// <summary>
        /// The current navigable.
        /// </summary>
        private Tuple<T, string> current = UnsetTuple;

        /// <summary>
        /// The current operation.
        /// </summary>
        private SynchronizedOperation operation;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the encapsulated region.
        /// </summary>
        /// <value>The name of the encapsulated region.</value>
        public string RegionName
        {
            get { return regionName; }
        }

        /// <summary>
        /// Gets the object in the collection that was last navigated to.
        /// </summary>
        /// <value>The object that was last navigated to, if available; otherwise, <c>null</c>.</value>
        public T Current
        {
            get { return Volatile.Read(ref current).Item1; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the object that was last navigated to has changed.
        /// </summary>
        public event EventHandler CurrentChanged;

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add { Event.Subscribe(ref collectionChanged, value); }
            remove { Event.Unsubscribe(ref collectionChanged, value); }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { Event.Subscribe(ref propertyChanged, value); }
            remove { Event.Unsubscribe(ref propertyChanged, value); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigableCollection{T, TMetadata}"/> class.
        /// </summary>
        /// <param name="regionName">The name of the encapsulated region.</param>
        /// <param name="collection">The underlying collection.</param>
        private NavigableCollection(string regionName, ObservableCollection<Lazy<T, TMetadata>> collection) : base(collection)
        {
            Requires.NotNullOrEmpty(regionName, nameof(regionName));

            this.regionName = regionName;

            this.collection = collection;

            collection.CollectionChanged += HandleCollectionChanged;

            ((INotifyPropertyChanged)collection).PropertyChanged += HandlePropertyChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigableCollection{T, TMetadata}"/> class.
        /// </summary>
        /// <param name="regionName">The name of the encapsulated region.</param>
        public NavigableCollection(string regionName) : this(regionName, new ObservableCollection<Lazy<T, TMetadata>>())
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Replaces elements of <see cref="NavigableCollection{T, TMetadata}"/> with the elements in the specified sequence.
        /// </summary>
        /// <param name="source">The sequence of replacement elements.</param>
        public void Recompose(IEnumerable<Lazy<T, TMetadata>> source)
        {
            Requires.NotNull(source, nameof(source));

            Lazy<T, TMetadata>[] array = source.ToArray();

            using (Recomposition recomposition = new Recomposition(this))
            {
                if (collection.Count > 0)
                {
                    collection.Clear();
                }

                for (int i = 0; i < array.Length; i++)
                {
                    collection.Add(array[i]);
                }

                if (current != UnsetTuple)
                {
                    T obj = current.Item1;

                    foreach (Lazy<T, TMetadata> item in collection)
                    {
                        if (item != null)
                        {
                            TMetadata metadata = item.Metadata;

                            if (metadata != null && metadata.Route == current.Item2 && ReferenceEquals(item.Value, obj))
                            {
                                return;
                            }
                        }
                    }

                    current = UnsetTuple;

                    if (obj != null)
                    {
                        recomposition.Dispose();

                        OnCurrentChanged();
                        OnPropertyChanged(CurrentPropertyChanged);

                        NavigatedFrom(obj);
                    }
                }
            }
        }

        /// <summary>
        /// Navigates to the object that is represented by the specified route.
        /// </summary>
        /// <param name="route">The route of the object to navigate to.</param>
        /// <param name="data">The data to pass with the navigation request.</param>
        /// <returns><c>true</c> if successfully navigated; otherwise, <c>false</c>.</returns>
        public bool Navigate(string route, object data)
        {
            using (Navigation navigation = new Navigation(this))
            {
                T obj = current.Item1;

                if (current.Item2 == route && NavigatingTo(obj, data))
                {
                    navigation.Dispose();

                    NavigatedTo(obj, data);

                    return true;
                }

                foreach (Lazy<T, TMetadata> item in collection)
                {
                    if (item != null)
                    {
                        TMetadata metadata = item.Metadata;

                        if (metadata != null && metadata.Route == route)
                        {
                            T target = item.Value;

                            if (ReferenceEquals(target, obj))
                            {
                                if (current.Item2 == route)
                                {
                                    continue;
                                }

                                if (NavigatingTo(obj, data))
                                {
                                    navigation.Dispose();

                                    NavigatedTo(obj, data);

                                    return true;
                                }
                            }

                            if (NavigatingTo(target, data))
                            {
                                if (NavigatingFrom(obj))
                                {
                                    current = Tuple.Create(target, route);

                                    navigation.Dispose();

                                    NavigatedTo(target, data);

                                    OnCurrentChanged();
                                    OnPropertyChanged(CurrentPropertyChanged);

                                    NavigatedFrom(obj);

                                    return true;
                                }

                                return false;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Called when the navigation to the specified object is requested.
        /// </summary>
        /// <param name="target">The object that is the destination for the navigation request.</param>
        /// <param name="data">The data passed with the navigation request.</param>
        /// <returns><c>true</c> if the <paramref name="target"/> accepts the navigation request; otherwise, <c>false</c>.</returns>
        private bool NavigatingTo(T target, object data)
        {
            if (target is INavigable navigable)
            {
                return navigable.NavigatingTo(regionName, data);
            }

            return true;
        }

        /// <summary>
        /// Called when the navigation to the specified object has completed.
        /// </summary>
        /// <param name="target">The object that is the destination for the navigation request.</param>
        /// <param name="data">The data passed with the navigation request.</param>
        private void NavigatedTo(T target, object data)
        {
            if (target is INavigable navigable)
            {
                navigable.NavigatedTo(regionName, data);
            }
        }

        /// <summary>
        /// Called when the navigation from the specified object is requested.
        /// </summary>
        /// <param name="source">The object that is the origin for the navigation request.</param>
        /// <returns><c>true</c> if the <paramref name="source"/> accepts the navigation request; otherwise, <c>false</c>.</returns>
        private bool NavigatingFrom(T source)
        {
            if (source is INavigable navigable)
            {
                return navigable.NavigatingFrom(regionName);
            }

            return true;
        }

        /// <summary>
        /// Called when the navigation from the specified object has completed.
        /// </summary>
        /// <param name="source">The object that is the origin for the navigation request.</param>
        private void NavigatedFrom(T source)
        {
            if (source is INavigable navigable)
            {
                navigable.NavigatedFrom(regionName);
            }
        }

        /// <summary>
        /// Raises the <see cref="CurrentChanged"/> event.
        /// </summary>
        protected virtual void OnCurrentChanged()
        {
            Event.Raise(CurrentChanged, this);
        }

        /// <summary>
        /// Called when the collection changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(e);
        }

        /// <summary>
        /// Called when a property value changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="INotifyCollectionChanged.CollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            Event.Raise(collectionChanged, this, e);
        }

        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            Event.Raise(propertyChanged, this, e);
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents an object that synchronizes the update operations.
        /// </summary>
        private abstract class SynchronizedOperation : IDisposable
        {
            #region Fields

            /// <summary>
            /// The underlying collection.
            /// </summary>
            private NavigableCollection<T, TMetadata> collection;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="SynchronizedOperation"/> class.
            /// </summary>
            /// <param name="collection">The underlying collection.</param>
            protected SynchronizedOperation(NavigableCollection<T, TMetadata> collection)
            {
                this.collection = collection;

                Interlocked.CompareExchange(ref collection.operation, this, null)?.Throw();
            }

            #endregion

            #region Methods

            /// <summary>
            /// Throws an exception indicating that the current operation is in progress.
            /// </summary>
            protected abstract void Throw();

            /// <summary>
            /// Exits the current update operation.
            /// </summary>
            public void Dispose()
            {
                if (collection != null)
                {
                    Interlocked.Exchange(ref collection.operation, null);

                    collection = null;
                }
            }

            #endregion
        }

        /// <summary>
        /// Represents an object that synchronizes the recomposition updates.
        /// </summary>
        private class Recomposition : SynchronizedOperation
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Recomposition"/> class.
            /// </summary>
            /// <param name="collection">The underlying collection.</param>
            internal Recomposition(NavigableCollection<T, TMetadata> collection) : base(collection)
            {
            }

            #endregion

            #region Methods

            /// <summary>
            /// Throws an exception indicating that the recomposition update is in progress.
            /// </summary>
            protected override void Throw()
            {
                throw new InvalidOperationException(Strings.CollectionRecomposing);
            }

            #endregion
        }

        /// <summary>
        /// Represents an object that synchronizes the navigation operations.
        /// </summary>
        private class Navigation : SynchronizedOperation
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Navigation"/> class.
            /// </summary>
            /// <param name="collection">The underlying collection.</param>
            internal Navigation(NavigableCollection<T, TMetadata> collection) : base(collection)
            {
            }

            #endregion

            #region Methods

            /// <summary>
            /// Throws an exception indicating that the navigation operation is in progress.
            /// </summary>
            protected override void Throw()
            {
                throw new InvalidOperationException(Strings.CollectionNavigating);
            }

            #endregion
        }

        #endregion
    }
}
