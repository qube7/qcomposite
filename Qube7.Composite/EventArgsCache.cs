using System.ComponentModel;
using System.Windows.Data;

namespace Qube7.Composite
{
    /// <summary>
    /// Provides the cached event data objects.
    /// </summary>
    internal static class EventArgsCache
    {
        #region Fields

        /// <summary>
        /// Represents the cached instance of <see cref="PropertyChangedEventArgs"/> indicating that all of the properties have changed.
        /// </summary>
        internal static readonly PropertyChangedEventArgs ObjectPropertyChanged = new PropertyChangedEventArgs(string.Empty);

        /// <summary>
        /// Represents the cached instance of <see cref="PropertyChangedEventArgs"/> indicating that an indexer property has changed.
        /// </summary>
        internal static readonly PropertyChangedEventArgs IndexerPropertyChanged = new PropertyChangedEventArgs(Binding.IndexerName);

        #endregion
    }
}
