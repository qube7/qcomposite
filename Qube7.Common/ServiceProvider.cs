using System;

namespace Qube7
{
    /// <summary>
    /// Provides helpers for the <see cref="IServiceProvider"/>.
    /// </summary>
    public static class ServiceProvider
    {
        #region Methods

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <param name="provider">The service provider.</param>
        /// <returns>A service object of type <typeparamref name="T"/>, if available; otherwise, <c>null</c>.</returns>
        public static T GetService<T>(this IServiceProvider provider) where T : class
        {
            Requires.NotNull(provider, nameof(provider));

            return provider.GetService(typeof(T)) as T;
        }

        #endregion
    }
}
