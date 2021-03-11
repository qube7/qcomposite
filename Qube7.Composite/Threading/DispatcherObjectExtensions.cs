using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace Qube7.Composite.Threading
{
    /// <summary>
    /// Provides the <see cref="DispatcherObject"/>/<see cref="IDispatcherObject"/> extension methods.
    /// </summary>
    public static class DispatcherObjectExtensions
    {
        #region Methods

        /// <summary>
        /// Executes the specified <see cref="Action"/> synchronously on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        public static void Invoke(this DispatcherObject obj, Action callback)
        {
            Requires.NotNull(obj, nameof(obj));

            obj.Dispatcher.Invoke(callback);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> synchronously at the specified priority on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        public static void Invoke(this DispatcherObject obj, Action callback, DispatcherPriority priority)
        {
            Requires.NotNull(obj, nameof(obj));

            obj.Dispatcher.Invoke(callback, priority);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> synchronously at the specified priority on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        /// <param name="cancellationToken">An object that indicates whether to cancel the action.</param>
        public static void Invoke(this DispatcherObject obj, Action callback, DispatcherPriority priority, CancellationToken cancellationToken)
        {
            Requires.NotNull(obj, nameof(obj));

            obj.Dispatcher.Invoke(callback, priority, cancellationToken);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> synchronously at the specified priority on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        /// <param name="cancellationToken">An object that indicates whether to cancel the action.</param>
        /// <param name="timeout">The maximum amount of time to wait for the operation to start. Once the operation has started, it will complete before this method returns. To specify an infinite wait, use a value of -1. In a same-thread call, any other negative value is converted to -1, resulting in an infinite wait. In a cross-thread call, any other negative value throws an <see cref="ArgumentOutOfRangeException"/>.</param>
        public static void Invoke(this DispatcherObject obj, Action callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout)
        {
            Requires.NotNull(obj, nameof(obj));

            obj.Dispatcher.Invoke(callback, priority, cancellationToken, timeout);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> synchronously on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <returns>The value returned by <paramref name="callback"/>.</returns>
        public static TResult Invoke<TResult>(this DispatcherObject obj, Func<TResult> callback)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.Invoke(callback);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> synchronously at the specified priority on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        /// <returns>The value returned by <paramref name="callback"/>.</returns>
        public static TResult Invoke<TResult>(this DispatcherObject obj, Func<TResult> callback, DispatcherPriority priority)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.Invoke(callback, priority);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> synchronously at the specified priority on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        /// <param name="cancellationToken">An object that indicates whether to cancel the operation.</param>
        /// <returns>The value returned by <paramref name="callback"/>.</returns>
        public static TResult Invoke<TResult>(this DispatcherObject obj, Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.Invoke(callback, priority, cancellationToken);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> synchronously at the specified priority on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        /// <param name="cancellationToken">An object that indicates whether to cancel the operation.</param>
        /// <param name="timeout">The maximum amount of time to wait for the operation to start. Once the operation has started, it will complete before this method returns. To specify an infinite wait, use a value of -1. In a same-thread call, any other negative value is converted to -1, resulting in an infinite wait. In a cross-thread call, any other negative value throws an <see cref="ArgumentOutOfRangeException"/>.</param>
        /// <returns>The value returned by <paramref name="callback"/>.</returns>
        public static TResult Invoke<TResult>(this DispatcherObject obj, Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.Invoke(callback, priority, cancellationToken, timeout);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> asynchronously on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <returns>An object, which is returned immediately after <see cref="InvokeAsync(DispatcherObject, Action)"/> is called, that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation InvokeAsync(this DispatcherObject obj, Action callback)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.InvokeAsync(callback);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> asynchronously at the specified priority on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        /// <returns>An object, which is returned immediately after <see cref="InvokeAsync(DispatcherObject, Action, DispatcherPriority)"/> is called, that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation InvokeAsync(this DispatcherObject obj, Action callback, DispatcherPriority priority)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.InvokeAsync(callback, priority);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> asynchronously at the specified priority on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        /// <param name="cancellationToken">An object that indicates whether to cancel the action.</param>
        /// <returns>An object, which is returned immediately after <see cref="InvokeAsync(DispatcherObject, Action, DispatcherPriority, CancellationToken)"/> is called, that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation InvokeAsync(this DispatcherObject obj, Action callback, DispatcherPriority priority, CancellationToken cancellationToken)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.InvokeAsync(callback, priority, cancellationToken);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> asynchronously on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <returns>An object, which is returned immediately after <see cref="InvokeAsync{TResult}(DispatcherObject, Func{TResult})"/> is called, that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation<TResult> InvokeAsync<TResult>(this DispatcherObject obj, Func<TResult> callback)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.InvokeAsync(callback);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> asynchronously at the specified priority on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        /// <returns>An object, which is returned immediately after <see cref="InvokeAsync{TResult}(DispatcherObject, Func{TResult}, DispatcherPriority)"/> is called, that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation<TResult> InvokeAsync<TResult>(this DispatcherObject obj, Func<TResult> callback, DispatcherPriority priority)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.InvokeAsync(callback, priority);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> asynchronously at the specified priority on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        /// <param name="cancellationToken">An object that indicates whether to cancel the operation.</param>
        /// <returns>An object, which is returned immediately after <see cref="InvokeAsync{TResult}(DispatcherObject, Func{TResult}, DispatcherPriority, CancellationToken)"/> is called, that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation<TResult> InvokeAsync<TResult>(this DispatcherObject obj, Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.InvokeAsync(callback, priority, cancellationToken);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> synchronously on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        public static void Invoke(this IDispatcherObject obj, Action callback)
        {
            Requires.NotNull(obj, nameof(obj));

            obj.Dispatcher.Invoke(callback);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> synchronously at the specified priority on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        public static void Invoke(this IDispatcherObject obj, Action callback, DispatcherPriority priority)
        {
            Requires.NotNull(obj, nameof(obj));

            obj.Dispatcher.Invoke(callback, priority);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> synchronously at the specified priority on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        /// <param name="cancellationToken">An object that indicates whether to cancel the action.</param>
        public static void Invoke(this IDispatcherObject obj, Action callback, DispatcherPriority priority, CancellationToken cancellationToken)
        {
            Requires.NotNull(obj, nameof(obj));

            obj.Dispatcher.Invoke(callback, priority, cancellationToken);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> synchronously at the specified priority on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        /// <param name="cancellationToken">An object that indicates whether to cancel the action.</param>
        /// <param name="timeout">The maximum amount of time to wait for the operation to start. Once the operation has started, it will complete before this method returns. To specify an infinite wait, use a value of -1. In a same-thread call, any other negative value is converted to -1, resulting in an infinite wait. In a cross-thread call, any other negative value throws an <see cref="ArgumentOutOfRangeException"/>.</param>
        public static void Invoke(this IDispatcherObject obj, Action callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout)
        {
            Requires.NotNull(obj, nameof(obj));

            obj.Dispatcher.Invoke(callback, priority, cancellationToken, timeout);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> synchronously on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <returns>The value returned by <paramref name="callback"/>.</returns>
        public static TResult Invoke<TResult>(this IDispatcherObject obj, Func<TResult> callback)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.Invoke(callback);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> synchronously at the specified priority on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        /// <returns>The value returned by <paramref name="callback"/>.</returns>
        public static TResult Invoke<TResult>(this IDispatcherObject obj, Func<TResult> callback, DispatcherPriority priority)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.Invoke(callback, priority);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> synchronously at the specified priority on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        /// <param name="cancellationToken">An object that indicates whether to cancel the operation.</param>
        /// <returns>The value returned by <paramref name="callback"/>.</returns>
        public static TResult Invoke<TResult>(this IDispatcherObject obj, Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.Invoke(callback, priority, cancellationToken);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> synchronously at the specified priority on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        /// <param name="cancellationToken">An object that indicates whether to cancel the operation.</param>
        /// <param name="timeout">The maximum amount of time to wait for the operation to start. Once the operation has started, it will complete before this method returns. To specify an infinite wait, use a value of -1. In a same-thread call, any other negative value is converted to -1, resulting in an infinite wait. In a cross-thread call, any other negative value throws an <see cref="ArgumentOutOfRangeException"/>.</param>
        /// <returns>The value returned by <paramref name="callback"/>.</returns>
        public static TResult Invoke<TResult>(this IDispatcherObject obj, Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.Invoke(callback, priority, cancellationToken, timeout);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> asynchronously on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <returns>An object, which is returned immediately after <see cref="InvokeAsync(DispatcherObject, Action)"/> is called, that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation InvokeAsync(this IDispatcherObject obj, Action callback)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.InvokeAsync(callback);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> asynchronously at the specified priority on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        /// <returns>An object, which is returned immediately after <see cref="InvokeAsync(DispatcherObject, Action, DispatcherPriority)"/> is called, that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation InvokeAsync(this IDispatcherObject obj, Action callback, DispatcherPriority priority)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.InvokeAsync(callback, priority);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> asynchronously at the specified priority on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        /// <param name="cancellationToken">An object that indicates whether to cancel the action.</param>
        /// <returns>An object, which is returned immediately after <see cref="InvokeAsync(DispatcherObject, Action, DispatcherPriority, CancellationToken)"/> is called, that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation InvokeAsync(this IDispatcherObject obj, Action callback, DispatcherPriority priority, CancellationToken cancellationToken)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.InvokeAsync(callback, priority, cancellationToken);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> asynchronously on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <returns>An object, which is returned immediately after <see cref="InvokeAsync{TResult}(DispatcherObject, Func{TResult})"/> is called, that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation<TResult> InvokeAsync<TResult>(this IDispatcherObject obj, Func<TResult> callback)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.InvokeAsync(callback);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> asynchronously at the specified priority on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        /// <returns>An object, which is returned immediately after <see cref="InvokeAsync{TResult}(DispatcherObject, Func{TResult}, DispatcherPriority)"/> is called, that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation<TResult> InvokeAsync<TResult>(this IDispatcherObject obj, Func<TResult> callback, DispatcherPriority priority)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.InvokeAsync(callback, priority);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> asynchronously at the specified priority on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="obj">The source dispatcher object.</param>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="Dispatcher"/>.</param>
        /// <param name="cancellationToken">An object that indicates whether to cancel the operation.</param>
        /// <returns>An object, which is returned immediately after <see cref="InvokeAsync{TResult}(DispatcherObject, Func{TResult}, DispatcherPriority, CancellationToken)"/> is called, that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation<TResult> InvokeAsync<TResult>(this IDispatcherObject obj, Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher.InvokeAsync(callback, priority, cancellationToken);
        }

        /// <summary>
        /// Determines whether the calling thread has access to the <see cref="IDispatcherObject"/>.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        /// <returns><c>true</c> if the calling thread has access to the object; otherwise, <c>false</c>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool CheckAccess(this IDispatcherObject obj)
        {
            Requires.NotNull(obj, nameof(obj));

            return obj.Dispatcher?.CheckAccess() ?? true;
        }

        /// <summary>
        /// Enforces that the calling thread has access to the <see cref="IDispatcherObject"/>.
        /// </summary>
        /// <param name="obj">The source dispatcher object.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void VerifyAccess(this IDispatcherObject obj)
        {
            Requires.NotNull(obj, nameof(obj));

            obj.Dispatcher?.VerifyAccess();
        }

        #endregion
    }
}
