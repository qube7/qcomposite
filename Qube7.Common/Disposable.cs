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
        /// Disposes the specified object instance.
        /// </summary>
        /// <typeparam name="T">The type of the object to dispose.</typeparam>
        /// <param name="disposable">The object to dispose.</param>
        public static void Dispose<T>(T disposable) where T : IDisposable
        {
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// Disposes the specified object instance if it implements the <see cref="IDisposable"/>.
        /// </summary>
        /// <param name="instance">The object to dispose.</param>
        public static void Dispose(object instance)
        {
            IDisposable disposable = instance as IDisposable;
            if (disposable != null)
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
