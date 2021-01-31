using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qube7.ComponentModel
{
    /// <summary>
    /// Represents a container that manages the lifetime of registered objects.
    /// </summary>
    /// <threadsafety static="true" instance="true"/>
    public class LifetimeContainer : ICollection<object>, IDisposable
    {
        #region Fields

        /// <summary>
        /// The object references collection.
        /// </summary>
        private readonly List<object> objects = new List<object>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of references contained in the <see cref="LifetimeContainer"/>.
        /// </summary>
        /// <value>The number of references contained in the <see cref="LifetimeContainer"/>.</value>
        public int Count
        {
            get
            {
                lock (objects)
                {
                    return objects.Count;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="LifetimeContainer"/> is read-only.
        /// </summary>
        /// <value><c>false</c> indicating that the <see cref="LifetimeContainer"/> is not read-only.</value>
        bool ICollection<object>.IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the <see cref="LifetimeContainer"/> is disposed.
        /// </summary>
        public event EventHandler Disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LifetimeContainer"/> class.
        /// </summary>
        public LifetimeContainer()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds an item to the <see cref="LifetimeContainer"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="LifetimeContainer"/>.</param>
        public void Add(object item)
        {
            lock (objects)
            {
                objects.Add(item);
            }
        }

        /// <summary>
        /// Adds the range of items to the <see cref="LifetimeContainer"/>.
        /// </summary>
        /// <param name="items">The objects to add to the <see cref="LifetimeContainer"/>.</param>
        public void AddRange(IEnumerable<object> items)
        {
            Requires.NotNull(items, nameof(items));

            lock (objects)
            {
                foreach (object item in items)
                {
                    objects.Add(item);
                }
            }
        }

        /// <summary>
        /// Adds the range of items to the <see cref="LifetimeContainer"/>.
        /// </summary>
        /// <param name="items">The objects to add to the <see cref="LifetimeContainer"/>.</param>
        public void AddRange(params object[] items)
        {
            AddRange(items.AsEnumerable());
        }

        /// <summary>
        /// Removes all items from the <see cref="LifetimeContainer"/>.
        /// </summary>
        public void Clear()
        {
            lock (objects)
            {
                objects.Clear();
            }
        }

        /// <summary>
        /// Determines whether the <see cref="LifetimeContainer"/> contains a specific object.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="LifetimeContainer"/>.</param>
        /// <returns><c>true</c> if <paramref name="item"/> is found in the <see cref="LifetimeContainer"/>; otherwise, <c>false</c>.</returns>
        public bool Contains(object item)
        {
            lock (objects)
            {
                return objects.Contains(item);
            }
        }

        /// <summary>
        /// Copies the references contained in the <see cref="LifetimeContainer"/> to the specified array, starting at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the references copied from the current <see cref="LifetimeContainer"/>.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(object[] array, int index)
        {
            lock (objects)
            {
                objects.CopyTo(array, index);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="LifetimeContainer"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="LifetimeContainer"/>.</param>
        /// <returns><c>true</c> if <paramref name="item"/> was successfully removed from the <see cref="LifetimeContainer"/>; otherwise, <c>false</c>.</returns>
        public bool Remove(object item)
        {
            lock (objects)
            {
                return objects.Remove(item);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="LifetimeContainer"/>.
        /// </summary>
        /// <returns>A enumerator that can be used to iterate through the <see cref="LifetimeContainer"/>.</returns>
        public IEnumerator<object> GetEnumerator()
        {
            lock (objects)
            {
                List<object> items = new List<object>(objects);

                return items.GetEnumerator();
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="LifetimeContainer"/>.
        /// </summary>
        /// <returns>A enumerator that can be used to iterate through the <see cref="LifetimeContainer"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Disposes contained objects and releases all resources used by the <see cref="LifetimeContainer"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes contained objects and releases resources used by the <see cref="LifetimeContainer"/>.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                object[] array;

                lock (objects)
                {
                    array = objects.ToArray();

                    objects.Clear();
                }

                for (int i = array.Length - 1; i >= 0; i--)
                {
                    Disposable.Dispose(array[i]);
                }

                OnDisposed();
            }
        }

        /// <summary>
        /// Raises the <see cref="Disposed"/> event.
        /// </summary>
        protected virtual void OnDisposed()
        {
            Event.Raise(Disposed, this);
        }

        #endregion
    }
}
