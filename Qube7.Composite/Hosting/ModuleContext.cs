using System;
using Qube7.ComponentModel;

namespace Qube7.Composite.Hosting
{
    /// <summary>
    /// Manages the lifetime of associated module object.
    /// </summary>
    public abstract class ModuleContext : IInitializable, IDisposable
    {
        #region Fields

        /// <summary>
        /// The associated module instance.
        /// </summary>
        private Module module;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the module instance associated with this <see cref="ModuleContext"/>.
        /// </summary>
        /// <value>The associated module instance.</value>
        public Module Module
        {
            get { return module; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleContext"/> class.
        /// </summary>
        protected ModuleContext()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes this instance of the <see cref="ModuleContext"/>.
        /// </summary>
        void IInitializable.Initialize()
        {
            if (module == null)
            {
                module = CreateModule(this);
            }
        }

        /// <summary>
        /// Creates the module instance to be associated with this <see cref="ModuleContext"/>.
        /// </summary>
        /// <returns>The created module object instance.</returns>
        protected abstract Module CreateModule();

        /// <summary>
        /// Creates the module instance to be associated with the specified <see cref="ModuleContext"/>.
        /// </summary>
        /// <param name="context">The associated <see cref="ModuleContext"/>.</param>
        /// <returns>The created module object instance.</returns>
        private static Module CreateModule(ModuleContext context)
        {
            Module module = context.CreateModule();

            if (module == null)
            {
                throw Error.ReturnsNull(nameof(CreateModule));
            }

            module.Initialize();

            return module;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="ModuleContext"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources used by the <see cref="ModuleContext"/>.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposable.Dispose(module);

                module = null;
            }
        }

        #endregion
    }
}
