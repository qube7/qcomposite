using System.Collections.Generic;
using System.Linq;

namespace Qube7.ComponentModel
{
    /// <summary>
    /// Provides methods for executing chained processing.
    /// </summary>
    public static class Chain
    {
        #region Methods

        /// <summary>
        /// Processes the specified context.
        /// </summary>
        /// <typeparam name="T">The type of the context.</typeparam>
        /// <param name="context">The chain context.</param>
        /// <param name="handlers">The chain handlers.</param>
        public static void Process<T>(T context, IEnumerable<IChainHandler<T>> handlers)
        {
            Requires.NotNull(handlers, nameof(handlers));

            using (IEnumerator<IChainHandler<T>> enumerator = handlers.GetEnumerator())
            {
                Pipeline<T> pipeline = new Pipeline<T>(enumerator, context);

                pipeline.Proceed();
            }
        }

        /// <summary>
        /// Processes the specified context.
        /// </summary>
        /// <typeparam name="T">The type of the context.</typeparam>
        /// <param name="context">The chain context.</param>
        /// <param name="handlers">The chain handlers.</param>
        public static void Process<T>(T context, params IChainHandler<T>[] handlers)
        {
            Process(context, handlers.AsEnumerable());
        }

        #endregion

        #region Nested types

        /// <summary>
        /// The chain processing pipeline.
        /// </summary>
        /// <typeparam name="T">The type of the context.</typeparam>
        private class Pipeline<T>
        {
            #region Fields

            /// <summary>
            /// The handlers enumerator.
            /// </summary>
            private readonly IEnumerator<IChainHandler<T>> enumerator;

            /// <summary>
            /// The chain context.
            /// </summary>
            private readonly T context;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Pipeline{T}"/> class.
            /// </summary>
            /// <param name="enumerator">The handlers enumerator.</param>
            /// <param name="context">The chain context.</param>
            internal Pipeline(IEnumerator<IChainHandler<T>> enumerator, T context)
            {
                this.enumerator = enumerator;
                this.context = context;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Proceeds this instance.
            /// </summary>
            internal void Proceed()
            {
                if (enumerator.MoveNext())
                {
                    IChainHandler<T> handler = enumerator.Current;
                    if (handler != null)
                    {
                        handler.Process(context, Proceed);
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}
