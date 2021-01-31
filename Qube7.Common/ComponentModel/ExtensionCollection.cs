using System;
using System.Collections.Generic;
using Qube7.Collections;

namespace Qube7.ComponentModel
{
    /// <summary>
    /// Represents a collection of aggregated <see cref="Extension{T}"/> objects.
    /// </summary>
    /// <typeparam name="T">The type of the object being extended.</typeparam>
    public sealed class ExtensionCollection<T> : SynchronizedCollection<Extension<T>> where T : class
    {
        #region Fields

        /// <summary>
        /// The owner of the collection.
        /// </summary>
        private readonly T owner;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionCollection{T}"/> class.
        /// </summary>
        /// <param name="owner">The owner of the collection.</param>
        public ExtensionCollection(T owner)
        {
            Requires.NotNull(owner, nameof(owner));

            this.owner = owner;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Removes all items from the <see cref="ExtensionCollection{T}"/>.
        /// </summary>
        protected override void ClearItems()
        {
            if (Items.Count > 0)
            {
                Extension<T>[] array = new Extension<T>[Items.Count];

                Items.CopyTo(array, 0);

                base.ClearItems();

                List<Exception> errors = null;

                for (int i = 0; i < array.Length; i++)
                {
                    try
                    {
                        array[i].Detach(owner);
                    }
                    catch (Exception exception)
                    {
                        if (errors == null)
                        {
                            errors = new List<Exception>(1);
                        }

                        errors.Add(exception);
                    }
                }

                if (errors != null)
                {
                    throw Error.Aggregate(errors.ToArray());
                }

                return;
            }

            base.ClearItems();
        }

        /// <summary>
        /// Inserts an element into the <see cref="ExtensionCollection{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="ExtensionCollection{T}"/>.</param>
        protected override void InsertItem(int index, Extension<T> item)
        {
            Requires.NotNull(item, nameof(item));

            item.Attach(owner);

            base.InsertItem(index, item);
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="ExtensionCollection{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            Extension<T> item = Items[index];

            base.RemoveItem(index);

            item.Detach(owner);
        }

        /// <summary>
        /// Set operation is not supported.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, Extension<T> item)
        {
            throw Error.NotSupported(Strings.ExtensionCannotSet);
        }

        #endregion
    }
}
