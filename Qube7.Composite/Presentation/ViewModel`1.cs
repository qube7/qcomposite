using System.Threading;

namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Provides a base class for the view models that expose commands.
    /// </summary>
    /// <typeparam name="TCommands">The type of object that aggregates the commands.</typeparam>
    public abstract class ViewModel<TCommands> : ViewModel where TCommands : class
    {
        #region Fields

        /// <summary>
        /// The command aggregator.
        /// </summary>
        private TCommands commands;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the object that aggregates the commands associated with this <see cref="ViewModel{TCommands}"/>.
        /// </summary>
        /// <value>The object that aggregates the commands of this <see cref="ViewModel{TCommands}"/>.</value>
        public TCommands Commands
        {
            get
            {
                if (commands == null)
                {
                    TCommands value = CreateCommands();

                    if (value == null)
                    {
                        throw Error.ReturnsNull(nameof(CreateCommands));
                    }

                    Interlocked.CompareExchange(ref commands, value, null);
                }

                return commands;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel{TCommands}"/> class.
        /// </summary>
        protected ViewModel()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// When overridden in a derived class, creates the object that aggregates the commands associated with this <see cref="ViewModel{TCommands}"/>.
        /// </summary>
        /// <returns>The object that aggregates the commands of this <see cref="ViewModel{TCommands}"/>.</returns>
        protected abstract TCommands CreateCommands();

        #endregion
    }
}
