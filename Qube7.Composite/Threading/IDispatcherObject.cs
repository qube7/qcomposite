using System.Windows.Threading;

namespace Qube7.Composite.Threading
{
    /// <summary>
    /// Represents an object that is associated with a <see cref="System.Windows.Threading.Dispatcher"/>.
    /// </summary>
    public interface IDispatcherObject
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="System.Windows.Threading.Dispatcher"/> this <see cref="IDispatcherObject"/> is associated with.
        /// </summary>
        /// <value>The associated <see cref="System.Windows.Threading.Dispatcher"/>.</value>
        Dispatcher Dispatcher { get; }

        #endregion
    }
}
