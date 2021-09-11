using System;
using System.Reflection;

namespace Qube7
{
    /// <summary>
    /// Creates the common error exception objects.
    /// </summary>
    public static class Error
    {
        #region Methods

        /// <summary>
        /// Creates the <see cref="ArgumentException"/> instance.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <returns>The <see cref="ArgumentException"/> instance.</returns>
        public static ArgumentException Argument(string message, string paramName)
        {
            Requires.NotNullOrEmpty(message, nameof(message));
            Requires.NotNullOrEmpty(paramName, nameof(paramName));

            return new ArgumentException(message, paramName);
        }

        /// <summary>
        /// Creates the <see cref="ArgumentException"/> instance.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <returns>The <see cref="ArgumentException"/> instance.</returns>
        public static ArgumentException Argument(string message)
        {
            Requires.NotNullOrEmpty(message, nameof(message));

            return new ArgumentException(message);
        }

        /// <summary>
        /// Creates the <see cref="ArgumentNullException"/> instance.
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <returns>The <see cref="ArgumentNullException"/> instance.</returns>
        public static ArgumentNullException ArgumentNull(string paramName)
        {
            Requires.NotNullOrEmpty(paramName, nameof(paramName));

            return new ArgumentNullException(paramName);
        }

        /// <summary>
        /// Creates the <see cref="ArgumentOutOfRangeException"/> instance.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <returns>The <see cref="ArgumentOutOfRangeException"/> instance.</returns>
        public static ArgumentOutOfRangeException ArgumentOutOfRange(string message, string paramName)
        {
            Requires.NotNullOrEmpty(message, nameof(message));
            Requires.NotNullOrEmpty(paramName, nameof(paramName));

            return new ArgumentOutOfRangeException(paramName, message);
        }

        /// <summary>
        /// Creates the <see cref="InvalidOperationException"/> instance.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <returns>The <see cref="InvalidOperationException"/> instance.</returns>
        public static InvalidOperationException InvalidOperation(string message)
        {
            Requires.NotNullOrEmpty(message, nameof(message));

            return new InvalidOperationException(message);
        }

        /// <summary>
        /// Creates the <see cref="InvalidOperationException"/> instance.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// <returns>The <see cref="InvalidOperationException"/> instance.</returns>
        public static InvalidOperationException InvalidOperation(string message, Exception innerException)
        {
            Requires.NotNullOrEmpty(message, nameof(message));
            Requires.NotNull(innerException, nameof(innerException));

            return new InvalidOperationException(message, innerException);
        }

        /// <summary>
        /// Creates the <see cref="ObjectDisposedException"/> instance.
        /// </summary>
        /// <param name="objectName">The name of the disposed object.</param>
        /// <returns>The <see cref="ObjectDisposedException"/> instance.</returns>
        public static ObjectDisposedException ObjectDisposed(string objectName)
        {
            Requires.NotNullOrEmpty(objectName, nameof(objectName));

            return new ObjectDisposedException(objectName);
        }

        /// <summary>
        /// Creates the <see cref="ObjectDisposedException"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="instance">The disposed object.</param>
        /// <returns>The <see cref="ObjectDisposedException"/> instance.</returns>
        public static ObjectDisposedException ObjectDisposed<T>(T instance) where T : class
        {
            Requires.NotNull(instance, nameof(instance));

            return new ObjectDisposedException(instance.ToString());
        }

        /// <summary>
        /// Creates the <see cref="NotSupportedException"/> instance.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <returns>The <see cref="NotSupportedException"/> instance.</returns>
        public static NotSupportedException NotSupported(string message)
        {
            Requires.NotNullOrEmpty(message, nameof(message));

            return new NotSupportedException(message);
        }

        /// <summary>
        /// Creates the <see cref="NotImplementedException"/> instance.
        /// </summary>
        /// <returns>The <see cref="NotImplementedException"/> instance.</returns>
        public static NotImplementedException NotImplemented()
        {
            return new NotImplementedException();
        }

        /// <summary>
        /// Creates the <see cref="NotImplementedException"/> instance for the not overridden member.
        /// </summary>
        /// <param name="memberName">The name of the not overridden member.</param>
        /// <returns>The <see cref="NotImplementedException"/> instance.</returns>
        public static NotImplementedException NotOverridden(string memberName)
        {
            Requires.NotNullOrEmpty(memberName, nameof(memberName));

            return new NotImplementedException(string.Format(Strings.NotOverridden, memberName));
        }

        /// <summary>
        /// Creates the <see cref="InvalidOperationException"/> instance for the member that returns <c>null</c> instead of expected not <c>null</c> value.
        /// </summary>
        /// <param name="memberName">The name of the member that returns <c>null</c> value.</param>
        /// <returns>The <see cref="InvalidOperationException"/> instance.</returns>
        public static InvalidOperationException ReturnsNull(string memberName)
        {
            Requires.NotNullOrEmpty(memberName, nameof(memberName));

            return new InvalidOperationException(string.Format(Strings.ReturnsNull, memberName));
        }

        /// <summary>
        /// Creates the <see cref="InvalidOperationException"/> instance for the member that does not have the attribute of the specified type applied.
        /// </summary>
        /// <param name="attributeType">The type of the expected attribute.</param>
        /// <param name="attributeProvider">The member that does not have the attribute applied.</param>
        /// <returns>The <see cref="InvalidOperationException"/> instance.</returns>
        public static InvalidOperationException AttributeNotDefined(Type attributeType, ICustomAttributeProvider attributeProvider)
        {
            Requires.NotNull(attributeType, nameof(attributeType));
            Requires.NotNull(attributeProvider, nameof(attributeProvider));

            return new InvalidOperationException(string.Format(Strings.AttributeNotDefined, attributeType, attributeProvider));
        }

        /// <summary>
        /// Creates the <see cref="AggregateException"/> instance.
        /// </summary>
        /// <param name="exceptions">The inner exceptions.</param>
        /// <returns>The <see cref="AggregateException"/> instance.</returns>
        public static AggregateException Aggregate(params Exception[] exceptions)
        {
            return new AggregateException(exceptions);
        }

        #endregion
    }
}
