namespace Qube7
{
    /// <summary>
    /// Provides static method for returning an array of objects specified as <c>params</c> parameter.
    /// </summary>
    public static class Params
    {
        #region Methods

        /// <summary>
        /// Returns the specified array of objects.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="items"/>.</typeparam>
        /// <param name="items">An array of objects to return.</param>
        /// <returns>The <paramref name="items"/> array.</returns>
        public static T[] Array<T>(params T[] items)
        {
            return items;
        }

        #endregion
    }
}
