using System;
using System.Windows.Input;

namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Provides a base class for the command implementations.
    /// </summary>
    public abstract class Command : ICommand
    {
        #region Fields

        /// <summary>
        /// The can-execute changed event handler.
        /// </summary>
        private EventHandler changed;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        event EventHandler ICommand.CanExecuteChanged
        {
            add { Event.Subscribe(ref changed, value); }
            remove { Event.Unsubscribe(ref changed, value); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        protected Command()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a command that delegates its handling logic.
        /// </summary>
        /// <param name="execute">The delegate to be executed when the command is invoked.</param>
        /// <returns>The command with delegated handling logic.</returns>
        public static Command Create(Action<object> execute)
        {
            Requires.NotNull(execute, nameof(execute));

            return new Delegate1Command(execute);
        }

        /// <summary>
        /// Creates a command that delegates its handling logic.
        /// </summary>
        /// <param name="execute">The delegate to be executed when the command is invoked.</param>
        /// <param name="canExecute">The delegate to be executed to determine whether the command can execute in its current state.</param>
        /// <returns>The command with delegated handling logic.</returns>
        public static Command Create(Action<object> execute, Func<object, bool> canExecute)
        {
            Requires.NotNull(execute, nameof(execute));

            if (canExecute == null)
            {
                return new Delegate1Command(execute);
            }

            return new Delegate2Command(execute, canExecute);
        }

        /// <summary>
        /// Creates a command that delegates its handling logic.
        /// </summary>
        /// <typeparam name="T">The type of the data parameter of the command.</typeparam>
        /// <param name="execute">The delegate to be executed when the command is invoked.</param>
        /// <returns>The command with delegated handling logic.</returns>
        public static Command<T> Create<T>(Action<T> execute)
        {
            Requires.NotNull(execute, nameof(execute));

            return new Delegate1Command<T>(execute);
        }

        /// <summary>
        /// Creates a command that delegates its handling logic.
        /// </summary>
        /// <typeparam name="T">The type of the data parameter of the command.</typeparam>
        /// <param name="execute">The delegate to be executed when the command is invoked.</param>
        /// <param name="canExecute">The delegate to be executed to determine whether the command can execute in its current state.</param>
        /// <returns>The command with delegated handling logic.</returns>
        public static Command<T> Create<T>(Action<T> execute, Func<T, bool> canExecute)
        {
            Requires.NotNull(execute, nameof(execute));

            if (canExecute == null)
            {
                return new Delegate1Command<T>(execute);
            }

            return new Delegate2Command<T>(execute, canExecute);
        }

        /// <summary>
        /// Determines whether the specified command can execute in its current state.
        /// </summary>
        /// <param name="command">The command to test.</param>
        /// <param name="parameter">The data used by the command.</param>
        /// <returns><c>true</c> if the <paramref name="command"/> can be executed; otherwise, <c>false</c>.</returns>
        public static bool CanExecute(ICommand command, object parameter)
        {
            Requires.NotNull(command, nameof(command));

            return command.CanExecute(parameter);
        }

        /// <summary>
        /// Invokes the specified command.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="parameter">The data used by the command.</param>
        public static void Execute(ICommand command, object parameter)
        {
            Requires.NotNull(command, nameof(command));

            command.Execute(parameter);
        }

        /// <summary>
        /// Determines whether the specified command can execute in its current state.
        /// </summary>
        /// <typeparam name="T">The type of the data parameter of the command.</typeparam>
        /// <param name="command">The command to test.</param>
        /// <param name="parameter">The data used by the command.</param>
        /// <returns><c>true</c> if the <paramref name="command"/> can be executed; otherwise, <c>false</c>.</returns>
        public static bool CanExecute<T>(Command<T> command, T parameter)
        {
            Requires.NotNull(command, nameof(command));

            return command.CanExecute(parameter);
        }

        /// <summary>
        /// Invokes the specified command.
        /// </summary>
        /// <typeparam name="T">The type of the data parameter of the command.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="parameter">The data used by the command.</param>
        public static void Execute<T>(Command<T> command, T parameter)
        {
            Requires.NotNull(command, nameof(command));

            command.Execute(parameter);
        }

        /// <summary>
        /// Raises the <see cref="ICommand.CanExecuteChanged"/> event for the specified <see cref="Command"/>.
        /// </summary>
        /// <param name="command">The <see cref="Command"/> for which to raise the <see cref="ICommand.CanExecuteChanged"/> event.</param>
        public static void RaiseChanged(Command command)
        {
            Requires.NotNull(command, nameof(command));

            command.OnCanExecuteChanged();
        }

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">The data used by the command.</param>
        /// <returns><c>true</c> if this command can be executed; otherwise, <c>false</c>.</returns>
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(parameter);
        }

        /// <summary>
        /// Called when the command is invoked.
        /// </summary>
        /// <param name="parameter">The data used by the command.</param>
        void ICommand.Execute(object parameter)
        {
            Execute(parameter);
        }

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">The data used by the command.</param>
        /// <returns><c>true</c> if this command can be executed; otherwise, <c>false</c>.</returns>
        protected virtual bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Called when the command is invoked.
        /// </summary>
        /// <param name="parameter">The data used by the command.</param>
        protected abstract void Execute(object parameter);

        /// <summary>
        /// Raises the <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            Event.Raise(changed, this);
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents a command that delegates its handling logic.
        /// </summary>
        private class Delegate1Command : Command
        {
            #region Fields

            /// <summary>
            /// The execute delegate.
            /// </summary>
            private readonly Action<object> execute;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Delegate1Command"/> class.
            /// </summary>
            /// <param name="execute">The execute delegate.</param>
            internal Delegate1Command(Action<object> execute)
            {
                this.execute = execute;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called when the command is invoked.
            /// </summary>
            /// <param name="parameter">The data used by the command.</param>
            protected override void Execute(object parameter)
            {
                execute(parameter);
            }

            #endregion
        }

        /// <summary>
        /// Represents a command that delegates its handling logic.
        /// </summary>
        private class Delegate2Command : Delegate1Command
        {
            #region Fields

            /// <summary>
            /// The can-execute delegate.
            /// </summary>
            private readonly Func<object, bool> canExecute;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Delegate2Command"/> class.
            /// </summary>
            /// <param name="execute">The execute delegate.</param>
            /// <param name="canExecute">The can-execute delegate.</param>
            internal Delegate2Command(Action<object> execute, Func<object, bool> canExecute) : base(execute)
            {
                this.canExecute = canExecute;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Determines whether the command can execute in its current state.
            /// </summary>
            /// <param name="parameter">The data used by the command.</param>
            /// <returns><c>true</c> if this command can be executed; otherwise, <c>false</c>.</returns>
            protected override bool CanExecute(object parameter)
            {
                return canExecute(parameter);
            }

            #endregion
        }

        /// <summary>
        /// Represents a command that delegates its handling logic.
        /// </summary>
        /// <typeparam name="T">The type of the data parameter of the command.</typeparam>
        private class Delegate1Command<T> : Command<T>
        {
            #region Fields

            /// <summary>
            /// The execute delegate.
            /// </summary>
            private readonly Action<T> execute;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Delegate1Command{T}"/> class.
            /// </summary>
            /// <param name="execute">The execute delegate.</param>
            internal Delegate1Command(Action<T> execute)
            {
                this.execute = execute;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called when the command is invoked.
            /// </summary>
            /// <param name="parameter">The data used by the command.</param>
            protected internal override void Execute(T parameter)
            {
                execute(parameter);
            }

            #endregion
        }

        /// <summary>
        /// Represents a command that delegates its handling logic.
        /// </summary>
        /// <typeparam name="T">The type of the data parameter of the command.</typeparam>
        private class Delegate2Command<T> : Delegate1Command<T>
        {
            #region Fields

            /// <summary>
            /// The can-execute delegate.
            /// </summary>
            private readonly Func<T, bool> canExecute;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Delegate2Command{T}"/> class.
            /// </summary>
            /// <param name="execute">The execute delegate.</param>
            /// <param name="canExecute">The can-execute delegate.</param>
            internal Delegate2Command(Action<T> execute, Func<T, bool> canExecute) : base(execute)
            {
                this.canExecute = canExecute;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Determines whether the command can execute in its current state.
            /// </summary>
            /// <param name="parameter">The data used by the command.</param>
            /// <returns><c>true</c> if this command can be executed; otherwise, <c>false</c>.</returns>
            protected internal override bool CanExecute(T parameter)
            {
                return canExecute(parameter);
            }

            #endregion
        }

        #endregion
    }
}
