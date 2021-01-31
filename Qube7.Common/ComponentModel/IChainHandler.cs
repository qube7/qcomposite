using System;

namespace Qube7.ComponentModel
{
    /// <summary>
    /// Represents a chain handler that is single step of the chained processing.
    /// </summary>
    /// <typeparam name="T">The type of the context.</typeparam>
    public interface IChainHandler<in T>
    {
        #region Methods

        /// <summary>
        /// Processes the specified context.
        /// </summary>
        /// <param name="context">The chain context.</param>
        /// <param name="proceed">The action proceeding the chain.</param>
        void Process(T context, Action proceed);

        #endregion
    }
}
