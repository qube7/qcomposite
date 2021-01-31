using System;

namespace Qube7.ComponentModel
{
    /// <summary>
    /// Provides a base class for implementations of the <see cref="IChainHandler{T}"/> generic interface.
    /// </summary>
    /// <typeparam name="T">The type of the context.</typeparam>
    public abstract class ChainHandler<T> : IChainHandler<T>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChainHandler{T}"/> class.
        /// </summary>
        protected ChainHandler()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Processes the specified context.
        /// </summary>
        /// <param name="context">The chain context.</param>
        /// <param name="proceed">The action proceeding the chain.</param>
        public void Process(T context, Action proceed)
        {
            PreProceed(context);

            Proceed(context, proceed);

            PostProceed(context);
        }

        /// <summary>
        /// Processes the specified context before proceeding the chain.
        /// </summary>
        /// <param name="context">The chain context.</param>
        protected abstract void PreProceed(T context);

        /// <summary>
        /// Proceeds the chain with the specified context.
        /// </summary>
        /// <param name="context">The chain context.</param>
        /// <param name="proceed">The action proceeding the chain.</param>
        protected virtual void Proceed(T context, Action proceed)
        {
            proceed();
        }

        /// <summary>
        /// Processes the specified context after proceeding the chain.
        /// </summary>
        /// <param name="context">The chain context.</param>
        protected abstract void PostProceed(T context);

        #endregion
    }
}
