namespace Qube7.ComponentModel
{
    /// <summary>
    /// Provides helpers for the <see cref="IInitializable"/>.
    /// </summary>
    public static class Initializable
    {
        #region Methods

        /// <summary>
        /// Initializes the specified object instance.
        /// </summary>
        /// <typeparam name="T">The type of the object to initialize.</typeparam>
        /// <param name="initializable">The object to initialize.</param>
        public static void Initialize<T>(T initializable) where T : IInitializable
        {
            if (initializable != null)
            {
                initializable.Initialize();
            }
        }

        /// <summary>
        /// Initializes the specified object instance if it implements the <see cref="IInitializable"/>.
        /// </summary>
        /// <param name="instance">The object to initialize.</param>
        public static void Initialize(object instance)
        {
            IInitializable initializable = instance as IInitializable;
            if (initializable != null)
            {
                initializable.Initialize();
            }
        }

        /// <summary>
        /// Creates and initializes a new instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to create.</typeparam>
        /// <returns>The initialized object instance.</returns>
        public static T Create<T>() where T : IInitializable, new()
        {
            T instance = new T();

            instance.Initialize();

            return instance;
        }

        #endregion
    }
}
