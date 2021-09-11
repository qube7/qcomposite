using System;
using System.Threading;
using System.Threading.Tasks;

namespace Qube7.Composite
{
    /// <summary>
    /// Provides extension methods for the <see cref="IRaiseEvent"/>.
    /// </summary>
    public static class RaiseEvent
    {
        #region Methods

        /// <summary>
        /// Raises the event that matches the specified contract name passing the <see cref="EventArgs.Empty"/> event data.
        /// </summary>
        /// <param name="raise">The event broker.</param>
        /// <param name="contractName">The contract name of the event to raise.</param>
        /// <param name="sender">The source of the event.</param>
        public static void Raise(this IRaiseEvent raise, string contractName, object sender)
        {
            Requires.NotNull(raise, nameof(raise));

            raise.Raise(contractName, sender, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the event that matches the specified contract name passing the <see cref="EventArgs.Empty"/> event data.
        /// </summary>
        /// <param name="raise">The event broker.</param>
        /// <param name="contractName">The contract name of the event to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static Task RaiseAsync(this IRaiseEvent raise, string contractName, object sender, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(raise, nameof(raise));

            return raise.RaiseAsync(contractName, sender, EventArgs.Empty, cancellationToken);
        }

        #endregion
    }
}
