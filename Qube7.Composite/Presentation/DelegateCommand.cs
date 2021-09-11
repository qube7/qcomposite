using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Qube7.Composite.Threading;

namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Represents a command that delegates its handling logic.
    /// </summary>
    /// <typeparam name="T">The type of the data parameter of the command.</typeparam>
    [DebuggerDisplay("Name = {Name}, Execute = {execute}, CanExecute = {canExecute}")]
    public class DelegateCommand<T> : Command<T>, IDelegateCommand
    {
        #region Fields

        /// <summary>
        /// The execute delegate.
        /// </summary>
        private readonly Action<T> execute;

        /// <summary>
        /// The can-execute delegate.
        /// </summary>
        private readonly Func<T, bool> canExecute;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand{T}"/> class.
        /// </summary>
        /// <param name="execute">The delegate to be executed when the command is invoked.</param>
        public DelegateCommand(Action<T> execute)
        {
            Requires.NotNull(execute, nameof(execute));

            this.execute = execute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand{T}"/> class.
        /// </summary>
        /// <param name="execute">The delegate to be executed when the command is invoked.</param>
        /// <param name="canExecute">The delegate to be executed to determine whether the command can execute in its current state.</param>
        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute) : this(execute)
        {
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand{T}"/> class.
        /// </summary>
        /// <param name="execute">The delegate to be executed when the command is invoked.</param>
        /// <param name="name">The name of the command.</param>
        public DelegateCommand(Action<T> execute, string name) : base(name)
        {
            Requires.NotNull(execute, nameof(execute));

            this.execute = execute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand{T}"/> class.
        /// </summary>
        /// <param name="execute">The delegate to be executed when the command is invoked.</param>
        /// <param name="canExecute">The delegate to be executed to determine whether the command can execute in its current state.</param>
        /// <param name="name">The name of the command.</param>
        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute, string name) : this(execute, name)
        {
            this.canExecute = canExecute;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when the command is invoked.
        /// </summary>
        /// <param name="parameter">The data used by the command.</param>
        public override void Execute(T parameter)
        {
            execute(parameter);
        }

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">The data used by the command.</param>
        /// <returns><c>true</c> if this command can be executed; otherwise, <c>false</c>.</returns>
        public override bool CanExecute(T parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        /// <summary>
        /// Raises the <see cref="ICommand.CanExecuteChanged"/> event for the current <see cref="DelegateCommand{T}"/>.
        /// </summary>
        public void RaiseChanged()
        {
            if (UIThread.CheckAccess())
            {
                OnCanExecuteChanged();

                return;
            }

            UIThread.Invoke(OnCanExecuteChanged, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Raises the <see cref="ICommand.CanExecuteChanged"/> event for the current <see cref="DelegateCommand{T}"/>.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async Task RaiseChangedAsync(CancellationToken cancellationToken = default)
        {
            if (UIThread.CheckAccess())
            {
                OnCanExecuteChanged();

                return;
            }

            await UIThread.InvokeAsync(OnCanExecuteChanged, DispatcherPriority.Normal, cancellationToken);
        }

        #endregion
    }
}
