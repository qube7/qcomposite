using System;
using System.Globalization;
using System.Threading;
using System.Windows;

namespace Qube7.Composite
{
    /// <summary>
    /// Provides the user interface culture associated with the current application.
    /// </summary>
    public static class Culture
    {
        #region Fields

        /// <summary>
        /// The current user interface culture.
        /// </summary>
        private static CultureInfo current = CultureInfo.InstalledUICulture;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="CultureInfo"/> object that represents the current user interface culture.
        /// </summary>
        /// <value>The current user interface culture.</value>
        public static CultureInfo Current
        {
            get { return current; }
            set
            {
                Requires.NotNull(value, nameof(value));

                Interlocked.Exchange(ref current, value);

                CultureInfo.DefaultThreadCurrentUICulture = value;

                CultureInfo.CurrentUICulture = value;

                Application application = Application.Current;

                if (application != null && !application.CheckAccess())
                {
                    application.Dispatcher.Thread.CurrentUICulture = value;
                }

                OnCurrentChanged();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the current culture changes.
        /// </summary>
        internal static event EventHandler CurrentChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="CurrentChanged"/> event.
        /// </summary>
        private static void OnCurrentChanged()
        {
            Event.Raise(CurrentChanged, null);
        }

        #endregion
    }
}
