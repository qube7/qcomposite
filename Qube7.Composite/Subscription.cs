using System;
using System.ComponentModel;

namespace Qube7.Composite
{
    /// <summary>
    /// Provides static methods for creating <see cref="Subscription{T}"/> objects.
    /// </summary>
    internal static class Subscription
    {
        #region Fields

        /// <summary>
        /// The empty parameterless method delegate.
        /// </summary>
        private static readonly Action Void0 = Empty;

        /// <summary>
        /// The empty single-parameter method delegate.
        /// </summary>
        private static readonly Action<object> Void1 = Empty;

        #endregion

        #region Methods

        /// <summary>
        /// Represents an empty parameterless method.
        /// </summary>
        private static void Empty()
        {
        }

        /// <summary>
        /// Represents an empty single-parameter method.
        /// </summary>
        /// <param name="obj">The parameter of the method.</param>
        private static void Empty(object obj)
        {
        }

        /// <summary>
        /// Creates the <see cref="Subscription{T}"/> for the property change notification.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="propertyName">The name of the property to observe.</param>
        /// <param name="callback">The delegate to subscribe.</param>
        /// <returns>The <see cref="Subscription{T}"/> instance.</returns>
        internal static Subscription<T> Create<T>(string propertyName, Action<T> callback) where T : class, INotifyPropertyChanged
        {
            return new SP1<T>(propertyName, callback);
        }

        /// <summary>
        /// Creates the <see cref="Subscription{T}"/> for the property change notification.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="propertyName">The name of the property to observe.</param>
        /// <param name="callback">The delegate to subscribe.</param>
        /// <returns>The <see cref="Subscription{T}"/> instance.</returns>
        internal static Subscription<T> Create<T>(string propertyName, Action callback) where T : class, INotifyPropertyChanged
        {
            return new SP0<T>(propertyName, callback);
        }

        /// <summary>
        /// Creates the <see cref="Subscription{T}"/> for the property change notification.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="callback">The delegate to subscribe.</param>
        /// <returns>The <see cref="Subscription{T}"/> instance.</returns>
        internal static Subscription<T> Create<T>(Action<T> callback) where T : class, INotifyPropertyChanged
        {
            return new S1<T>(callback);
        }

