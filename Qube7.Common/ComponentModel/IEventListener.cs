namespace Qube7.ComponentModel
{
    /// <summary>
    /// Represents an object that listens for an event.
    /// </summary>
    /// <typeparam name="T">The type of the event data.</typeparam>
    public interface IEventListener<in T>
    {
        #region Methods

        /// <summary>
        /// Called when the subscribed event occurs.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <typeparamref name="T"/> that contains the event data.</param>
        void OnEvent(object sender, T e);

        #endregion
    }
}
