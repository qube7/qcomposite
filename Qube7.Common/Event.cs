using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using Qube7.ComponentModel;

namespace Qube7
{
    /// <summary>
    /// Provides helper methods for performing thread-safe event operations.
    /// </summary>
    public static class Event
    {
        #region Methods

        /// <summary>
        /// Raises the specified event handler.
        /// </summary>
        /// <param name="handler">The event handler to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        public static void Raise(EventHandler handler, object sender, EventArgs e)
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        /// Raises the specified event handler passing the <see cref="EventArgs.Empty"/> event data.
        /// </summary>
        /// <param name="handler">The event handler to raise.</param>
        /// <param name="sender">The source of the event.</param>
        public static void Raise(EventHandler handler, object sender)
        {
            Raise(handler, sender, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the specified event handler.
        /// </summary>
        /// <typeparam name="T">The type of the event data.</typeparam>
        /// <param name="handler">The event handler to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <typeparamref name="T"/> that contains the event data.</param>
        public static void Raise<T>(EventHandler<T> handler, object sender, T e)
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        /// Raises the specified event handler.
        /// </summary>
        /// <param name="handler">The event handler to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CancelEventArgs"/> that contains the event data.</param>
        public static void Raise(CancelEventHandler handler, object sender, CancelEventArgs e)
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        /// Raises the specified event handler.
        /// </summary>
        /// <param name="handler">The event handler to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="PropertyChangingEventArgs"/> that contains the event data.</param>
        public static void Raise(PropertyChangingEventHandler handler, object sender, PropertyChangingEventArgs e)
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        /// Raises the specified event handler.
        /// </summary>
        /// <param name="handler">The event handler to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="PropertyChangedEventArgs"/> that contains the event data.</param>
        public static void Raise(PropertyChangedEventHandler handler, object sender, PropertyChangedEventArgs e)
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        /// Raises the specified event handler.
        /// </summary>
        /// <param name="handler">The event handler to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CollectionChangeEventArgs"/> that contains the event data.</param>
        public static void Raise(CollectionChangeEventHandler handler, object sender, CollectionChangeEventArgs e)
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        /// Raises the specified event handler.
        /// </summary>
        /// <param name="handler">The event handler to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="NotifyCollectionChangedEventArgs"/> that contains the event data.</param>
        public static void Raise(NotifyCollectionChangedEventHandler handler, object sender, NotifyCollectionChangedEventArgs e)
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        /// Raises the specified event handler ensuring the event is received by each subscriber.
        /// </summary>
        /// <param name="handler">The event handler to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        /// <exception cref="AggregateException">The exception is thrown by at least one of the subscribers.</exception>
        public static void RaiseSafe(EventHandler handler, object sender, EventArgs e)
        {
            if (handler != null)
            {
                List<Exception> errors = null;

                Delegate[] list = handler.GetInvocationList();
                for (int i = 0; i < list.Length; i++)
                {
                    try
                    {
                        (list[i] as EventHandler)(sender, e);
                    }
                    catch (Exception exception)
                    {
                        if (errors == null)
                        {
                            errors = new List<Exception>(1);
                        }

                        errors.Add(exception);
                    }
                }

                if (errors != null)
                {
                    throw Error.Aggregate(errors.ToArray());
                }
            }
        }

        /// <summary>
        /// Raises the specified event handler passing the <see cref="EventArgs.Empty"/> event data ensuring the event is received by each subscriber.
        /// </summary>
        /// <param name="handler">The event handler to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <exception cref="AggregateException">The exception is thrown by at least one of the subscribers.</exception>
        public static void RaiseSafe(EventHandler handler, object sender)
        {
            RaiseSafe(handler, sender, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the specified event handler ensuring the event is received by each subscriber.
        /// </summary>
        /// <typeparam name="T">The type of the event data.</typeparam>
        /// <param name="handler">The event handler to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <typeparamref name="T"/> that contains the event data.</param>
        /// <exception cref="AggregateException">The exception is thrown by at least one of the subscribers.</exception>
        public static void RaiseSafe<T>(EventHandler<T> handler, object sender, T e)
        {
            if (handler != null)
            {
                List<Exception> errors = null;

                Delegate[] list = handler.GetInvocationList();
                for (int i = 0; i < list.Length; i++)
                {
                    try
                    {
                        (list[i] as EventHandler<T>)(sender, e);
                    }
                    catch (Exception exception)
                    {
                        if (errors == null)
                        {
                            errors = new List<Exception>(1);
                        }

                        errors.Add(exception);
                    }
                }

                if (errors != null)
                {
                    throw Error.Aggregate(errors.ToArray());
                }
            }
        }

        /// <summary>
        /// Raises the specified event handler ensuring the event is received by each subscriber.
        /// </summary>
        /// <param name="handler">The event handler to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CancelEventArgs"/> that contains the event data.</param>
        /// <exception cref="AggregateException">The exception is thrown by at least one of the subscribers.</exception>
        public static void RaiseSafe(CancelEventHandler handler, object sender, CancelEventArgs e)
        {
            if (handler != null)
            {
                List<Exception> errors = null;

                Delegate[] list = handler.GetInvocationList();
                for (int i = 0; i < list.Length; i++)
                {
                    try
                    {
                        (list[i] as CancelEventHandler)(sender, e);
                    }
                    catch (Exception exception)
                    {
                        if (errors == null)
                        {
                            errors = new List<Exception>(1);
                        }

                        errors.Add(exception);
                    }
                }

                if (errors != null)
                {
                    throw Error.Aggregate(errors.ToArray());
                }
            }
        }

        /// <summary>
        /// Raises the specified event handler ensuring the event is received by each subscriber.
        /// </summary>
        /// <param name="handler">The event handler to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="PropertyChangingEventArgs"/> that contains the event data.</param>
        /// <exception cref="AggregateException">The exception is thrown by at least one of the subscribers.</exception>
        public static void RaiseSafe(PropertyChangingEventHandler handler, object sender, PropertyChangingEventArgs e)
        {
            if (handler != null)
            {
                List<Exception> errors = null;

                Delegate[] list = handler.GetInvocationList();
                for (int i = 0; i < list.Length; i++)
                {
                    try
                    {
                        (list[i] as PropertyChangingEventHandler)(sender, e);
                    }
                    catch (Exception exception)
                    {
                        if (errors == null)
                        {
                            errors = new List<Exception>(1);
                        }

                        errors.Add(exception);
                    }
                }

                if (errors != null)
                {
                    throw Error.Aggregate(errors.ToArray());
                }
            }
        }

        /// <summary>
        /// Raises the specified event handler ensuring the event is received by each subscriber.
        /// </summary>
        /// <param name="handler">The event handler to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="PropertyChangedEventArgs"/> that contains the event data.</param>
        /// <exception cref="AggregateException">The exception is thrown by at least one of the subscribers.</exception>
        public static void RaiseSafe(PropertyChangedEventHandler handler, object sender, PropertyChangedEventArgs e)
        {
            if (handler != null)
            {
                List<Exception> errors = null;

                Delegate[] list = handler.GetInvocationList();
                for (int i = 0; i < list.Length; i++)
                {
                    try
                    {
                        (list[i] as PropertyChangedEventHandler)(sender, e);
                    }
                    catch (Exception exception)
                    {
                        if (errors == null)
                        {
                            errors = new List<Exception>(1);
                        }

                        errors.Add(exception);
                    }
                }

                if (errors != null)
                {
                    throw Error.Aggregate(errors.ToArray());
                }
            }
        }

        /// <summary>
        /// Raises the specified event handler ensuring the event is received by each subscriber.
        /// </summary>
        /// <param name="handler">The event handler to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CollectionChangeEventArgs"/> that contains the event data.</param>
        /// <exception cref="AggregateException">The exception is thrown by at least one of the subscribers.</exception>
        public static void RaiseSafe(CollectionChangeEventHandler handler, object sender, CollectionChangeEventArgs e)
        {
            if (handler != null)
            {
                List<Exception> errors = null;

                Delegate[] list = handler.GetInvocationList();
                for (int i = 0; i < list.Length; i++)
                {
                    try
                    {
                        (list[i] as CollectionChangeEventHandler)(sender, e);
                    }
                    catch (Exception exception)
                    {
                        if (errors == null)
                        {
                            errors = new List<Exception>(1);
                        }

                        errors.Add(exception);
                    }
                }

                if (errors != null)
                {
                    throw Error.Aggregate(errors.ToArray());
                }
            }
        }

        /// <summary>
        /// Raises the specified event handler ensuring the event is received by each subscriber.
        /// </summary>
        /// <param name="handler">The event handler to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="NotifyCollectionChangedEventArgs"/> that contains the event data.</param>
        /// <exception cref="AggregateException">The exception is thrown by at least one of the subscribers.</exception>
        public static void RaiseSafe(NotifyCollectionChangedEventHandler handler, object sender, NotifyCollectionChangedEventArgs e)
        {
            if (handler != null)
            {
                List<Exception> errors = null;

                Delegate[] list = handler.GetInvocationList();
                for (int i = 0; i < list.Length; i++)
                {
                    try
                    {
                        (list[i] as NotifyCollectionChangedEventHandler)(sender, e);
                    }
                    catch (Exception exception)
                    {
                        if (errors == null)
                        {
                            errors = new List<Exception>(1);
                        }

                        errors.Add(exception);
                    }
                }

                if (errors != null)
                {
                    throw Error.Aggregate(errors.ToArray());
                }
            }
        }

        /// <summary>
        /// Raises the specified event handler breaking the invocation if the event has been canceled.
        /// </summary>
        /// <typeparam name="T">The type of the event data.</typeparam>
        /// <param name="handler">The event handler to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <typeparamref name="T"/> that contains the event data.</param>
        public static void RaiseCancel<T>(EventHandler<T> handler, object sender, T e) where T : CancelableEventArgs
        {
            Requires.NotNull(e, nameof(e));

            if (handler != null)
            {
                if (e.CanCancel)
                {
                    Delegate[] list = handler.GetInvocationList();
                    for (int i = 0; i < list.Length; i++)
                    {
                        if (e.IsCanceled)
                        {
                            return;
                        }

                        (list[i] as EventHandler<T>)(sender, e);
                    }

                    return;
                }

                handler(sender, e);
            }
        }

        /// <summary>
        /// Raises the specified event handler breaking the invocation if the event has been canceled.
        /// </summary>
        /// <param name="handler">The event handler to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CancelEventArgs"/> that contains the event data.</param>
        public static void RaiseCancel(CancelEventHandler handler, object sender, CancelEventArgs e)
        {
            Requires.NotNull(e, nameof(e));

            if (handler != null)
            {
                Delegate[] list = handler.GetInvocationList();
                for (int i = 0; i < list.Length; i++)
                {
                    if (e.Cancel)
                    {
                        return;
                    }

                    (list[i] as CancelEventHandler)(sender, e);
                }
            }
        }

        /// <summary>
        /// Subscribes the specified event handler.
        /// </summary>
        /// <param name="handler">The event handler to subscribe to.</param>
        /// <param name="value">The event handler to subscribe.</param>
        public static void Subscribe(ref EventHandler handler, EventHandler value)
        {
            EventHandler comparand;
            EventHandler location = handler;
            do
            {
                comparand = location;

                location = Interlocked.CompareExchange(ref handler, (EventHandler)Delegate.Combine(comparand, value), comparand);
            }
            while (location != comparand);
        }

        /// <summary>
        /// Unsubscribes the specified event handler.
        /// </summary>
        /// <param name="handler">The event handler to unsubscribe from.</param>
        /// <param name="value">The event handler to unsubscribe.</param>
        public static void Unsubscribe(ref EventHandler handler, EventHandler value)
        {
            EventHandler comparand;
            EventHandler location = handler;
            do
            {
                comparand = location;

                location = Interlocked.CompareExchange(ref handler, (EventHandler)Delegate.Remove(comparand, value), comparand);
            }
            while (location != comparand);
        }

        /// <summary>
        /// Subscribes the specified event handler.
        /// </summary>
        /// <typeparam name="T">The type of the event data.</typeparam>
        /// <param name="handler">The event handler to subscribe to.</param>
        /// <param name="value">The event handler to subscribe.</param>
        public static void Subscribe<T>(ref EventHandler<T> handler, EventHandler<T> value)
        {
            EventHandler<T> comparand;
            EventHandler<T> location = handler;
            do
            {
                comparand = location;

                location = Interlocked.CompareExchange(ref handler, (EventHandler<T>)Delegate.Combine(comparand, value), comparand);
            }
            while (location != comparand);
        }

        /// <summary>
        /// Unsubscribes the specified event handler.
        /// </summary>
        /// <typeparam name="T">The type of the event data.</typeparam>
        /// <param name="handler">The event handler to unsubscribe from.</param>
        /// <param name="value">The event handler to unsubscribe.</param>
        public static void Unsubscribe<T>(ref EventHandler<T> handler, EventHandler<T> value)
        {
            EventHandler<T> comparand;
            EventHandler<T> location = handler;
            do
            {
                comparand = location;

                location = Interlocked.CompareExchange(ref handler, (EventHandler<T>)Delegate.Remove(comparand, value), comparand);
            }
            while (location != comparand);
        }

        /// <summary>
        /// Subscribes the specified event handler.
        /// </summary>
        /// <param name="handler">The event handler to subscribe to.</param>
        /// <param name="value">The event handler to subscribe.</param>
        public static void Subscribe(ref CancelEventHandler handler, CancelEventHandler value)
        {
            CancelEventHandler comparand;
            CancelEventHandler location = handler;
            do
            {
                comparand = location;

                location = Interlocked.CompareExchange(ref handler, (CancelEventHandler)Delegate.Combine(comparand, value), comparand);
            }
            while (location != comparand);
        }

        /// <summary>
        /// Unsubscribes the specified event handler.
        /// </summary>
        /// <param name="handler">The event handler to unsubscribe from.</param>
        /// <param name="value">The event handler to unsubscribe.</param>
        public static void Unsubscribe(ref CancelEventHandler handler, CancelEventHandler value)
        {
            CancelEventHandler comparand;
            CancelEventHandler location = handler;
            do
            {
                comparand = location;

                location = Interlocked.CompareExchange(ref handler, (CancelEventHandler)Delegate.Remove(comparand, value), comparand);
            }
            while (location != comparand);
        }

        /// <summary>
        /// Subscribes the specified event handler.
        /// </summary>
        /// <param name="handler">The event handler to subscribe to.</param>
        /// <param name="value">The event handler to subscribe.</param>
        public static void Subscribe(ref PropertyChangingEventHandler handler, PropertyChangingEventHandler value)
        {
            PropertyChangingEventHandler comparand;
            PropertyChangingEventHandler location = handler;
            do
            {
                comparand = location;

                location = Interlocked.CompareExchange(ref handler, (PropertyChangingEventHandler)Delegate.Combine(comparand, value), comparand);
            }
            while (location != comparand);
        }

        /// <summary>
        /// Unsubscribes the specified event handler.
        /// </summary>
        /// <param name="handler">The event handler to unsubscribe from.</param>
        /// <param name="value">The event handler to unsubscribe.</param>
        public static void Unsubscribe(ref PropertyChangingEventHandler handler, PropertyChangingEventHandler value)
        {
            PropertyChangingEventHandler comparand;
            PropertyChangingEventHandler location = handler;
            do
            {
                comparand = location;

                location = Interlocked.CompareExchange(ref handler, (PropertyChangingEventHandler)Delegate.Remove(comparand, value), comparand);
            }
            while (location != comparand);
        }

        /// <summary>
        /// Subscribes the specified event handler.
        /// </summary>
        /// <param name="handler">The event handler to subscribe to.</param>
        /// <param name="value">The event handler to subscribe.</param>
        public static void Subscribe(ref PropertyChangedEventHandler handler, PropertyChangedEventHandler value)
        {
            PropertyChangedEventHandler comparand;
            PropertyChangedEventHandler location = handler;
            do
            {
                comparand = location;

                location = Interlocked.CompareExchange(ref handler, (PropertyChangedEventHandler)Delegate.Combine(comparand, value), comparand);
            }
            while (location != comparand);
        }

        /// <summary>
        /// Unsubscribes the specified event handler.
        /// </summary>
        /// <param name="handler">The event handler to unsubscribe from.</param>
        /// <param name="value">The event handler to unsubscribe.</param>
        public static void Unsubscribe(ref PropertyChangedEventHandler handler, PropertyChangedEventHandler value)
        {
            PropertyChangedEventHandler comparand;
            PropertyChangedEventHandler location = handler;
            do
            {
                comparand = location;

                location = Interlocked.CompareExchange(ref handler, (PropertyChangedEventHandler)Delegate.Remove(comparand, value), comparand);
            }
            while (location != comparand);
        }

        /// <summary>
        /// Subscribes the specified event handler.
        /// </summary>
        /// <param name="handler">The event handler to subscribe to.</param>
        /// <param name="value">The event handler to subscribe.</param>
        public static void Subscribe(ref CollectionChangeEventHandler handler, CollectionChangeEventHandler value)
        {
            CollectionChangeEventHandler comparand;
            CollectionChangeEventHandler location = handler;
            do
            {
                comparand = location;

                location = Interlocked.CompareExchange(ref handler, (CollectionChangeEventHandler)Delegate.Combine(comparand, value), comparand);
            }
            while (location != comparand);
        }

        /// <summary>
        /// Unsubscribes the specified event handler.
        /// </summary>
        /// <param name="handler">The event handler to unsubscribe from.</param>
        /// <param name="value">The event handler to unsubscribe.</param>
        public static void Unsubscribe(ref CollectionChangeEventHandler handler, CollectionChangeEventHandler value)
        {
            CollectionChangeEventHandler comparand;
            CollectionChangeEventHandler location = handler;
            do
            {
                comparand = location;

                location = Interlocked.CompareExchange(ref handler, (CollectionChangeEventHandler)Delegate.Remove(comparand, value), comparand);
            }
            while (location != comparand);
        }

        /// <summary>
        /// Subscribes the specified event handler.
        /// </summary>
        /// <param name="handler">The event handler to subscribe to.</param>
        /// <param name="value">The event handler to subscribe.</param>
        public static void Subscribe(ref NotifyCollectionChangedEventHandler handler, NotifyCollectionChangedEventHandler value)
        {
            NotifyCollectionChangedEventHandler comparand;
            NotifyCollectionChangedEventHandler location = handler;
            do
            {
                comparand = location;

                location = Interlocked.CompareExchange(ref handler, (NotifyCollectionChangedEventHandler)Delegate.Combine(comparand, value), comparand);
            }
            while (location != comparand);
        }

        /// <summary>
        /// Unsubscribes the specified event handler.
        /// </summary>
        /// <param name="handler">The event handler to unsubscribe from.</param>
        /// <param name="value">The event handler to unsubscribe.</param>
        public static void Unsubscribe(ref NotifyCollectionChangedEventHandler handler, NotifyCollectionChangedEventHandler value)
        {
            NotifyCollectionChangedEventHandler comparand;
            NotifyCollectionChangedEventHandler location = handler;
            do
            {
                comparand = location;

                location = Interlocked.CompareExchange(ref handler, (NotifyCollectionChangedEventHandler)Delegate.Remove(comparand, value), comparand);
            }
            while (location != comparand);
        }

        #endregion
    }
}
