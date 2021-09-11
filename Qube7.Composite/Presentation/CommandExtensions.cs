using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Qube7.Composite.Threading;

namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Provides extension methods for the <see cref="ICommand"/>.
    /// </summary>
    public static class CommandExtensions
    {
        #region Methods

        /// <summary>
        /// Raises the <see cref="ICommand.CanExecuteChanged"/> event for each <see cref="IDelegateCommand"/> in the specified sequence.
        /// </summary>
        /// <param name="commands">The sequence on whose <see cref="IDelegateCommand"/> elements to raise the event.</param>
        public static void RaiseChanged(this IEnumerable<ICommand> commands)
        {
            Requires.NotNull(commands, nameof(commands));

            List<IDelegateCommand> list = new List<IDelegateCommand>();

            list.AddRange(commands.OfType<IDelegateCommand>());

            if (list.Count > 0)
            {
                if (UIThread.CheckAccess())
                {
                    RaiseChanged(list);

                    return;
                }

                UIThread.Invoke(() => RaiseChanged(list), DispatcherPriority.Normal);
            }
        }

        /// <summary>
        /// Raises the <see cref="ICommand.CanExecuteChanged"/> event for each <see cref="IDelegateCommand"/> in the specified sequence.
        /// </summary>
        /// <param name="commands">The sequence on whose <see cref="IDelegateCommand"/> elements to raise the event.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static async Task RaiseChangedAsync(this IEnumerable<ICommand> commands, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(commands, nameof(commands));

            List<IDelegateCommand> list = new List<IDelegateCommand>();

            list.AddRange(commands.OfType<IDelegateCommand>());

            if (list.Count > 0)
            {
                await UIThread.InvokeAsync(() => RaiseChanged(list, cancellationToken), DispatcherPriority.Normal, cancellationToken);
            }
        }

        /// <summary>
        /// Raises the <see cref="ICommand.CanExecuteChanged"/> event for each <see cref="IDelegateCommand"/> in the specified list.
        /// </summary>
        /// <param name="commands">The list of <see cref="IDelegateCommand"/> objects on whose to raise the event.</param>
        private static void RaiseChanged(List<IDelegateCommand> commands)
        {
            for (int i = 0; i < commands.Count; i++)
            {
                commands[i].RaiseChanged();
            }
        }

        /// <summary>
        /// Raises the <see cref="ICommand.CanExecuteChanged"/> event for each <see cref="IDelegateCommand"/> in the specified list.
        /// </summary>
        /// <param name="commands">The list of <see cref="IDelegateCommand"/> objects on whose to raise the event.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        private static void RaiseChanged(List<IDelegateCommand> commands, CancellationToken cancellationToken)
        {
            for (int i = 0; i < commands.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                commands[i].RaiseChanged();
            }
        }

        #endregion
    }
}
