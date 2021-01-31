using System;

namespace Qube7.Composite
{
    /// <summary>
    /// Provides helpers for the <see cref="IRaiseEvent"/>.
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

        #endregion
    }
}
