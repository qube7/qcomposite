using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Qube7.Collections
{
    /// <summary>
    /// Provides the base class for a thread-safe, generic collection.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    /// <threadsafety static="true" instance="true"/>
    [DebuggerDisplay("Count = {Count}")]
    public class SynchronizedCollection<T> : IList<T>
    {
        #region Fields

        /// <summary>
        /// The synchronization object.
        /// </summary>
        private readonly object sync;

        /// <summary>
        /// The wrapped collection.
        /// </summary>
        private readonly IList<T> items;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of elements contained in the <see cref="SynchronizedCollection{T}"/>.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="SynchronizedCollection{T}"/>.</value>
        public int Count
        {
            get
            {
                lock (sync)
                {
                    return items.Count;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="SynchronizedCollection{T}"/> is read-only.
        /// </summary>
        /// <value><c>true</c> if the <see cref="SynchronizedCollection{T}"/> is read-only; otherwise, <c>false</c>.</value>
        bool ICollection<T>.IsReadOnly
        {
            get
            {
                lock (sync)
                {
                    return items.IsReadOnly;
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="IList{T}"/> wrapped by the <see cref="SynchronizedCollection{T}"/>.
        /// </summary>
        /// <value>The <see cref="IList{T}"/> wrapped by the <see cref="SynchronizedCollection{T}"/>.</value>
        protected IList<T> Items
        {
            get { return items; }
        }

        #endregion

        #region Indexers

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <value>The element at the specified index.</value>
        public T this[int index]
        {
            get
            {
                lock (sync)
                {
                    return items[index];
                }
            }
            set
            {
                lock (sync)
                {
                    if (items.IsReadOnly)
                    {
                        throw Error.NotSupported(Strings.ReadOnlyCollection);
                    }

                    if (index < 0 || index >= items.Count)
                    {
                        throw Error.ArgumentOutOfRange(Strings.IndexRangeList, nameof(index));
                    }

                    SetItem(index, value);
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedCollection{T}"/> class as a wrapper for the specified list.
        /// </summary>
        /// <param name="list">The list that is wrapped by the <see cref="SynchronizedCollection{T}"/>.</param>
        public SynchronizedCollection(IList<T> list)
        {
            Requires.NotNull(list, nameof(list));

            ICollection collection = list as ICollection;

            sync = collection?.SyncRoot ?? new object();

            items = list;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedCollection{T}"/> class.
        /// </summary>
        public SynchronizedCollection() : this(new List<T>())
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds an item to the <see cref="SynchronizedCollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="SynchronizedCollection{T}"/>.</param>
        public void Add(T item)
        {
            lock (sync)
            {
                if (items.IsReadOnly)
                {
                    throw Error.NotSupported(Strings.ReadOnlyCollection);
                }

                InsertItem(items.Count, item);
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="SynchronizedCollection{T}"/>.
        /// </summary>
        public void Clear()
        {
            lock (sync)
            {
                if (items.IsReadOnly)
                {
                    throw Error.NotSupported(Strings.ReadOnlyCollection);
                }

                ClearItems();
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="SynchronizedCollection{T}"/>.
        /// </summary>
        protected virtual void ClearItems()
        {
            items.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="SynchronizedCollection{T}"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="SynchronizedCollection{T}"/>.</param>
        /// <returns><c>true</c> if <paramref name="item"/> is found in the <see cref="SynchronizedCollection{T}"/>; otherwise, <c>false</c>.</returns>
        public bool Contains(T item)
        {
            lock (sync)
            {
                return items.Contains(item);
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="SynchronizedCollection{T}"/> to the specified array, starting at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the current <see cref="SynchronizedCollection{T}"/>.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(T[] array, int index)
        {
            lock (sync)
            {
                items.CopyTo(array, index);
            }
        }

        /// <summary>
        /// Determines the index of a specific value in the <see cref="SynchronizedCollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="SynchronizedCollection{T}"/>.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name="item"/> within the entire <see cref="SynchronizedCollection{T}"/>, if found; otherwise, -1.</returns>
        public int IndexOf(T item)
        {
            lock (sync)
            {
                return items.IndexOf(item);
            }
        }

        /// <summary>
        /// Inserts an element into the <see cref="SynchronizedCollection{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="SynchronizedCollection{T}"/>.</param>
        public void Insert(int index, T item)
        {
            lock (sync)
            {
                if (items.IsReadOnly)
                {
                    throw Error.NotSupported(Strings.ReadOnlyCollection);
                }

                if (index < 0 || index > items.Count)
                {
                    throw Error.ArgumentOutOfRange(Strings.IndexRangeListInsert, nameof(index));
                }

                InsertItem(index, item);
            }
        }

        /// <summary>
        /// Inserts an element into the <see cref="SynchronizedCollection{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="SynchronizedCollection{T}"/>.</param>
        protected virtual void InsertItem(int index, T item)
        {
            items.Insert(index, item);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="SynchronizedCollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="SynchronizedCollection{T}"/>.</param>
        /// <returns><c>true</c> if <paramref name="item"/> was successfully removed from the <see cref="SynchronizedCollection{T}"/>; otherwise, <c>false</c>.</returns>
        public bool Remove(T item)
        {
            lock (sync)
            {
                if (items.IsReadOnly)
                {
                    throw Error.NotSupported(Strings.ReadOnlyCollection);
                }

                int index = items.IndexOf(item);

                if (index < 0)
                {
                    return false;
                }

                RemoveItem(index);

                return true;
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="SynchronizedCollection{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            lock (sync)
            {
                if (items.IsReadOnly)
                {
                    throw Error.NotSupported(Strings.ReadOnlyCollection);
                }

                if (index < 0 || index >= items.Count)
                {
                    throw Error.ArgumentOutOfRange(Strings.IndexRangeList, nameof(index));
                }

                RemoveItem(index);
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="SynchronizedCollection{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected virtual void RemoveItem(int index)
        {
            items.RemoveAt(index);
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected virtual void SetItem(int index, T item)
        {
            items[index] = item;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SynchronizedCollection{T}"/>.
        /// </summary>
        /// <returns>A enumerator that can be used to iterate through the <see cref="SynchronizedCollection{T}"/>.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            lock (sync)
            {
                return items.GetEnumerator();
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SynchronizedCollection{T}"/>.
        /// </summary>
        /// <returns>A enumerator that can be used to iterate through the <see cref="SynchronizedCollection{T}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
