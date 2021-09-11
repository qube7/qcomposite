using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Qube7.Composite.KnownBoxes;

namespace Qube7.Composite.Behaviors
{
    /// <summary>
    /// Provides the attached behaviors for the <see cref="TextBox"/>.
    /// </summary>
    public static class TextBoxBehavior
    {
        #region Fields

        /// <summary>
        /// Identifies the <see cref="P:SelectAllOnGotFocus"/> attached dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectAllOnGotFocusProperty = DependencyProperty.RegisterAttached("SelectAllOnGotFocus", typeof(bool), typeof(TextBoxBehavior), new PropertyMetadata(OnSelectAllOnGotFocusChanged));

        /// <summary>
        /// Identifies the <see cref="P:UpdateTextOnEnter"/> attached dependency property.
        /// </summary>
        public static readonly DependencyProperty UpdateTextOnEnterProperty = DependencyProperty.RegisterAttached("UpdateTextOnEnter", typeof(bool), typeof(TextBoxBehavior), new PropertyMetadata(OnUpdateTextOnEnterChanged));

        #endregion

        #region Methods

        /// <summary>
        /// Called when the effective value of the <see cref="SelectAllOnGotFocusProperty"/> changes.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectAllOnGotFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBoxBase textBox)
            {
                if (e.NewValue is bool && (bool)e.NewValue)
                {
                    textBox.GotFocus += OnTextBoxGotFocus;

                    return;
                }

                textBox.GotFocus -= OnTextBoxGotFocus;
            }
        }

        /// <summary>
        /// Called when the text box gets logical focus.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="RoutedEventArgs"/> that contains the event data.</param>
        private static void OnTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBoxBase textBox)
            {
                textBox.SelectAll();
            }
        }

        /// <summary>
        /// Called when the effective value of the <see cref="UpdateTextOnEnterProperty"/> changes.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnUpdateTextOnEnterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if (e.NewValue is bool && (bool)e.NewValue)
                {
                    textBox.KeyDown += OnTextBoxKeyDown;

                    return;
                }

                textBox.KeyDown -= OnTextBoxKeyDown;
            }
        }

        /// <summary>
        /// Called when a key is pressed while focus is on the text box.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="KeyEventArgs"/> that contains the event data.</param>
        private static void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is TextBox textBox)
            {
                BindingExpression expression = textBox.GetBindingExpression(TextBox.TextProperty);

                expression?.UpdateSource();
            }
        }

        /// <summary>
        /// Gets a value indicating whether to select all contents in the specified <see cref="TextBoxBase"/> when it receives focus.
        /// </summary>
        /// <param name="textBox">The <see cref="TextBoxBase"/> for which to get whether to select all contents when focus is got.</param>
        /// <returns><c>true</c> if to select all contents for the <paramref name="textBox"/> when focus is got; otherwise, <c>false</c>.</returns>
        public static bool GetSelectAllOnGotFocus(TextBoxBase textBox)
        {
            Requires.NotNull(textBox, nameof(textBox));

            return (bool)textBox.GetValue(SelectAllOnGotFocusProperty);
        }

        /// <summary>
        /// Sets a value indicating whether to select all contents in the specified <see cref="TextBoxBase"/> when it receives focus.
        /// </summary>
        /// <param name="textBox">The <see cref="TextBoxBase"/> for which to set whether to select all contents when focus is got.</param>
        /// <param name="value"><c>true</c> if to select all contents for the <paramref name="textBox"/> when focus is got; otherwise, <c>false</c>.</param>
        public static void SetSelectAllOnGotFocus(TextBoxBase textBox, bool value)
        {
            Requires.NotNull(textBox, nameof(textBox));

            textBox.SetValue(SelectAllOnGotFocusProperty, BooleanBox.FromValue(value));
        }

        /// <summary>
        /// Gets a value indicating whether to update binding source for the <see cref="TextBox.Text"/> property of the specified <see cref="TextBox"/> when the enter key is pressed.
        /// </summary>
        /// <param name="textBox">The <see cref="TextBox"/> for which to get whether to update source of the text binding when enter key is pressed.</param>
        /// <returns><c>true</c> if to update source of the text binding for the <paramref name="textBox"/> when enter key is pressed; otherwise, <c>false</c>.</returns>
        public static bool GetUpdateTextOnEnter(TextBox textBox)
        {
            Requires.NotNull(textBox, nameof(textBox));

            return (bool)textBox.GetValue(UpdateTextOnEnterProperty);
        }

        /// <summary>
        /// Sets a value indicating whether to update binding source for the <see cref="TextBox.Text"/> property of the specified <see cref="TextBox"/> when the enter key is pressed.
        /// </summary>
        /// <param name="textBox">The <see cref="TextBox"/> for which to set whether to update source of the text binding when enter key is pressed.</param>
        /// <param name="value"><c>true</c> if to update source of the text binding for the <paramref name="textBox"/> when enter key is pressed; otherwise, <c>false</c>.</param>
        public static void SetUpdateTextOnEnter(TextBox textBox, bool value)
        {
            Requires.NotNull(textBox, nameof(textBox));

            textBox.SetValue(UpdateTextOnEnterProperty, BooleanBox.FromValue(value));
        }

        #endregion
    }
}
