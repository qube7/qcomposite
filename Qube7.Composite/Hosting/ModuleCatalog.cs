using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Qube7.Composite.Hosting
{
    /// <summary>
    /// Represents a catalog of application modules that combines the elements of <see cref="ModuleDiscovery"/> objects.
    /// </summary>
    [ContentProperty(nameof(Modules))]
    public class ModuleCatalog : ModuleDiscovery, IDisposable
    {
        #region Fields

        /// <summary>
        /// The collection of underlying module discovery objects.
        /// </summary>
        private readonly Collection<ModuleDiscovery> modules;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of <see cref="ModuleDiscovery"/> objects that underlie the <see cref="ModuleCatalog"/> object.
        /// </summary>
        /// <value>The collection of underlying module discovery objects.</value>
        public Collection<ModuleDiscovery> Modules
        {
            get { return modules; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleCatalog"/> class.
        /// </summary>
        public ModuleCatalog()
        {
            modules = new Collection<ModuleDiscovery>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ModuleCatalog"/>.
        /// </summary>
        /// <returns>A enumerator that can be used to iterate through the <see cref="ModuleCatalog"/>.</returns>
        public override IEnumerator<ModuleContext> GetEnumerator()
        {
            foreach (ModuleDiscovery discovery in modules)
            {
                if (discovery != null)
                {
                    foreach (ModuleContext context in discovery)
                    {
                        yield return context;
                    }
                }
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="ModuleCatalog"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources used by the <see cref="ModuleCatalog"/>.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ModuleDiscovery[] array = new ModuleDiscovery[modules.Count];

                modules.CopyTo(array, 0);

                modules.Clear();

                for (int i = 0; i < array.Length; i++)
                {
                    Disposable.Dispose(array[i]);
                }
            }
        }

        #endregion
    }
}
