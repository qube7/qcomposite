using System;
using Qube7.ComponentModel;

namespace Qube7.Composite
{
    /// <summary>
    /// Provides a weak event implementation for the <see cref="Culture.Current"/> property changed event.
    /// </summary>
    /// <threadsafety static="true" instance="true"/>
    public sealed class CultureChangedEvent : WeakEvent<EventArgs, CultureChangedEvent>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CultureChangedEvent"/> class.
        /// </summary>
        public CultureChangedEvent()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified listener for the event being managed.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        internal static void AddListener(IEventListener<EventArgs> listener)
        {
            ProtectedAddListener(listener);
        }

        /// <summary>
        /// Removes the specified listener for the event being managed.
        /// </summary>
        /// <param name="listener">The listener to remove.</param>
        internal static void RemoveListener(IEventListener<EventArgs> listener)
        {
            ProtectedRemoveListener(listener);
        }

        /// <summary>
        /// Subscribes the specified handler delegate for the event being managed.
        /// </summary>
        /// <param name="handler">The handler delegate to subscribe.</param>
        /// <returns>An <see cref="Subscription"/> used for unsubscribing from the event being managed.</returns>
        public static Subscription Subscribe(EventHandler handler)
        {
            Requires.NotNull(handler, nameof(handler));

            return new Subscription(handler);
        }

        /// <summary>
        /// Starts listening for the event being managed.
        /// </summary>
        protected override void StartListening()
        {
            Culture.CurrentChanged += OnEvent;
        }

        /// <summary>
        /// Stops listening for the event being managed.
        /// </summary>
        protected override void StopListening()
        {
            Culture.CurrentChanged -= OnEvent;
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents a cancelable <see cref="CultureChangedEvent"/> subscription.
        /// </summary>
        /// <threadsafety static="true" instance="true"/>
        public class Subscription : Subscription<EventArgs, CultureChangedEvent>
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Subscription"/> class.
            /// </summary>
            /// <param name="handler">The handler delegate to subscribe.</param>
            internal Subscription(EventHandler handler) : base(EventListener.FromHandler(handler))
            {
            }

            #endregion
        }

        #endregion
    }
}
