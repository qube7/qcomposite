using System;
using System.Windows;
using System.Windows.Threading;
using Qube7.Composite.Threading;

namespace Qube7.Composite
{
    /// <summary>
    /// Provides helpers for the <see cref="Dispatcher"/>.
    /// </summary>
    public static class Dispatch
    {
        #region Methods

        /// <summary>
        /// Executes the specified delegate on the calling thread or synchronously at the specified priority on the thread the <see cref="Dispatcher"/> is associated with.
        /// </summary>
        /// <param name="dispatcher">The source dispatcher.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        public static void Sync(Dispatcher dispatcher, Action callback, DispatcherPriority priority)
        {
            Requires.NotNull(dispatcher, nameof(dispatcher));
            Requires.NotNull(callback, nameof(callback));

            if (dispatcher.CheckAccess())
            {
                callback();

                return;
            }

            dispatcher.Invoke(callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or synchronously on the thread the <see cref="Dispatcher"/> is associated with.
        /// </summary>
        /// <param name="dispatcher">The source dispatcher.</param>
        /// <param name="callback">The delegate to execute.</param>
        public static void Sync(Dispatcher dispatcher, Action callback)
        {
            Requires.NotNull(dispatcher, nameof(dispatcher));
            Requires.NotNull(callback, nameof(callback));

            if (dispatcher.CheckAccess())
            {
                callback();

                return;
            }

            dispatcher.Invoke(callback);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or synchronously at the specified priority on the thread the <see cref="Dispatcher"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="dispatcher">The source dispatcher.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        /// <returns>The value returned by the <paramref name="callback"/>.</returns>
        public static TResult Sync<TResult>(Dispatcher dispatcher, Func<TResult> callback, DispatcherPriority priority)
        {
            Requires.NotNull(dispatcher, nameof(dispatcher));
            Requires.NotNull(callback, nameof(callback));

            if (dispatcher.CheckAccess())
            {
                return callback();
            }

            return dispatcher.Invoke(callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or synchronously on the thread the <see cref="Dispatcher"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="dispatcher">The source dispatcher.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <returns>The value returned by the <paramref name="callback"/>.</returns>
        public static TResult Sync<TResult>(Dispatcher dispatcher, Func<TResult> callback)
        {
            Requires.NotNull(dispatcher, nameof(dispatcher));
            Requires.NotNull(callback, nameof(callback));

            if (dispatcher.CheckAccess())
            {
                return callback();
            }

            return dispatcher.Invoke(callback);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or synchronously at the specified priority on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        public static void Sync(DispatcherObject dispatchable, Action callback, DispatcherPriority priority)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));
            Requires.NotNull(callback, nameof(callback));

            if (dispatchable.CheckAccess())
            {
                callback();

                return;
            }

            dispatchable.Dispatcher.Invoke(callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or synchronously on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        public static void Sync(DispatcherObject dispatchable, Action callback)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));
            Requires.NotNull(callback, nameof(callback));

            if (dispatchable.CheckAccess())
            {
                callback();

                return;
            }

            dispatchable.Dispatcher.Invoke(callback);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or synchronously at the specified priority on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        /// <returns>The value returned by the <paramref name="callback"/>.</returns>
        public static TResult Sync<TResult>(DispatcherObject dispatchable, Func<TResult> callback, DispatcherPriority priority)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));
            Requires.NotNull(callback, nameof(callback));

            if (dispatchable.CheckAccess())
            {
                return callback();
            }

            return dispatchable.Dispatcher.Invoke(callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or synchronously on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <returns>The value returned by the <paramref name="callback"/>.</returns>
        public static TResult Sync<TResult>(DispatcherObject dispatchable, Func<TResult> callback)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));
            Requires.NotNull(callback, nameof(callback));

            if (dispatchable.CheckAccess())
            {
                return callback();
            }

