using System;
using System.Windows.Threading;

namespace Qube7.Composite.Threading
{
    /// <summary>
    /// Provides the cached dispatcher operation objects.
    /// </summary>
    internal static class DispatcherOperationCache
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
        internal static DispatcherOperation CompletedOperation
        {
            get
            {
                if (completed == null)
                {
                    completed = Dispatcher.CurrentDispatcher.InvokeAsync(Empty);

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
        private static void Empty()
        {
        }

        #endregion
    }
}
