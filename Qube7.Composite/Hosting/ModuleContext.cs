using System;
using System.Threading;

namespace Qube7.Composite.Hosting
{
    /// <summary>
    /// Manages the lifetime of associated module object.
    /// </summary>
    public abstract class ModuleContext : IDisposable
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
        internal Module Module
        {
            get
            {
                if (module == null)
                {
                    Module created = CreateModule();

                    if (created == null)
                    {
                        throw Error.ReturnsNull(nameof(CreateModule));
                    }

                    created.Initialize();

                    Interlocked.CompareExchange(ref module, created, null);
                }

                return module;
            }
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
        /// Creates the module instance to be associated with this <see cref="ModuleContext"/>.
        /// </summary>
        /// <returns>The created module object instance.</returns>
        protected abstract Module CreateModule();

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
            if (disposing && module != null)
            {
                Disposable.Dispose(module);

                module = null;
            }
        }

        #endregion
    }
}
