using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Qube7.Collections
{
    /// <summary>
    /// Represents a collection of weakly referenced objects.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public class WeakCollection<T> : ICollection<T>, IDisposable where T : class
    {
        #region Fields

        /// <summary>
        /// The default capacity.
        /// </summary>
        private const int DefaultCapacity = 4;

        /// <summary>
        /// The weak items array.
        /// </summary>
        private Weak<T>[] items;

        /// <summary>
        /// The total number of elements.
        /// </summary>
        private int size;

        /// <summary>
        /// The current version.
        /// </summary>
        private int version;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of elements contained in the <see cref="WeakCollection{T}"/>.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="WeakCollection{T}"/>.</value>
        public int Count
        {
            get { return size; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="WeakCollection{T}"/> is read-only.
        /// </summary>
        /// <value><c>false</c> indicating that the <see cref="WeakCollection{T}"/> is not read-only.</value>
        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a sequence containing alive elements of the <see cref="WeakCollection{T}"/>.
        /// </summary>
        /// <value>A sequence containing alive elements of the <see cref="WeakCollection{T}"/>.</value>
        public IEnumerable<T> AliveItems
        {
            get
            {
                foreach (T item in this)
                {
                    if (item != null)
                    {
                        yield return item;
                    }
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakCollection{T}"/> class.
        /// </summary>
        public WeakCollection()
        {
            items = Array.Empty<Weak<T>>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds an item to the <see cref="WeakCollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="WeakCollection{T}"/>.</param>
        public void Add(T item)
        {
            EnsureCapacity(size + 1);

            items[size++] = Weak.Create(item);

            version++;
        }

        /// <summary>
        /// Removes all items from the <see cref="WeakCollection{T}"/>.
        /// </summary>
        public void Clear()
        {
            if (size > 0)
            {
                for (int i = 0; i < size; i++)
                {
                    items[i].Dispose();
                }

                Array.Clear(items, 0, size);

                size = 0;
            }

            version++;
        }

        /// <summary>
        /// Determines whether the <see cref="WeakCollection{T}"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="WeakCollection{T}"/>.</param>
        /// <returns><c>true</c> if <paramref name="item"/> is found in the <see cref="WeakCollection{T}"/>; otherwise, <c>false</c>.</returns>
        public bool Contains(T item)
        {
            for (int i = 0; i < size; i++)
            {
                if (items[i].Target == item)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Copies the elements of the <see cref="WeakCollection{T}"/> to the specified array, starting at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the current <see cref="WeakCollection{T}"/>.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(T[] array, int index)
        {
            Requires.NotNull(array, nameof(array));
            Requires.NonNegative(index, nameof(index));

            if (array.Length - index < size)
            {
                throw Error.Argument(Strings.OffsetLengthInvalid);
            }

            for (int i = 0; i < size; i++)
            {
                array[index++] = items[i].Target;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="WeakCollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="WeakCollection{T}"/>.</param>
        /// <returns><c>true</c> if <paramref name="item"/> was successfully removed from the <see cref="WeakCollection{T}"/>; otherwise, <c>false</c>.</returns>
        public bool Remove(T item)
        {
            for (int i = 0; i < size; i++)
            {
                if (items[i].Target == item)
                {
                    RemoveAt(i);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the <see cref="WeakCollection{T}"/>.</returns>
        public int RemoveWhere(Predicate<T> match)
        {
            Requires.NotNull(match, nameof(match));

            int num = 0;

            if (size > 0)
            {
                for (int i = size - 1; i >= 0; i--)
                {
                    if (match(items[i].Target))
                    {
                        RemoveAt(i);

                        num++;
                    }
                }
            }

            return num;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="WeakCollection{T}"/>.
        /// </summary>
        /// <returns>A enumerator that can be used to iterate through the <see cref="WeakCollection{T}"/>.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="WeakCollection{T}"/>.
        /// </summary>
        /// <returns>A enumerator that can be used to iterate through the <see cref="WeakCollection{T}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Ensures that the capacity of the <see cref="WeakCollection{T}"/> is at least the specified value.
        /// </summary>
        /// <param name="capacity">The minimum capacity to ensure.</param>
        private void EnsureCapacity(int capacity)
        {
            if (capacity < 0)
            {
                throw Error.Argument(Strings.CapacityOverflow, nameof(capacity));
            }

            if (items.Length < capacity)
            {
                int length = items.Length == 0 ? DefaultCapacity : items.Length * 2;

                Resize(length < capacity ? capacity : length);
            }
        }

        /// <summary>
        /// Changes the current capacity of the <see cref="WeakCollection{T}"/> to the specified value.
        /// </summary>
        /// <param name="capacity">The new capacity.</param>
        private void Resize(int capacity)
        {
            Weak<T>[] array = new Weak<T>[capacity];

            if (size > 0)
            {
                Array.Copy(items, 0, array, 0, size);
            }

            items = array;
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="WeakCollection{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        private void RemoveAt(int index)
        {
            items[index].Dispose();

            size--;

            if (index < size)
            {
                Array.Copy(items, index + 1, items, index, size - index);
            }

            items[size] = null;

            version++;
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="WeakCollection{T}"/>, if that number is less than a threshold value.
        /// </summary>
        public void TrimExcess()
        {
            if (size < items.Length * 0.8)
            {
                Resize(size);
            }
        }

        /// <summary>
        /// Removes from the <see cref="WeakCollection{T}"/> elements that are no longer accessible.
        /// </summary>
        /// <returns>The number of elements removed from the <see cref="WeakCollection{T}"/>.</returns>
        public int Purge()
        {
            int num = 0;

            if (size > 0)
            {
                for (int i = size - 1; i >= 0; i--)
                {
                    if (!items[i].IsAlive)
                    {
                        RemoveAt(i);

                        num++;
                    }
                }
            }

            return num;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="WeakCollection{T}"/>.
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources used by the <see cref="WeakCollection{T}"/>.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Clear();
            }
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Enumerates the elements of a <see cref="WeakCollection{T}"/>.
        /// </summary>
        private struct Enumerator : IEnumerator<T>
        {
            #region Fields

            /// <summary>
            /// The underlying collection.
            /// </summary>
            private readonly WeakCollection<T> collection;

            /// <summary>
            /// The current position.
            /// </summary>
            private int index;

            /// <summary>
            /// The version of the collection.
            /// </summary>
            private int version;

            /// <summary>
            /// The current weak item.
            /// </summary>
            private Weak<T> current;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            /// <value>The element in the <see cref="WeakCollection{T}"/> at the current position of the enumerator.</value>
            public T Current
            {
                get { return current.Target; }
            }

            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            /// <value>The element in the <see cref="WeakCollection{T}"/> at the current position of the enumerator.</value>
            object IEnumerator.Current
            {
                get
                {
                    if (index == 0 || index > collection.size)
                    {
                        throw Error.InvalidOperation(Strings.EnumeratorInvalidPosition);
                    }

                    return current.Target;
                }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> struct.
            /// </summary>
            /// <param name="collection">The underlying collection.</param>
            internal Enumerator(WeakCollection<T> collection) : this()
            {
                this.collection = collection;

                version = collection.version;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="WeakCollection{T}"/>.
            /// </summary>
            /// <returns><c>true</c> if the enumerator was successfully advanced to the next element; <c>false</c> if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                if (version != collection.version)
                {
                    throw Error.InvalidOperation(Strings.EnumeratorFailedVersion);
                }

                if (index < collection.size)
                {
                    current = collection.items[index++];

                    return true;
                }

                index = collection.size + 1;

                current = null;

                return false;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                if (version != collection.version)
                {
                    throw Error.InvalidOperation(Strings.EnumeratorFailedVersion);
                }

                index = 0;

                current = null;
            }

            /// <summary>
            /// Releases all resources used by the <see cref="Enumerator"/>.
            /// </summary>
            public void Dispose()
            {
                index = 0;

                current = null;
            }

            #endregion
        }

        #endregion
    }
}
