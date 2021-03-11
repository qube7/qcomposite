using System;
using Qube7.Threading;

namespace Qube7.ComponentModel
{
    /// <summary>
    /// Provides data for an event that conditionally supports cancellation.
    /// </summary>
    public class CancelableEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// A value indicating whether the event is marked for cancellation.
        /// </summary>
        private int canceled;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the event can be canceled.
        /// </summary>
        /// <value><c>true</c> if the event can be canceled; otherwise, <c>false</c>.</value>
        public bool CanCancel { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the event is marked for cancellation.
        /// </summary>
        /// <value><c>true</c> if the event is marked for cancellation; otherwise, <c>false</c>.</value>
        public bool IsCanceled
        {
            get { return Variable.Equals1(ref canceled); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableEventArgs"/> class with the <see cref="CanCancel"/> property set to the given value.
        /// </summary>
        /// <param name="canCancel"><c>true</c> if the event can be canceled; otherwise, <c>false</c>.</param>
        public CancelableEventArgs(bool canCancel)
        {
            CanCancel = canCancel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableEventArgs"/> class with the <see cref="CanCancel"/> property set to <c>true</c>.
        /// </summary>
        public CancelableEventArgs() : this(true)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Marks the event for cancellation.
        /// </summary>
        public void Cancel()
        {
            if (CanCancel)
            {
                Variable.Increment0(ref canceled);

                return;
            }

            throw Error.InvalidOperation(Strings.NotCancelable);
        }

        #endregion
    }
}
