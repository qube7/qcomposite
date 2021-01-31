using System.ComponentModel;

namespace Qube7.ComponentModel
{
    /// <summary>
    /// Provides a weak event implementation for the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
    /// </summary>
    /// <threadsafety static="true" instance="true"/>
    public sealed class PropertyChangedEvent : WeakEvent<INotifyPropertyChanged, PropertyChangedEventArgs, PropertyChangedEvent>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedEvent"/> class.
        /// </summary>
        public PropertyChangedEvent()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified listener to the specified source object for the event being managed.
        /// </summary>
        /// <param name="source">The source object to add listener to.</param>
        /// <param name="listener">The listener to add.</param>
        public static void AddListener(INotifyPropertyChanged source, IEventListener<PropertyChangedEventArgs> listener)
        {
            ProtectedAddListener(source, listener);
        }

        /// <summary>
        /// Removes the specified listener from the specified source object for the event being managed.
        /// </summary>
        /// <param name="source">The source object to remove listener from.</param>
        /// <param name="listener">The listener to remove.</param>
        public static void RemoveListener(INotifyPropertyChanged source, IEventListener<PropertyChangedEventArgs> listener)
        {
            ProtectedRemoveListener(source, listener);
        }

        /// <summary>
        /// Subscribes the specified handler delegate to the specified source object for the event being managed.
        /// </summary>
        /// <param name="source">The source object to subscribe to.</param>
        /// <param name="handler">The handler delegate to subscribe.</param>
        /// <returns>An <see cref="Subscription"/> used for unsubscribing from the event being managed.</returns>
        public static Subscription Subscribe(INotifyPropertyChanged source, PropertyChangedEventHandler handler)
        {
            Requires.NotNull(handler, nameof(handler));

            return new Subscription(source, handler);
        }

        /// <summary>
        /// Starts listening on the specified source object for the event being managed.
        /// </summary>
        /// <param name="source">The source object to start listening on.</param>
        protected override void StartListening(INotifyPropertyChanged source)
        {
            source.PropertyChanged += OnEvent;
        }

        /// <summary>
        /// Stops listening on the specified source object for the event being managed.
        /// </summary>
        /// <param name="source">The source object to stop listening on.</param>
        protected override void StopListening(INotifyPropertyChanged source)
        {
            source.PropertyChanged -= OnEvent;
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents a cancelable <see cref="PropertyChangedEvent"/> subscription.
        /// </summary>
        /// <threadsafety static="true" instance="true"/>
        public class Subscription : Subscription<INotifyPropertyChanged, PropertyChangedEventArgs, PropertyChangedEvent>
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Subscription"/> class.
            /// </summary>
            /// <param name="source">The source object to subscribe to.</param>
            /// <param name="handler">The handler delegate to subscribe.</param>
            internal Subscription(INotifyPropertyChanged source, PropertyChangedEventHandler handler) : base(source, new EventListener(handler))
            {
            }

            #endregion

            #region Nested types

            /// <summary>
            /// Represents a event listener that delegates the event-handling method.
            /// </summary>
            private class EventListener : EventListener<PropertyChangedEventArgs, PropertyChangedEventHandler>
            {
                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="EventListener"/> class.
                /// </summary>
                /// <param name="handler">The event handler delegate.</param>
                internal EventListener(PropertyChangedEventHandler handler) : base(handler)
                {
                }

                #endregion

                #region Methods

                /// <summary>
                /// Handles the subscribed event using the specified handler delegate.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">An <see cref="PropertyChangedEventArgs"/> that contains the event data.</param>
                /// <param name="handler">The event handler delegate.</param>
                protected override void HandleEvent(object sender, PropertyChangedEventArgs e, PropertyChangedEventHandler handler)
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
