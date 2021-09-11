using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Qube7.Composite.Hosting
{
    /// <summary>
    /// Represents a catalog of application modules that combines the elements of <see cref="ModuleIterator"/> objects.
    /// </summary>
    [ContentProperty(nameof(Modules))]
    public class ModuleCatalog : ModuleIterator
    {
        #region Fields

        /// <summary>
        /// The collection of underlying module iterator objects.
        /// </summary>
        private readonly Collection<ModuleIterator> modules;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of <see cref="ModuleIterator"/> objects that underlie the <see cref="ModuleCatalog"/> object.
        /// </summary>
        /// <value>The collection of underlying module iterator objects.</value>
        public Collection<ModuleIterator> Modules
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
            modules = new Collection<ModuleIterator>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ModuleCatalog"/>.
        /// </summary>
        /// <returns>A enumerator that can be used to iterate through the <see cref="ModuleCatalog"/>.</returns>
        public override IEnumerator<ModuleContext> GetEnumerator()
        {
            foreach (ModuleIterator iterator in modules)
            {
                if (iterator != null)
                {
                    foreach (ModuleContext context in iterator)
                    {
                        yield return context;
                    }
                }
            }
        }

        #endregion
    }
}
