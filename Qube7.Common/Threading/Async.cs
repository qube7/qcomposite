using System;
using System.Threading;

namespace Qube7.Threading
{
    /// <summary>
    /// Provides helpers for executing asynchronous operations.
    /// </summary>
    public static class Async
    {
        #region Methods

        /// <summary>
        /// Starts the asynchronous operation in the thread pool.
        /// </summary>
        /// <param name="callback">The delegate to execute.</param>
        public static void Start(Action callback)
        {
            Requires.NotNull(callback, nameof(callback));

            ThreadPool.QueueUserWorkItem(s => callback());
        }

        /// <summary>
        /// Starts the asynchronous operation in the thread pool.
        /// </summary>
        /// <typeparam name="T">The parameter type of the specified delegate.</typeparam>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="arg">The object to pass as an argument.</param>
        public static void Start<T>(Action<T> callback, T arg)
        {
            Requires.NotNull(callback, nameof(callback));

            ThreadPool.QueueUserWorkItem(s => callback((T)s), arg);
        }

        #endregion
    }
}
