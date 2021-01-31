using System;
using System.ComponentModel;

namespace Qube7.Composite
{
    /// <summary>
    /// Defines methods for subscribing and unsubscribing property change notification.
    /// </summary>
    /// <typeparam name="T">The type of the source object.</typeparam>
    public interface ISubscriber<out T> where T : INotifyPropertyChanged
    {
        #region Methods

        /// <summary>
        /// Subscribes the specified delegate for the change notification of the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property to observe.</param>
        /// <param name="callback">The delegate to subscribe.</param>
        /// <returns>The current subscriber.</returns>
        /// <remarks>The <c>null</c> or <see cref="String.Empty"/> used for the <paramref name="propertyName"/> indicate subscription for notification when all properties on the source object have changed.</remarks>
        ISubscriber<T> Subscribe(string propertyName, Action<T> callback);

        /// <summary>
        /// Subscribes the specified delegate for the change notification of the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property to observe.</param>
        /// <param name="callback">The delegate to subscribe.</param>
        /// <returns>The current subscriber.</returns>
        /// <remarks>The <c>null</c> or <see cref="String.Empty"/> used for the <paramref name="propertyName"/> indicate subscription for notification when all properties on the source object have changed.</remarks>
        ISubscriber<T> Subscribe(string propertyName, Action callback);

        /// <summary>
        /// Subscribes the specified delegate for the change notification of any property.
        /// </summary>
        /// <param name="callback">The delegate to subscribe.</param>
        /// <returns>The current subscriber.</returns>
        ISubscriber<T> Subscribe(Action<T> callback);

        /// <summary>
        /// Subscribes the specified delegate for the change notification of any property.
        /// </summary>
        /// <param name="callback">The delegate to subscribe.</param>
        /// <returns>The current subscriber.</returns>
        ISubscriber<T> Subscribe(Action callback);

        /// <summary>
        /// Unsubscribes the specified delegate from the change notification of the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the observed property.</param>
        /// <param name="callback">The delegate to unsubscribe.</param>
        /// <returns>The current subscriber.</returns>
        /// <remarks>The <c>null</c> or <see cref="String.Empty"/> used for the <paramref name="propertyName"/> indicate subscription for notification when all properties on the source object have changed.</remarks>
        ISubscriber<T> Unsubscribe(string propertyName, Action<T> callback);

        /// <summary>
        /// Unsubscribes the specified delegate from the change notification of the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the observed property.</param>
        /// <param name="callback">The delegate to unsubscribe.</param>
        /// <returns>The current subscriber.</returns>
        /// <remarks>The <c>null</c> or <see cref="String.Empty"/> used for the <paramref name="propertyName"/> indicate subscription for notification when all properties on the source object have changed.</remarks>
        ISubscriber<T> Unsubscribe(string propertyName, Action callback);

        /// <summary>
        /// Unsubscribes the specified delegate from the change notification of any property.
        /// </summary>
        /// <param name="callback">The delegate to unsubscribe.</param>
        /// <returns>The current subscriber.</returns>
        ISubscriber<T> Unsubscribe(Action<T> callback);

        /// <summary>
        /// Unsubscribes the specified delegate from the change notification of any property.
        /// </summary>
        /// <param name="callback">The delegate to unsubscribe.</param>
        /// <returns>The current subscriber.</returns>
        ISubscriber<T> Unsubscribe(Action callback);

        #endregion
    }
}
