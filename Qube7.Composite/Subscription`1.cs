using System;
using System.ComponentModel;

namespace Qube7.Composite
{
    /// <summary>
    /// Represents an subscription of the property change notification.
    /// </summary>
    /// <typeparam name="T">The type of the source object.</typeparam>
    internal abstract class Subscription<T> where T : class, INotifyPropertyChanged
    {
        #region Methods

        /// <summary>
        /// Processes the property change notification event.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        internal abstract void ProcessEvent(T source, string propertyName);

        /// <summary>
        /// Cancels the subscription of the property change notification.
        /// </summary>
        /// <param name="propertyName">The name of the observed property.</param>
        /// <param name="callback">The delegate to unsubscribe.</param>
        /// <returns><c>true</c> if subscription was successfully canceled; otherwise, <c>false</c>.</returns>
        internal virtual bool Unsubscribe(string propertyName, Action<T> callback)
        {
            return false;
        }

        /// <summary>
        /// Cancels the subscription of the property change notification.
        /// </summary>
        /// <param name="propertyName">The name of the observed property.</param>
        /// <param name="callback">The delegate to unsubscribe.</param>
        /// <returns><c>true</c> if subscription was successfully canceled; otherwise, <c>false</c>.</returns>
        internal virtual bool Unsubscribe(string propertyName, Action callback)
        {
            return false;
        }

        /// <summary>
        /// Cancels the subscription of the property change notification.
        /// </summary>
        /// <param name="callback">The delegate to unsubscribe.</param>
        /// <returns><c>true</c> if subscription was successfully canceled; otherwise, <c>false</c>.</returns>
        internal virtual bool Unsubscribe(Action<T> callback)
        {
            return false;
        }

        /// <summary>
        /// Cancels the subscription of the property change notification.
        /// </summary>
        /// <param name="callback">The delegate to unsubscribe.</param>
        /// <returns><c>true</c> if subscription was successfully canceled; otherwise, <c>false</c>.</returns>
        internal virtual bool Unsubscribe(Action callback)
        {
            return false;
        }

        #endregion
    }
}
