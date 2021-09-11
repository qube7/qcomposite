using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace Qube7.Composite
{
    /// <summary>
    /// Provides extension methods for registering composable parts.
    /// </summary>
    public static class RegistrationExtensions
    {
        #region Methods

        /// <summary>
        /// Ensures that the composable part catalog has initialized contained part definitions.
        /// </summary>
        /// <typeparam name="T">The type of the composable part catalog being checked.</typeparam>
        /// <param name="catalog">The composable part catalog to check.</param>
        /// <returns>A reference to the same <paramref name="catalog"/>.</returns>
        public static T EnsureInitialized<T>(this T catalog) where T : ComposablePartCatalog
        {
            Requires.NotNull(catalog, nameof(catalog));

            if (catalog is TypeCatalog type)
            {
                using (type.GetEnumerator())
                {
                    return catalog;
                }
            }

            if (catalog is AssemblyCatalog assembly)
            {
                using (assembly.GetEnumerator())
                {
                    return catalog;
                }
            }

            if (catalog is AggregateCatalog aggregate)
            {
                foreach (ComposablePartCatalog item in aggregate.Catalogs)
                {
                    item.EnsureInitialized();
                }

                return catalog;
            }

            using (IEnumerator<ComposablePartDefinition> enumerator = catalog.GetEnumerator())
            {
                while (enumerator.MoveNext()) { }

                return catalog;
            }
        }

        #endregion
    }
}
