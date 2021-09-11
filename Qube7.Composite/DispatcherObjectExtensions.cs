using System;
using System.Windows.Threading;
using Qube7.Composite.Threading;

namespace Qube7.Composite
{
    /// <summary>
    /// Provides extension methods for the <see cref="IDispatcherObject"/>.
    /// </summary>
    public static class DispatcherObjectExtensions
    {
        #region Methods

        /// <summary>
        /// Performs the specified <see cref="Action{T}"/> on the <see cref="IDispatcherObject"/> on the calling thread if it has access to the <see cref="IDispatcherObject"/> or synchronously on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="T">The type of the source dispatcher object.</typeparam>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to perform on <paramref name="obj"/>.</param>
        public static void Update<T>(this T obj, Action<T> callback) where T : class, IDispatcherObject
        {
            Requires.NotNull(obj, nameof(obj));
            Requires.NotNull(callback, nameof(callback));

            if (obj.CheckAccess())
            {
                callback(obj);

                return;
            }

            obj.Dispatcher.Invoke(() => callback(obj), DispatcherPriority.Normal);
        }

        /// <summary>
        /// Performs the specified <see cref="Action{T}"/> on the <see cref="IDispatcherObject"/> on the calling thread if it has access to the <see cref="IDispatcherObject"/> or asynchronously on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="T">The type of the source dispatcher object.</typeparam>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to perform on <paramref name="obj"/>.</param>
        /// <returns>An object that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation UpdateAsync<T>(this T obj, Action<T> callback) where T : class, IDispatcherObject
        {
            Requires.NotNull(obj, nameof(obj));
            Requires.NotNull(callback, nameof(callback));

            if (obj.CheckAccess())
            {
                callback(obj);

                return DispatcherOperationCache.CompletedOperation;
            }

            return obj.Dispatcher.InvokeAsync(() => callback(obj), DispatcherPriority.Normal);
        }

        #endregion
    }
}
