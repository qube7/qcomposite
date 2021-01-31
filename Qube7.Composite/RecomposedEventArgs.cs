using System;
using System.Collections.Generic;

namespace Qube7.Composite
{
    /// <summary>
    /// Provides data for the <see cref="RecomposableCollection{T}.Recomposed"/> event.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    public class RecomposedEventArgs<T> : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the elements that have been added to the <see cref="RecomposableCollection{T}"/>.
        /// </summary>
        /// <value>A <see cref="IEnumerable{T}"/> that contains the added elements.</value>
        public IEnumerable<T> Added { get; private set; }

        /// <summary>
        /// Gets the elements that have been removed from the <see cref="RecomposableCollection{T}"/>.
        /// </summary>
        /// <value>A <see cref="IEnumerable{T}"/> that contains the removed elements.</value>
        public IEnumerable<T> Removed { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RecomposedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="added">A <see cref="IEnumerable{T}"/> that contains the added elements.</param>
        /// <param name="removed">A <see cref="IEnumerable{T}"/> that contains the removed elements.</param>
        public RecomposedEventArgs(IEnumerable<T> added, IEnumerable<T> removed)
        {
            Requires.NotNull(added, nameof(added));
            Requires.NotNull(removed, nameof(removed));

            Added = added;
            Removed = removed;
        }

        #endregion
    }
}
