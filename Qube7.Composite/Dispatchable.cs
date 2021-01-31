using System;
using System.ComponentModel;
using System.Windows.Threading;
using Qube7.Composite.Threading;

namespace Qube7.Composite
{
    /// <summary>
    /// Provides helpers for the <see cref="DispatcherObject"/>/<see cref="IDispatcherObject"/>.
    /// </summary>
    public static class Dispatchable
    {
        #region Methods

        /// <summary>
        /// Executes the specified delegate synchronously at the specified priority on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        public static void Invoke(this DispatcherObject dispatchable, Action callback, DispatcherPriority priority)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));

            dispatchable.Dispatcher.Invoke(callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate synchronously on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        public static void Invoke(this DispatcherObject dispatchable, Action callback)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));

            dispatchable.Dispatcher.Invoke(callback);
        }

        /// <summary>
        /// Executes the specified delegate synchronously at the specified priority on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        /// <returns>The value returned by the <paramref name="callback"/>.</returns>
        public static TResult Invoke<TResult>(this DispatcherObject dispatchable, Func<TResult> callback, DispatcherPriority priority)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));

            return dispatchable.Dispatcher.Invoke(callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate synchronously on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <returns>The value returned by the <paramref name="callback"/>.</returns>
        public static TResult Invoke<TResult>(this DispatcherObject dispatchable, Func<TResult> callback)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));

            return dispatchable.Dispatcher.Invoke(callback);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously at the specified priority on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        /// <returns>An object that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation InvokeAsync(this DispatcherObject dispatchable, Action callback, DispatcherPriority priority)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));

            return dispatchable.Dispatcher.InvokeAsync(callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <returns>An object that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation InvokeAsync(this DispatcherObject dispatchable, Action callback)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));

            return dispatchable.Dispatcher.InvokeAsync(callback);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously at the specified priority on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        /// <returns>An object that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation<TResult> InvokeAsync<TResult>(this DispatcherObject dispatchable, Func<TResult> callback, DispatcherPriority priority)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));

            return dispatchable.Dispatcher.InvokeAsync(callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <returns>An object that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation<TResult> InvokeAsync<TResult>(this DispatcherObject dispatchable, Func<TResult> callback)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));

            return dispatchable.Dispatcher.InvokeAsync(callback);
        }

        /// <summary>
        /// Executes the specified delegate synchronously at the specified priority on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        public static void Invoke(this IDispatcherObject dispatchable, Action callback, DispatcherPriority priority)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));

            dispatchable.Dispatcher.Invoke(callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate synchronously on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        public static void Invoke(this IDispatcherObject dispatchable, Action callback)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));

            dispatchable.Dispatcher.Invoke(callback);
        }

        /// <summary>
        /// Executes the specified delegate synchronously at the specified priority on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        /// <returns>The value returned by the <paramref name="callback"/>.</returns>
        public static TResult Invoke<TResult>(this IDispatcherObject dispatchable, Func<TResult> callback, DispatcherPriority priority)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));

            return dispatchable.Dispatcher.Invoke(callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate synchronously on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <returns>The value returned by the <paramref name="callback"/>.</returns>
        public static TResult Invoke<TResult>(this IDispatcherObject dispatchable, Func<TResult> callback)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));

            return dispatchable.Dispatcher.Invoke(callback);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously at the specified priority on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        /// <returns>An object that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation InvokeAsync(this IDispatcherObject dispatchable, Action callback, DispatcherPriority priority)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));

            return dispatchable.Dispatcher.InvokeAsync(callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <returns>An object that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation InvokeAsync(this IDispatcherObject dispatchable, Action callback)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));

            return dispatchable.Dispatcher.InvokeAsync(callback);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously at the specified priority on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        /// <returns>An object that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation<TResult> InvokeAsync<TResult>(this IDispatcherObject dispatchable, Func<TResult> callback, DispatcherPriority priority)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));

            return dispatchable.Dispatcher.InvokeAsync(callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <returns>An object that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation<TResult> InvokeAsync<TResult>(this IDispatcherObject dispatchable, Func<TResult> callback)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));

            return dispatchable.Dispatcher.InvokeAsync(callback);
        }

        /// <summary>
        /// Determines whether the calling thread has access to the <see cref="IDispatcherObject"/>.
        /// </summary>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <returns><c>true</c> if the calling thread has access to the object; otherwise, <c>false</c>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool CheckAccess(this IDispatcherObject dispatchable)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));

            return dispatchable.Dispatcher?.CheckAccess() ?? true;
        }

        /// <summary>
        /// Enforces that the calling thread has access to the <see cref="IDispatcherObject"/>.
        /// </summary>
        /// <param name="dispatchable">The source dispatcher object.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void VerifyAccess(this IDispatcherObject dispatchable)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));

            dispatchable.Dispatcher?.VerifyAccess();
        }

        #endregion
    }
}
