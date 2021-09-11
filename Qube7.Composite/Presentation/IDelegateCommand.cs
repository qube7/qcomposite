using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Defines a command that delegates its handling logic.
    /// </summary>
    public interface IDelegateCommand : ICommand
    {
        #region Properties

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        /// <value>The name of the command.</value>
        string Name { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="ICommand.CanExecuteChanged"/> event for the current <see cref="IDelegateCommand"/>.
        /// </summary>
        void RaiseChanged();

        /// <summary>
        /// Raises the <see cref="ICommand.CanExecuteChanged"/> event for the current <see cref="IDelegateCommand"/>.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task RaiseChangedAsync(CancellationToken cancellationToken = default);

        #endregion
    }
}
