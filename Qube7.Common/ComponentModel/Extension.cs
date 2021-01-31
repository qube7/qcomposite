namespace Qube7.ComponentModel
{
    /// <summary>
    /// Represents an object that extend another object through aggregation.
    /// </summary>
    /// <typeparam name="T">The type of object that participates in the custom behavior.</typeparam>
    public abstract class Extension<T> where T : class
    {
        #region Methods

        /// <summary>
        /// When overridden in a derived class, enables an extension object to find out when it has been aggregated.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</param>
        /// <remarks>This method is called when the extension is added to the <see cref="ExtensionCollection{T}"/>.</remarks>
        protected internal abstract void Attach(T owner);

        /// <summary>
        /// When overridden in a derived class, enables an object to find out when it is no longer aggregated.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</param>
        /// <remarks>This method is called when an extension is removed from the <see cref="ExtensionCollection{T}"/>.</remarks>
        protected internal abstract void Detach(T owner);

        #endregion
    }
}
