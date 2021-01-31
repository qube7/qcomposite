using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Qube7
{
    /// <summary>
    /// Represents a weak reference, which references an object while still allowing that object to be reclaimed by garbage collection.
    /// </summary>
    /// <typeparam name="T">The type of the target object.</typeparam>
    [DebuggerDisplay("Target = {Target}")]
    public class Weak<T> : IDisposable where T : class
    {
        #region Fields

        /// <summary>
        /// The target object handle.
        /// </summary>
        private GCHandle handle;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the object referenced by the current <see cref="Weak{T}"/>.
        /// </summary>
        /// <value>The object referenced by the current <see cref="Weak{T}"/>, if accessible; otherwise, <c>null</c>.</value>
        public virtual T Target
        {
            get
            {
                try
                {
                    return handle.Target as T;
                }
                catch (InvalidOperationException)
                {
                    throw Error.ObjectDisposed(this);
                }
            }
            set
            {
                try
                {
                    handle.Target = value;
                }
                catch (InvalidOperationException)
                {
                    throw Error.ObjectDisposed(this);
                }
            }
        }

        /// <summary>
        /// Gets an indication whether the object referenced by the current <see cref="Weak{T}"/> is accessible.
        /// </summary>
        /// <value><c>true</c> if the object referenced by the current <see cref="Weak{T}"/> is accessible; otherwise, <c>false</c>.</value>
        public virtual bool IsAlive
        {
            get
            {
                try
                {
                    return handle.Target != null;
                }
                catch (InvalidOperationException)
                {
                    throw Error.ObjectDisposed(this);
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Weak{T}"/> class.
        /// </summary>
        /// <param name="target">The target object to track.</param>
        public Weak(T target)
        {
            handle = GCHandle.Alloc(target, GCHandleType.Weak);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Weak{T}"/> class.
        /// </summary>
        public Weak() : this(null)
        {
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the <see cref="Weak{T}"/> is reclaimed by garbage collection.
        /// </summary>
        ~Weak()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases all resources used by the <see cref="Weak{T}"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources used by the <see cref="Weak{T}"/>.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (handle.IsAllocated)
            {
                handle.Free();
            }
        }

        #endregion
    }
}
