using System;
using System.Windows.Threading;
using Qube7.Composite.Threading;

namespace Qube7.Composite
{
    /// <summary>
    /// Provides the <see cref="DispatcherObject"/>/<see cref="IDispatcherObject"/> extension methods.
    /// </summary>
    public static class DispatcherObjectExtensions
    {
        #region Fields

        /// <summary>
        /// The successfully completed operation.
        /// </summary>
        [ThreadStatic]
        private static DispatcherOperation completed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the operation that has already completed successfully.
        /// </summary>
        /// <value>The successfully completed operation.</value>
        private static DispatcherOperation Completed
        {
            get
            {
                if (completed == null)
                {
                    completed = Dispatcher.CurrentDispatcher.InvokeAsync(Void0);

                    completed.Wait();
                }

                return completed;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Represents an empty parameterless method.
        /// </summary>
        private static void Void0()
        {
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> on the calling thread if it has access to the <see cref="DispatcherObject"/> or synchronously on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke.</param>
        public static void Dispatch(this DispatcherObject obj, Action callback)
        {
            Requires.NotNull(obj, nameof(obj));
            Requires.NotNull(callback, nameof(callback));

            if (obj.CheckAccess())
            {
                callback();

                return;
            }

            obj.Dispatcher.Invoke(callback, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> on the calling thread if it has access to the <see cref="DispatcherObject"/> or asynchronously on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke.</param>
        /// <returns>An object that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation DispatchAsync(this DispatcherObject obj, Action callback)
        {
            Requires.NotNull(obj, nameof(obj));
            Requires.NotNull(callback, nameof(callback));

            if (obj.CheckAccess())
            {
                callback();

                return Completed;
            }

            return obj.Dispatcher.InvokeAsync(callback, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> on the calling thread if it has access to the <see cref="IDispatcherObject"/> or synchronously on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke.</param>
        public static void Dispatch(this IDispatcherObject obj, Action callback)
        {
            Requires.NotNull(obj, nameof(obj));
            Requires.NotNull(callback, nameof(callback));

            if (obj.CheckAccess())
            {
                callback();

                return;
            }

            obj.Dispatcher.Invoke(callback, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> on the calling thread if it has access to the <see cref="IDispatcherObject"/> or asynchronously on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke.</param>
        /// <returns>An object that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation DispatchAsync(this IDispatcherObject obj, Action callback)
        {
            Requires.NotNull(obj, nameof(obj));
            Requires.NotNull(callback, nameof(callback));

            if (obj.CheckAccess())
            {
                callback();

                return Completed;
            }

            return obj.Dispatcher.InvokeAsync(callback, DispatcherPriority.Normal);
        }

        #endregion
    }
}
