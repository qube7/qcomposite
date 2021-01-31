using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Qube7.Composite
{
    /// <summary>
    /// Provides methods for subscribing and unsubscribing property change notification.
    /// </summary>
    public static class Subscriber
    {
        #region Methods

        /// <summary>
        /// Subscribes the specified delegate for the change notification of the specified property.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="propertySelector">The selector expression of the property to observe.</param>
        /// <param name="callback">The delegate to subscribe.</param>
        /// <returns>The current subscriber.</returns>
        public static ISubscriber<T> Subscribe<T, TProperty>(this ISubscriber<T> subscriber, Expression<Func<T, TProperty>> propertySelector, Action<T> callback) where T : INotifyPropertyChanged
        {
            Requires.NotNull(subscriber, nameof(subscriber));

            return subscriber.Subscribe(GetPropertyName(propertySelector), callback);
        }

        /// <summary>
        /// Subscribes the specified delegate for the change notification of the specified property.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="propertySelector">The selector expression of the property to observe.</param>
        /// <param name="callback">The delegate to subscribe.</param>
        /// <returns>The current subscriber.</returns>
        public static ISubscriber<T> Subscribe<T, TProperty>(this ISubscriber<T> subscriber, Expression<Func<T, TProperty>> propertySelector, Action callback) where T : INotifyPropertyChanged
        {
            Requires.NotNull(subscriber, nameof(subscriber));

            return subscriber.Subscribe(GetPropertyName(propertySelector), callback);
        }

        /// <summary>
        /// Unsubscribes the specified delegate from the change notification of the specified property.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="propertySelector">The selector expression of the observed property.</param>
        /// <param name="callback">The delegate to unsubscribe.</param>
        /// <returns>The current subscriber.</returns>
        public static ISubscriber<T> Unsubscribe<T, TProperty>(this ISubscriber<T> subscriber, Expression<Func<T, TProperty>> propertySelector, Action<T> callback) where T : INotifyPropertyChanged
        {
            Requires.NotNull(subscriber, nameof(subscriber));

            return subscriber.Unsubscribe(GetPropertyName(propertySelector), callback);
        }

        /// <summary>
        /// Unsubscribes the specified delegate from the change notification of the specified property.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="propertySelector">The selector expression of the observed property.</param>
        /// <param name="callback">The delegate to unsubscribe.</param>
        /// <returns>The current subscriber.</returns>
        public static ISubscriber<T> Unsubscribe<T, TProperty>(this ISubscriber<T> subscriber, Expression<Func<T, TProperty>> propertySelector, Action callback) where T : INotifyPropertyChanged
        {
            Requires.NotNull(subscriber, nameof(subscriber));

            return subscriber.Unsubscribe(GetPropertyName(propertySelector), callback);
        }

        /// <summary>
        /// Gets the name of the property determined by the specified selector expression.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertySelector">The selector expression of the property.</param>
        /// <returns>The name of the property.</returns>
        private static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> propertySelector)
        {
            Requires.NotNull(propertySelector, nameof(propertySelector));

            MemberExpression expression = propertySelector.Body as MemberExpression;
            if (expression == null || expression.Expression as ParameterExpression == null || expression.Member as PropertyInfo == null)
            {
                throw Error.Argument(Format.Current(Strings.PropertyExpressionInvalid, typeof(T)), nameof(propertySelector));
            }

            return expression.Member.Name;
        }

        #endregion
    }
}
