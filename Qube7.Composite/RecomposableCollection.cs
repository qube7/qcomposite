using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Qube7.Collections;

namespace Qube7.Composite
{
    /// <summary>
    /// Represents a collection that supports recomposition update.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    public class RecomposableCollection<T> : ReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// The underlying collection.
        /// </summary>
        private readonly ObservableCollection<T> collection;

        /// <summary>
        /// The equality comparer.
        /// </summary>
        private readonly IEqualityComparer<T> comparer;

        /// <summary>
        /// The collection changed event handler.
        /// </summary>
        private NotifyCollectionChangedEventHandler collectionChanged;

        /// <summary>
        /// The property changed event handler.
        /// </summary>
        private PropertyChangedEventHandler propertyChanged;

        /// <summary>
        /// The current recomposition.
        /// </summary>
        private Recomposition recomposition;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the collection recomposes.
        /// </summary>
        public event EventHandler<RecomposedEventArgs<T>> Recomposed;

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
        /// Initializes a new instance of the <see cref="RecomposableCollection{T}"/> class.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing elements, or <c>null</c> to use the default equality comparer.</param>
        /// <param name="collection">The underlying collection.</param>
        private RecomposableCollection(IEqualityComparer<T> comparer, ObservableCollection<T> collection) : base(collection)
        {
            this.comparer = comparer ?? EqualityComparer<T>.Default;

            this.collection = collection;

            collection.CollectionChanged += HandleCollectionChanged;

            ((INotifyPropertyChanged)collection).PropertyChanged += HandlePropertyChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecomposableCollection{T}"/> class.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing elements, or <c>null</c> to use the default equality comparer.</param>
        public RecomposableCollection(IEqualityComparer<T> comparer) : this(comparer, new ObservableCollection<T>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecomposableCollection{T}"/> class.
        /// </summary>
        public RecomposableCollection() : this(null, new ObservableCollection<T>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecomposableCollection{T}"/> class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing elements, or <c>null</c> to use the default equality comparer.</param>
        public RecomposableCollection(IEnumerable<T> collection, IEqualityComparer<T> comparer) : this(comparer, new ObservableCollection<T>(collection))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecomposableCollection{T}"/> class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param>
        public RecomposableCollection(IEnumerable<T> collection) : this(null, new ObservableCollection<T>(collection))
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Synchronizes the <see cref="RecomposableCollection{T}"/> with the elements in the specified sequence.
        /// </summary>
        /// <param name="source">The sequence of replacement elements.</param>
        public void Recompose(IEnumerable<T> source)
        {
            Requires.NotNull(source, nameof(source));

            T[] array = source.ToArray();

            using (Recomposition recomposition = new Recomposition(this))
            {
                if (array.Length == 0)
                {
                    if (collection.Count > 0)
                    {
                        T[] items = collection.ToArray();

                        collection.Clear();

                        recomposition.Dispose();

                        OnRecomposed(new RecomposedEventArgs<T>(Array.Empty<T>(), Array.AsReadOnly(items)));
                    }

                    return;
                }

                List<T> list = new List<T>(array);

                List<int> indexes = new List<int>();

                for (int i = 0; i < collection.Count; i++)
                {
                    if (list.Count > 0)
                    {
                        int index = list.IndexOf(collection[i], comparer);
                        if (index < 0)
                        {
                            indexes.Add(i);

                            continue;
                        }

                        list.RemoveAt(index);

                        continue;
                    }

                    indexes.Add(i);
                }

                if (indexes.Count == collection.Count)
                {
                    T[] items = Array.Empty<T>();

                    if (collection.Count > 0)
                    {
                        items = collection.ToArray();

                        collection.Clear();
                    }

                    for (int i = 0; i < array.Length; i++)
                    {
                        collection.Add(array[i]);
                    }

                    recomposition.Dispose();

                    OnRecomposed(new RecomposedEventArgs<T>(Array.AsReadOnly(array), Array.AsReadOnly(items)));

                    return;
                }

                List<T> added = new List<T>();
                List<T> removed = new List<T>();

                for (int i = indexes.Count - 1; i >= 0; i--)
                {
                    T item = collection[indexes[i]];

                    collection.RemoveAt(indexes[i]);

                    removed.Insert(0, item);
                }

                for (int i = 0; i < array.Length; i++)
                {
                    if (i < collection.Count)
                    {
                        if (comparer.Equals(array[i], collection[i]))
                        {
                            continue;
                        }

                        if (i < collection.Count - 1)
                        {
                            int index = collection.IndexOf(array[i], i + 1, comparer);
                            if (index > 0)
                            {
                                collection.Move(index, i);

                                continue;
                            }
                        }

                        collection.Insert(i, array[i]);

                        added.Add(array[i]);

                        continue;
                    }

                    collection.Add(array[i]);

                    added.Add(array[i]);
                }

                while (collection.Count > array.Length)
                {
                    T item = collection[array.Length];

                    collection.RemoveAt(array.Length);

                    removed.Add(item);
                }

                if (added.Count > 0 || removed.Count > 0)
                {
                    recomposition.Dispose();

                    OnRecomposed(new RecomposedEventArgs<T>(added.AsReadOnly(), removed.AsReadOnly()));
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="Recomposed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="RecomposedEventArgs{T}"/> instance containing the event data.</param>
        protected virtual void OnRecomposed(RecomposedEventArgs<T> e)
        {
            Event.Raise(Recomposed, this, e);
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
        /// Represents an object that synchronizes the recomposition updates.
        /// </summary>
        private class Recomposition : IDisposable
        {
            #region Fields

            /// <summary>
            /// The underlying collection.
            /// </summary>
            private RecomposableCollection<T> collection;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Recomposition"/> class.
            /// </summary>
            /// <param name="collection">The underlying collection.</param>
            internal Recomposition(RecomposableCollection<T> collection)
            {
                this.collection = collection;

                if (Interlocked.CompareExchange(ref collection.recomposition, this, null) != null)
                {
                    throw new InvalidOperationException(Strings.CollectionRecomposing);
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Exits the current recomposition update.
            /// </summary>
            public void Dispose()
            {
                if (collection != null)
                {
                    Interlocked.Exchange(ref collection.recomposition, null);

                    collection = null;
                }
            }

            #endregion
        }

        #endregion
    }
}
