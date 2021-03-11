using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Qube7.Composite.Threading
{
    /// <summary>
    /// Manages access to the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
    /// </summary>
    public static class UIThread
    {
        #region Fields

        /// <summary>
        /// The associated dispatcher.
        /// </summary>
        private static readonly Dispatcher dispatcher;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="System.Windows.Threading.Dispatcher"/> the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <value>The associated <see cref="System.Windows.Threading.Dispatcher"/>.</value>
        public static Dispatcher Dispatcher
        {
            get { return dispatcher; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="UIThread"/> class.
        /// </summary>
        static UIThread()
        {
            dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes the specified <see cref="Action"/> synchronously on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        public static void Invoke(Action callback)
        {
            dispatcher.Invoke(callback);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> synchronously at the specified priority on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="System.Windows.Threading.Dispatcher"/>.</param>
        public static void Invoke(Action callback, DispatcherPriority priority)
        {
            dispatcher.Invoke(callback, priority);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> synchronously at the specified priority on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="System.Windows.Threading.Dispatcher"/>.</param>
        /// <param name="cancellationToken">An object that indicates whether to cancel the action.</param>
        public static void Invoke(Action callback, DispatcherPriority priority, CancellationToken cancellationToken)
        {
            dispatcher.Invoke(callback, priority, cancellationToken);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> synchronously at the specified priority on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="System.Windows.Threading.Dispatcher"/>.</param>
        /// <param name="cancellationToken">An object that indicates whether to cancel the action.</param>
        /// <param name="timeout">The maximum amount of time to wait for the operation to start. Once the operation has started, it will complete before this method returns. To specify an infinite wait, use a value of -1. In a same-thread call, any other negative value is converted to -1, resulting in an infinite wait. In a cross-thread call, any other negative value throws an <see cref="ArgumentOutOfRangeException"/>.</param>
        public static void Invoke(Action callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout)
        {
            dispatcher.Invoke(callback, priority, cancellationToken, timeout);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> synchronously on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <returns>The value returned by <paramref name="callback"/>.</returns>
        public static TResult Invoke<TResult>(Func<TResult> callback)
        {
            return dispatcher.Invoke(callback);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> synchronously at the specified priority on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="System.Windows.Threading.Dispatcher"/>.</param>
        /// <returns>The value returned by <paramref name="callback"/>.</returns>
        public static TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority)
        {
            return dispatcher.Invoke(callback, priority);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> synchronously at the specified priority on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="System.Windows.Threading.Dispatcher"/>.</param>
        /// <param name="cancellationToken">An object that indicates whether to cancel the operation.</param>
        /// <returns>The value returned by <paramref name="callback"/>.</returns>
        public static TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken)
        {
            return dispatcher.Invoke(callback, priority, cancellationToken);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> synchronously at the specified priority on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="System.Windows.Threading.Dispatcher"/>.</param>
        /// <param name="cancellationToken">An object that indicates whether to cancel the operation.</param>
        /// <param name="timeout">The maximum amount of time to wait for the operation to start. Once the operation has started, it will complete before this method returns. To specify an infinite wait, use a value of -1. In a same-thread call, any other negative value is converted to -1, resulting in an infinite wait. In a cross-thread call, any other negative value throws an <see cref="ArgumentOutOfRangeException"/>.</param>
        /// <returns>The value returned by <paramref name="callback"/>.</returns>
        public static TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken, TimeSpan timeout)
        {
            return dispatcher.Invoke(callback, priority, cancellationToken, timeout);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> asynchronously on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <returns>An object, which is returned immediately after <see cref="InvokeAsync(Action)"/> is called, that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation InvokeAsync(Action callback)
        {
            return dispatcher.InvokeAsync(callback);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> asynchronously at the specified priority on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="System.Windows.Threading.Dispatcher"/>.</param>
        /// <returns>An object, which is returned immediately after <see cref="InvokeAsync(Action, DispatcherPriority)"/> is called, that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation InvokeAsync(Action callback, DispatcherPriority priority)
        {
            return dispatcher.InvokeAsync(callback, priority);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> asynchronously at the specified priority on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="System.Windows.Threading.Dispatcher"/>.</param>
        /// <param name="cancellationToken">An object that indicates whether to cancel the action.</param>
        /// <returns>An object, which is returned immediately after <see cref="InvokeAsync(Action, DispatcherPriority, CancellationToken)"/> is called, that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation InvokeAsync(Action callback, DispatcherPriority priority, CancellationToken cancellationToken)
        {
            return dispatcher.InvokeAsync(callback, priority, cancellationToken);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> asynchronously on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <returns>An object, which is returned immediately after <see cref="InvokeAsync{TResult}(Func{TResult})"/> is called, that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation<TResult> InvokeAsync<TResult>(Func<TResult> callback)
        {
            return dispatcher.InvokeAsync(callback);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> asynchronously at the specified priority on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="System.Windows.Threading.Dispatcher"/>.</param>
        /// <returns>An object, which is returned immediately after <see cref="InvokeAsync{TResult}(Func{TResult}, DispatcherPriority)"/> is called, that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation<TResult> InvokeAsync<TResult>(Func<TResult> callback, DispatcherPriority priority)
        {
            return dispatcher.InvokeAsync(callback, priority);
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> asynchronously at the specified priority on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <param name="priority">The priority that determines the order in which the specified callback is invoked relative to the other pending operations in the <see cref="System.Windows.Threading.Dispatcher"/>.</param>
        /// <param name="cancellationToken">An object that indicates whether to cancel the operation.</param>
        /// <returns>An object, which is returned immediately after <see cref="InvokeAsync{TResult}(Func{TResult}, DispatcherPriority, CancellationToken)"/> is called, that can be used to interact with the delegate as it is pending execution in the event queue.</returns>
        public static DispatcherOperation<TResult> InvokeAsync<TResult>(Func<TResult> callback, DispatcherPriority priority, CancellationToken cancellationToken)
        {
            return dispatcher.InvokeAsync(callback, priority, cancellationToken);
        }

        /// <summary>
        /// Determines whether the calling thread has access to the <see cref="Application"/> object for the current <see cref="AppDomain"/>.
        /// </summary>
        /// <returns><c>true</c> if the calling thread has access to the object; otherwise, <c>false</c>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool CheckAccess()
        {
            return dispatcher.CheckAccess();
        }

        /// <summary>
        /// Enforces that the calling thread has access to the <see cref="Application"/> object for the current <see cref="AppDomain"/>.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void VerifyAccess()
        {
            dispatcher.VerifyAccess();
        }

        #endregion
    }
}
