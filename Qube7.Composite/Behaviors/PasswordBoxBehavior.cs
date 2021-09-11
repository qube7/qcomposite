using System.Security;
using System.Windows;
using System.Windows.Controls;
using Qube7.Composite.KnownBoxes;
using Qube7.Composite.Threading;

namespace Qube7.Composite.Behaviors
{
    /// <summary>
    /// Provides the attached behaviors for the <see cref="PasswordBox"/>.
    /// </summary>
    public static class PasswordBoxBehavior
    {
        #region Fields

        /// <summary>
        /// The unset secure string password value.
        /// </summary>
        private static readonly SecureString UnsetSecure = new SecureString();

        /// <summary>
        /// Identifies the <see cref="P:SecurePassword"/> attached dependency property.
        /// </summary>
        public static readonly DependencyProperty SecurePasswordProperty = DependencyProperty.RegisterAttached("SecurePassword", typeof(SecureString), typeof(PasswordBoxBehavior), new FrameworkPropertyMetadata(UnsetSecure, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSecurePasswordChanged));

        /// <summary>
        /// Identifies the <see cref="P:SelectAllOnGotFocus"/> attached dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectAllOnGotFocusProperty = DependencyProperty.RegisterAttached("SelectAllOnGotFocus", typeof(bool), typeof(PasswordBoxBehavior), new PropertyMetadata(OnSelectAllOnGotFocusChanged));

        #endregion

        #region Methods

        /// <summary>
        /// Called when the effective value of the <see cref="SecurePasswordProperty"/> changes.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSecurePasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                if (e.OldValue == UnsetSecure)
                {
                    passwordBox.PasswordChanged += OnPasswordBoxPasswordChanged;

                    passwordBox.InvokeAsync(() => passwordBox.SetCurrentValue(SecurePasswordProperty, passwordBox.SecurePassword));

                    return;
                }

                if (e.NewValue == UnsetSecure)
                {
                    passwordBox.PasswordChanged -= OnPasswordBoxPasswordChanged;

                    return;
                }
            }
        }

        /// <summary>
        /// Called when the value of the <see cref="PasswordBox.Password"/> property changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="RoutedEventArgs"/> that contains the event data.</param>
        private static void OnPasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                passwordBox.SetCurrentValue(SecurePasswordProperty, passwordBox.SecurePassword);
            }
        }

        /// <summary>
        /// Called when the effective value of the <see cref="SelectAllOnGotFocusProperty"/> changes.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectAllOnGotFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                if (e.NewValue is bool && (bool)e.NewValue)
                {
                    passwordBox.GotFocus += OnPasswordBoxGotFocus;

                    return;
                }

                passwordBox.GotFocus -= OnPasswordBoxGotFocus;
            }
        }

        /// <summary>
        /// Called when the password box gets logical focus.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="RoutedEventArgs"/> that contains the event data.</param>
        private static void OnPasswordBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                passwordBox.SelectAll();
            }
        }

        /// <summary>
        /// Gets the attached secure string password for the specified <see cref="PasswordBox"/>.
        /// </summary>
        /// <param name="passwordBox">The <see cref="PasswordBox"/> for which to get the attached secure string password.</param>
        /// <returns>A secure string representing the attached password for the <paramref name="passwordBox"/>.</returns>
        public static SecureString GetSecurePassword(PasswordBox passwordBox)
        {
            Requires.NotNull(passwordBox, nameof(passwordBox));

            return (SecureString)passwordBox.GetValue(SecurePasswordProperty);
        }

        /// <summary>
        /// Sets the attached secure string password for the specified <see cref="PasswordBox"/>.
        /// </summary>
        /// <param name="passwordBox">The <see cref="PasswordBox"/> for which to set the attached secure string password.</param>
        /// <param name="value">A secure string representing the attached password for the <paramref name="passwordBox"/>.</param>
        public static void SetSecurePassword(PasswordBox passwordBox, SecureString value)
        {
            Requires.NotNull(passwordBox, nameof(passwordBox));

            passwordBox.SetValue(SecurePasswordProperty, value);
        }

        /// <summary>
        /// Gets a value indicating whether to select all contents in the specified <see cref="PasswordBox"/> when it receives focus.
        /// </summary>
        /// <param name="passwordBox">The <see cref="PasswordBox"/> for which to get whether to select all contents when focus is got.</param>
        /// <returns><c>true</c> if to select all contents for the <paramref name="passwordBox"/> when focus is got; otherwise, <c>false</c>.</returns>
        public static bool GetSelectAllOnGotFocus(PasswordBox passwordBox)
        {
            Requires.NotNull(passwordBox, nameof(passwordBox));

            return (bool)passwordBox.GetValue(SelectAllOnGotFocusProperty);
        }

        /// <summary>
        /// Sets a value indicating whether to select all contents in the specified <see cref="PasswordBox"/> when it receives focus.
        /// </summary>
        /// <param name="passwordBox">The <see cref="PasswordBox"/> for which to set whether to select all contents when focus is got.</param>
        /// <param name="value"><c>true</c> if to select all contents for the <paramref name="passwordBox"/> when focus is got; otherwise, <c>false</c>.</param>
        public static void SetSelectAllOnGotFocus(PasswordBox passwordBox, bool value)
        {
            Requires.NotNull(passwordBox, nameof(passwordBox));

            passwordBox.SetValue(SelectAllOnGotFocusProperty, BooleanBox.FromValue(value));
        }

        #endregion
    }
}
