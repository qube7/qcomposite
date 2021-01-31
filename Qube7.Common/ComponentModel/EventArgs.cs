using System;

namespace Qube7.ComponentModel
{
    /// <summary>
    /// Represents the generic <see cref="EventArgs"/> that contains the event data.
    /// </summary>
    /// <typeparam name="T">The type of the event data.</typeparam>
    public class EventArgs<T> : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the data associated with the event.
        /// </summary>
        /// <value>The event data.</value>
        public T Data { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T}"/> class.
        /// </summary>
        /// <param name="data">The event data.</param>
        public EventArgs(T data)
        {
            Data = data;
        }

        #endregion
    }
}
