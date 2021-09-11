using System.Threading;
using System.Threading.Tasks;

namespace Qube7.Composite
{
    /// <summary>
    /// Represents an object that publishes brokered events.
    /// </summary>
    public interface IRaiseEvent
    {
        #region Methods

        /// <summary>
        /// Raises the event that matches the specified contract name.
        /// </summary>
        /// <typeparam name="T">The type of the event data.</typeparam>
        /// <param name="contractName">The contract name of the event to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <typeparamref name="T"/> that contains the event data.</param>
        void Raise<T>(string contractName, object sender, T e);

        /// <summary>
        /// Raises the event that matches the specified contract name.
        /// </summary>
        /// <typeparam name="T">The type of the event data.</typeparam>
        /// <param name="contractName">The contract name of the event to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <typeparamref name="T"/> that contains the event data.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task RaiseAsync<T>(string contractName, object sender, T e, CancellationToken cancellationToken = default);

        #endregion
    }
}
