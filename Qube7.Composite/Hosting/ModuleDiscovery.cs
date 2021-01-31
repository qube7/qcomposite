using System.Collections;
using System.Collections.Generic;

namespace Qube7.Composite.Hosting
{
    /// <summary>
    /// Provides discovery of modules for the composite application.
    /// </summary>
    public abstract class ModuleDiscovery : IEnumerable<ModuleContext>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleDiscovery"/> class.
        /// </summary>
        protected ModuleDiscovery()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ModuleDiscovery"/>.
        /// </summary>
        /// <returns>A enumerator that can be used to iterate through the <see cref="ModuleDiscovery"/>.</returns>
        public abstract IEnumerator<ModuleContext> GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ModuleDiscovery"/>.
        /// </summary>
        /// <returns>A enumerator that can be used to iterate through the <see cref="ModuleDiscovery"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
