using System.Collections;
using System.Collections.Generic;

namespace Qube7.Composite.Hosting
{
    /// <summary>
    /// Provides a sequence of modules for the composite application.
    /// </summary>
    public abstract class ModuleIterator : IEnumerable<ModuleContext>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleIterator"/> class.
        /// </summary>
        protected ModuleIterator()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ModuleIterator"/>.
        /// </summary>
        /// <returns>A enumerator that can be used to iterate through the <see cref="ModuleIterator"/>.</returns>
        public abstract IEnumerator<ModuleContext> GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ModuleIterator"/>.
        /// </summary>
        /// <returns>A enumerator that can be used to iterate through the <see cref="ModuleIterator"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