            return dispatchable.Dispatcher.Invoke(callback);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or synchronously at the specified priority on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        public static void Sync(IDispatcherObject dispatchable, Action callback, DispatcherPriority priority)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));
            Requires.NotNull(callback, nameof(callback));

            if (dispatchable.CheckAccess())
            {
                callback();

                return;
            }

            dispatchable.Dispatcher.Invoke(callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or synchronously on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        public static void Sync(IDispatcherObject dispatchable, Action callback)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));
            Requires.NotNull(callback, nameof(callback));

            if (dispatchable.CheckAccess())
            {
                callback();

                return;
            }

            dispatchable.Dispatcher.Invoke(callback);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or synchronously at the specified priority on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        /// <returns>The value returned by the <paramref name="callback"/>.</returns>
        public static TResult Sync<TResult>(IDispatcherObject dispatchable, Func<TResult> callback, DispatcherPriority priority)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));
            Requires.NotNull(callback, nameof(callback));

            if (dispatchable.CheckAccess())
            {
                return callback();
            }

            return dispatchable.Dispatcher.Invoke(callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or synchronously on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <returns>The value returned by the <paramref name="callback"/>.</returns>
        public static TResult Sync<TResult>(IDispatcherObject dispatchable, Func<TResult> callback)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));
            Requires.NotNull(callback, nameof(callback));

            if (dispatchable.CheckAccess())
            {
                return callback();
            }

            return dispatchable.Dispatcher.Invoke(callback);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or synchronously at the specified priority on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        public static void Sync(Action callback, DispatcherPriority priority)
        {
            Sync(Application.Current, callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or synchronously on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <param name="callback">The delegate to execute.</param>
        public static void Sync(Action callback)
        {
            Sync(Application.Current, callback);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or synchronously at the specified priority on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        /// <returns>The value returned by the <paramref name="callback"/>.</returns>
        public static TResult Sync<TResult>(Func<TResult> callback, DispatcherPriority priority)
        {
            return Sync(Application.Current, callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or synchronously on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The return value type of the specified delegate.</typeparam>
        /// <param name="callback">The delegate to execute.</param>
        /// <returns>The value returned by the <paramref name="callback"/>.</returns>
        public static TResult Sync<TResult>(Func<TResult> callback)
        {
            return Sync(Application.Current, callback);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or asynchronously at the specified priority on the thread the <see cref="Dispatcher"/> is associated with.
        /// </summary>
        /// <param name="dispatcher">The source dispatcher.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        /// <returns>An object that can be used to interact with the delegate as it is pending execution in the event queue, or <c>null</c> if the delegate has been executed on the calling thread.</returns>
        public static DispatcherOperation Async(Dispatcher dispatcher, Action callback, DispatcherPriority priority)
        {
            Requires.NotNull(dispatcher, nameof(dispatcher));
            Requires.NotNull(callback, nameof(callback));

            if (dispatcher.CheckAccess())
            {
                callback();

                return null;
            }

            return dispatcher.InvokeAsync(callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or asynchronously on the thread the <see cref="Dispatcher"/> is associated with.
        /// </summary>
        /// <param name="dispatcher">The source dispatcher.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <returns>An object that can be used to interact with the delegate as it is pending execution in the event queue, or <c>null</c> if the delegate has been executed on the calling thread.</returns>
        public static DispatcherOperation Async(Dispatcher dispatcher, Action callback)
        {
            Requires.NotNull(dispatcher, nameof(dispatcher));
            Requires.NotNull(callback, nameof(callback));

            if (dispatcher.CheckAccess())
            {
                callback();

                return null;
            }

            return dispatcher.InvokeAsync(callback);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or asynchronously at the specified priority on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        /// <returns>An object that can be used to interact with the delegate as it is pending execution in the event queue, or <c>null</c> if the delegate has been executed on the calling thread.</returns>
        public static DispatcherOperation Async(DispatcherObject dispatchable, Action callback, DispatcherPriority priority)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));
            Requires.NotNull(callback, nameof(callback));

            if (dispatchable.CheckAccess())
            {
                callback();

                return null;
            }

            return dispatchable.Dispatcher.InvokeAsync(callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or asynchronously on the thread the <see cref="DispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <returns>An object that can be used to interact with the delegate as it is pending execution in the event queue, or <c>null</c> if the delegate has been executed on the calling thread.</returns>
        public static DispatcherOperation Async(DispatcherObject dispatchable, Action callback)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));
            Requires.NotNull(callback, nameof(callback));

            if (dispatchable.CheckAccess())
            {
                callback();

                return null;
            }

            return dispatchable.Dispatcher.InvokeAsync(callback);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or asynchronously at the specified priority on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        /// <returns>An object that can be used to interact with the delegate as it is pending execution in the event queue, or <c>null</c> if the delegate has been executed on the calling thread.</returns>
        public static DispatcherOperation Async(IDispatcherObject dispatchable, Action callback, DispatcherPriority priority)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));
            Requires.NotNull(callback, nameof(callback));

            if (dispatchable.CheckAccess())
            {
                callback();

                return null;
            }

            return dispatchable.Dispatcher.InvokeAsync(callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or asynchronously on the thread the <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <param name="dispatchable">The source dispatcher object.</param>
        /// <param name="callback">The delegate to execute.</param>
        /// <returns>An object that can be used to interact with the delegate as it is pending execution in the event queue, or <c>null</c> if the delegate has been executed on the calling thread.</returns>
        public static DispatcherOperation Async(IDispatcherObject dispatchable, Action callback)
        {
            Requires.NotNull(dispatchable, nameof(dispatchable));
            Requires.NotNull(callback, nameof(callback));

            if (dispatchable.CheckAccess())
            {
                callback();

                return null;
            }

            return dispatchable.Dispatcher.InvokeAsync(callback);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or asynchronously at the specified priority on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <param name="callback">The delegate to execute.</param>
        /// <param name="priority">The priority for the <paramref name="callback"/>.</param>
        /// <returns>An object that can be used to interact with the delegate as it is pending execution in the event queue, or <c>null</c> if the delegate has been executed on the calling thread.</returns>
        public static DispatcherOperation Async(Action callback, DispatcherPriority priority)
        {
            return Async(Application.Current, callback, priority);
        }

        /// <summary>
        /// Executes the specified delegate on the calling thread or asynchronously on the thread the <see cref="Application"/> object for the current <see cref="AppDomain"/> is associated with.
        /// </summary>
        /// <param name="callback">The delegate to execute.</param>
        /// <returns>An object that can be used to interact with the delegate as it is pending execution in the event queue, or <c>null</c> if the delegate has been executed on the calling thread.</returns>
        public static DispatcherOperation Async(Action callback)
        {
            return Async(Application.Current, callback);
        }

        #endregion
    }
}
