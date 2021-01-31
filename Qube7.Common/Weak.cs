namespace Qube7
{
    /// <summary>
    /// Provides static method for creating <see cref="Weak{T}"/> objects.
    /// </summary>
    public static class Weak
    {
        #region Methods

        /// <summary>
        /// Creates a new <see cref="Weak{T}"/> for the specified target object.
        /// </summary>
        /// <typeparam name="T">The type of the target object.</typeparam>
        /// <param name="target">The target object to track.</param>
        /// <returns>A <see cref="Weak{T}"/> for the <paramref name="target"/>.</returns>
        public static Weak<T> Create<T>(T target) where T : class
        {
            return new Weak<T>(target);
        }

        #endregion
    }
}