        /// <summary>
        /// Creates the <see cref="Subscription{T}"/> for the property change notification.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="callback">The delegate to subscribe.</param>
        /// <returns>The <see cref="Subscription{T}"/> instance.</returns>
        internal static Subscription<T> Create<T>(Action callback) where T : class, INotifyPropertyChanged
        {
            return new S0<T>(callback);
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents an subscription of the property change notification.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        private class S0<T> : Subscription<T> where T : class, INotifyPropertyChanged
        {
            #region Fields

            /// <summary>
            /// The subscribed delegate.
            /// </summary>
            private volatile Action action;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="S0{T}"/> class.
            /// </summary>
            /// <param name="callback">The delegate to subscribe.</param>
            internal S0(Action callback)
            {
                action = callback;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Processes the property change notification event.
            /// </summary>
            /// <param name="source">The source of the event.</param>
            /// <param name="propertyName">The name of the property that changed.</param>
            internal override void ProcessEvent(T source, string propertyName)
            {
                action();
            }

            /// <summary>
            /// Cancels the subscription of the property change notification.
            /// </summary>
            /// <param name="callback">The delegate to unsubscribe.</param>
            /// <returns><c>true</c> if subscription was successfully canceled; otherwise, <c>false</c>.</returns>
            internal override bool Unsubscribe(Action callback)
            {
                if (callback == action)
                {
                    action = Void0;

                    return true;
                }

                return false;
            }

            #endregion
        }

        /// <summary>
        /// Represents an subscription of the property change notification.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        private class S1<T> : Subscription<T> where T : class, INotifyPropertyChanged
        {
            #region Fields

            /// <summary>
            /// The subscribed delegate.
            /// </summary>
            private volatile Action<T> action;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="S1{T}"/> class.
            /// </summary>
            /// <param name="callback">The delegate to subscribe.</param>
            internal S1(Action<T> callback)
            {
                action = callback;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Processes the property change notification event.
            /// </summary>
            /// <param name="source">The source of the event.</param>
            /// <param name="propertyName">The name of the property that changed.</param>
            internal override void ProcessEvent(T source, string propertyName)
            {
                action(source);
            }

            /// <summary>
            /// Cancels the subscription of the property change notification.
            /// </summary>
            /// <param name="callback">The delegate to unsubscribe.</param>
            /// <returns><c>true</c> if subscription was successfully canceled; otherwise, <c>false</c>.</returns>
            internal override bool Unsubscribe(Action<T> callback)
            {
                if (callback == action)
                {
                    action = Void1;

                    return true;
                }

                return false;
            }

            #endregion
        }

        /// <summary>
        /// Represents an subscription of the property change notification.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        private class SP0<T> : Subscription<T> where T : class, INotifyPropertyChanged
        {
            #region Fields

            /// <summary>
            /// The name of the observed property.
            /// </summary>
            private readonly string name;

            /// <summary>
            /// The subscribed delegate.
            /// </summary>
            private volatile Action action;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="SP0{T}"/> class.
            /// </summary>
            /// <param name="propertyName">The name of the property to observe.</param>
            /// <param name="callback">The delegate to subscribe.</param>
            internal SP0(string propertyName, Action callback)
            {
                name = propertyName;
                action = callback;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Processes the property change notification event.
            /// </summary>
            /// <param name="source">The source of the event.</param>
            /// <param name="propertyName">The name of the property that changed.</param>
            internal override void ProcessEvent(T source, string propertyName)
            {
                if (propertyName == name || propertyName.Length == 0)
                {
                    action();
                }
            }

            /// <summary>
            /// Cancels the subscription of the property change notification.
            /// </summary>
            /// <param name="propertyName">The name of the observed property.</param>
            /// <param name="callback">The delegate to unsubscribe.</param>
            /// <returns><c>true</c> if subscription was successfully canceled; otherwise, <c>false</c>.</returns>
            internal override bool Unsubscribe(string propertyName, Action callback)
            {
                if (propertyName == name && callback == action)
                {
                    action = Void0;

                    return true;
                }

                return false;
            }

            #endregion
        }

        /// <summary>
        /// Represents an subscription of the property change notification.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        private class SP1<T> : Subscription<T> where T : class, INotifyPropertyChanged
        {
            #region Fields

            /// <summary>
            /// The name of the observed property.
            /// </summary>
            private readonly string name;

            /// <summary>
            /// The subscribed delegate.
            /// </summary>
            private volatile Action<T> action;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="SP1{T}"/> class.
            /// </summary>
            /// <param name="propertyName">The name of the property to observe.</param>
            /// <param name="callback">The delegate to subscribe.</param>
            internal SP1(string propertyName, Action<T> callback)
            {
                name = propertyName;
                action = callback;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Processes the property change notification event.
            /// </summary>
            /// <param name="source">The source of the event.</param>
            /// <param name="propertyName">The name of the property that changed.</param>
            internal override void ProcessEvent(T source, string propertyName)
            {
                if (propertyName == name || propertyName.Length == 0)
                {
                    action(source);
                }
            }

            /// <summary>
            /// Cancels the subscription of the property change notification.
            /// </summary>
            /// <param name="propertyName">The name of the observed property.</param>
            /// <param name="callback">The delegate to unsubscribe.</param>
            /// <returns><c>true</c> if subscription was successfully canceled; otherwise, <c>false</c>.</returns>
            internal override bool Unsubscribe(string propertyName, Action<T> callback)
            {
                if (propertyName == name && callback == action)
                {
                    action = Void1;

                    return true;
                }

                return false;
            }

            #endregion
        }

        #endregion
    }
}
