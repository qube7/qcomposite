using System.Collections.Specialized;

namespace Qube7.ComponentModel
{
    /// <summary>
    /// Provides a weak event implementation for the <see cref="INotifyCollectionChanged.CollectionChanged"/> event.
    /// </summary>
    /// <threadsafety static="true" instance="true"/>
    public sealed class CollectionChangedEvent : WeakEvent<INotifyCollectionChanged, NotifyCollectionChangedEventArgs, CollectionChangedEvent>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionChangedEvent"/> class.
        /// </summary>
        public CollectionChangedEvent()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified listener to the specified source object for the event being managed.
        /// </summary>
        /// <param name="source">The source object to add listener to.</param>
        /// <param name="listener">The listener to add.</param>
        public static void AddListener(INotifyCollectionChanged source, IEventListener<NotifyCollectionChangedEventArgs> listener)
        {
            ProtectedAddListener(source, listener);
        }

        /// <summary>
        /// Removes the specified listener from the specified source object for the event being managed.
        /// </summary>
        /// <param name="source">The source object to remove listener from.</param>
        /// <param name="listener">The listener to remove.</param>
        public static void RemoveListener(INotifyCollectionChanged source, IEventListener<NotifyCollectionChangedEventArgs> listener)
        {
            ProtectedRemoveListener(source, listener);
        }

        /// <summary>
        /// Subscribes the specified handler delegate to the specified source object for the event being managed.
        /// </summary>
        /// <param name="source">The source object to subscribe to.</param>
        /// <param name="handler">The handler delegate to subscribe.</param>
        /// <returns>An <see cref="Subscription"/> used for unsubscribing from the event being managed.</returns>
        public static Subscription Subscribe(INotifyCollectionChanged source, NotifyCollectionChangedEventHandler handler)
        {
            Requires.NotNull(handler, nameof(handler));

            return new Subscription(source, handler);
        }

        /// <summary>
        /// Starts listening on the specified source object for the event being managed.
        /// </summary>
        /// <param name="source">The source object to start listening on.</param>
        protected override void StartListening(INotifyCollectionChanged source)
        {
            source.CollectionChanged += OnEvent;
        }

        /// <summary>
        /// Stops listening on the specified source object for the event being managed.
        /// </summary>
        /// <param name="source">The source object to stop listening on.</param>
        protected override void StopListening(INotifyCollectionChanged source)
        {
            source.CollectionChanged -= OnEvent;
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents a cancelable <see cref="CollectionChangedEvent"/> subscription.
        /// </summary>
        /// <threadsafety static="true" instance="true"/>
        public class Subscription : Subscription<INotifyCollectionChanged, NotifyCollectionChangedEventArgs, CollectionChangedEvent>
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Subscription"/> class.
            /// </summary>
            /// <param name="source">The source object to subscribe to.</param>
            /// <param name="handler">The handler delegate to subscribe.</param>
            internal Subscription(INotifyCollectionChanged source, NotifyCollectionChangedEventHandler handler) : base(source, new EventListener(handler))
            {
            }

            #endregion

            #region Nested types

            /// <summary>
            /// Represents a event listener that delegates the event-handling method.
            /// </summary>
            private class EventListener : EventListener<NotifyCollectionChangedEventArgs, NotifyCollectionChangedEventHandler>
            {
                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="EventListener"/> class.
                /// </summary>
                /// <param name="handler">The event handler delegate.</param>
                internal EventListener(NotifyCollectionChangedEventHandler handler) : base(handler)
                {
                }

                #endregion

                #region Methods

                /// <summary>
                /// Handles the subscribed event using the specified handler delegate.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">An <see cref="NotifyCollectionChangedEventArgs"/> that contains the event data.</param>
                /// <param name="handler">The event handler delegate.</param>
                protected override void HandleEvent(object sender, NotifyCollectionChangedEventArgs e, NotifyCollectionChangedEventHandler handler)
                {
                    handler(sender, e);
                }

                #endregion
            }

            #endregion
        }

        #endregion
    }
}
