using System;
using System.Windows.Input;

namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Provides a base class for the command implementations.
    /// </summary>
    /// <typeparam name="T">The type of the data parameter of the command.</typeparam>
    public abstract class Command<T> : ICommand
    {
        #region Fields

        /// <summary>
        /// A value indicating whether the type of the data parameter is <see cref="Object"/> type.
        /// </summary>
        private static readonly bool IsObject = typeof(T) == typeof(object);

        /// <summary>
        /// A value indicating whether the type of the data parameter accepts <c>null</c> value.
        /// </summary>
        private static readonly bool IsNullable = default(T) == null;

        /// <summary>
        /// The name of the command.
        /// </summary>
        private readonly string name;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        /// <value>The name of the command.</value>
        public string Name
        {
            get { return name; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{T}"/> class.
        /// </summary>
        protected Command()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{T}"/> class.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        protected Command(string name)
        {
            this.name = name;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">The data used by the command.</param>
        /// <returns><c>true</c> if this command can be executed; otherwise, <c>false</c>.</returns>
        bool ICommand.CanExecute(object parameter)
        {
            if (IsObject || parameter is T)
            {
                return CanExecute((T)parameter);
            }

            if (parameter == null && IsNullable)
            {
                return CanExecute(default);
            }

            return false;
        }

        /// <summary>
        /// Called when the command is invoked.
        /// </summary>
        /// <param name="parameter">The data used by the command.</param>
        void ICommand.Execute(object parameter)
        {
            if (IsObject || parameter is T)
            {
                Execute((T)parameter);

                return;
            }

            if (parameter == null && IsNullable)
            {
                Execute(default);

                return;
            }

            throw Error.InvalidOperation(string.Format(Strings.ParameterTypeInvalid, typeof(T)));
        }

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">The data used by the command.</param>
        /// <returns><c>true</c> if this command can be executed; otherwise, <c>false</c>.</returns>
        public virtual bool CanExecute(T parameter)
        {
            return true;
        }

        /// <summary>
        /// Called when the command is invoked.
        /// </summary>
        /// <param name="parameter">The data used by the command.</param>
        public abstract void Execute(T parameter);

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            Event.Raise(CanExecuteChanged, this);
        }

        #endregion
    }
}
