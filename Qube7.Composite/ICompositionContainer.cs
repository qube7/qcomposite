using System.ComponentModel.Composition.Hosting;

namespace Qube7.Composite
{
    /// <summary>
    /// Represents an object that manages the composition of parts.
    /// </summary>
    public interface ICompositionContainer
    {
        #region Methods

        /// <summary>
        /// Adds or removes the parts in the specified <see cref="CompositionBatch"/> from the container and executes composition.
        /// </summary>
        /// <param name="batch">Changes to the <see cref="ICompositionContainer"/> to include during the composition.</param>
        void Compose(CompositionBatch batch);

        #endregion
    }
}
