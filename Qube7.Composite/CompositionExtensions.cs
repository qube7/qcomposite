using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace Qube7.Composite
{
    /// <summary>
    /// Provides extension methods for managing composition of parts.
    /// </summary>
    public static class CompositionExtensions
    {
        #region Methods

        /// <summary>
        /// Creates composable parts from an array of attributed objects and composes them in the specified composition container.
        /// </summary>
        /// <param name="container">The composition container to perform composition in.</param>
        /// <param name="attributedParts">An array of attributed objects to compose.</param>
        public static void ComposeParts(this ICompositionContainer container, params object[] attributedParts)
        {
            Requires.NotNull(container, nameof(container));
            Requires.NotNull(attributedParts, nameof(attributedParts));

            ComposablePart[] parts = new ComposablePart[attributedParts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = AttributedModelServices.CreatePart(attributedParts[i]);
            }

            CompositionBatch batch = new CompositionBatch(parts, Enumerable.Empty<ComposablePart>());

            container.Compose(batch);
        }

        #endregion
    }
}
