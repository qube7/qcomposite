using System.ComponentModel;

namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Provides a base class for the strongly typed command implementations.
    /// </summary>
    /// <typeparam name="T">The type of the data parameter of the command.</typeparam>
    public abstract class Command<T> : Command
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{T}"/> class.
        /// </summary>
        protected Command()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">The data used by the command.</param>
        /// <returns><c>true</c> if this command can be executed; otherwise, <c>false</c>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected sealed override bool CanExecute(object parameter)
        {
            if (parameter == null)
            {
                if (default(T) == null)
                {
                    return CanExecute(default);
                }

                return false;
            }

            if (parameter is T)
            {
                return CanExecute((T)parameter);
            }

            return false;
        }

        /// <summary>
        /// Called when the command is invoked.
        /// </summary>
        /// <param name="parameter">The data used by the command.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected sealed override void Execute(object parameter)
        {
            if (parameter == null)
            {
                if (default(T) == null)
                {
                    Execute(default);

                    return;
                }

                throw Error.InvalidOperation(Format.Current(Strings.ParameterTypeInvalid, typeof(T)));
            }

            if (parameter is T)
            {
                Execute((T)parameter);

                return;
            }

            throw Error.InvalidOperation(Format.Current(Strings.ParameterTypeInvalid, typeof(T)));
        }

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">The data used by the command.</param>
        /// <returns><c>true</c> if this command can be executed; otherwise, <c>false</c>.</returns>
        protected internal virtual bool CanExecute(T parameter)
        {
            return true;
        }

        /// <summary>
        /// Called when the command is invoked.
        /// </summary>
        /// <param name="parameter">The data used by the command.</param>
        protected internal abstract void Execute(T parameter);

        #endregion
    }
}
