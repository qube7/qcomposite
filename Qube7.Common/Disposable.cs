using System;
using System.Threading;

namespace Qube7
{
    /// <summary>
    /// Delegates the disposal action.
    /// </summary>
    public class Disposable : IDisposable
    {
        #region Fields

        /// <summary>
        /// The dispose action.
        /// </summary>
        private Action dispose;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposable"/> class.
        /// </summary>
        /// <param name="dispose">The dispose action.</param>
        public Disposable(Action dispose)
        {
            this.dispose = dispose;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes the specified object instance if it implements the <see cref="IDisposable"/>.
        /// </summary>
        /// <param name="instance">The object to dispose.</param>
        public static void Dispose(object instance)
        {
            if (instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// Invokes the dispose action.
        /// </summary>
        public void Dispose()
        {
            Action action = Interlocked.Exchange(ref dispose, null);
            if (action != null)
            {
                action();
            }
        }

        #endregion
    }
}
