using System;
using System.Windows;
using System.Windows.Input;
using Qube7.Composite.KnownBoxes;

namespace Qube7.Composite.Behaviors
{
    /// <summary>
    /// Provides the attached behaviors for the <see cref="Window"/>.
    /// </summary>
    public static class WindowBehavior
    {
        #region Fields

        /// <summary>
        /// Identifies the <see cref="P:HandleWindowCommands"/> attached dependency property.
        /// </summary>
        public static readonly DependencyProperty HandleWindowCommandsProperty = DependencyProperty.RegisterAttached("HandleWindowCommands", typeof(bool), typeof(WindowBehavior), new PropertyMetadata(OnHandleWindowCommandsChanged));

        /// <summary>
        /// The minimize window command binding.
        /// </summary>
        private static readonly CommandBinding minimize = new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindowExecuted, OnMinimizeWindowCanExecute);

        /// <summary>
        /// The maximize window command binding.
        /// </summary>
        private static readonly CommandBinding maximize = new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindowExecuted, OnMaximizeWindowCanExecute);

        /// <summary>
        /// The restore window command binding.
        /// </summary>
        private static readonly CommandBinding restore = new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindowExecuted, OnRestoreWindowCanExecute);

        /// <summary>
        /// The close window command binding.
        /// </summary>
        private static readonly CommandBinding close = new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindowExecuted);

        #endregion

        #region Methods

        /// <summary>
        /// Called when the effective value of the <see cref="HandleWindowCommandsProperty"/> changes.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHandleWindowCommandsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window window = d as Window;
            if (window != null)
            {
                CommandBindingCollection bindings = window.CommandBindings;

                if (e.NewValue is bool && (bool)e.NewValue)
                {
                    if (bindings.IndexOf(minimize) == -1)
                    {
                        bindings.Add(minimize);
                    }

                    if (bindings.IndexOf(maximize) == -1)
                    {
                        bindings.Add(maximize);
                    }

                    if (bindings.IndexOf(restore) == -1)
                    {
                        bindings.Add(restore);
                    }

                    if (bindings.IndexOf(close) == -1)
                    {
                        bindings.Add(close);
                    }

                    window.StateChanged -= OnWindowStateChanged;
                    window.StateChanged += OnWindowStateChanged;

                    return;
                }

                bindings.Remove(minimize);

                bindings.Remove(maximize);

                bindings.Remove(restore);

                bindings.Remove(close);

                window.StateChanged -= OnWindowStateChanged;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to handle <see cref="SystemCommands.MinimizeWindowCommand"/>, <see cref="SystemCommands.MaximizeWindowCommand"/>, <see cref="SystemCommands.RestoreWindowCommand"/> and <see cref="SystemCommands.CloseWindowCommand"/> for the specified <see cref="Window"/>.
        /// </summary>
        /// <param name="window">The <see cref="Window"/> for which to get whether to handle commands.</param>
        /// <returns><c>true</c> if to handle commands for the <paramref name="window"/>; otherwise, <c>false</c>.</returns>
        public static bool GetHandleWindowCommands(Window window)
        {
            Requires.NotNull(window, nameof(window));

            return (bool)window.GetValue(HandleWindowCommandsProperty);
        }

        /// <summary>
        /// Sets a value indicating whether to handle <see cref="SystemCommands.MinimizeWindowCommand"/>, <see cref="SystemCommands.MaximizeWindowCommand"/>, <see cref="SystemCommands.RestoreWindowCommand"/> and <see cref="SystemCommands.CloseWindowCommand"/> for the specified <see cref="Window"/>.
        /// </summary>
        /// <param name="window">The <see cref="Window"/> for which to set whether to handle commands.</param>
        /// <param name="value"><c>true</c> if to handle commands for the <paramref name="window"/>; otherwise, <c>false</c>.</param>
        public static void SetHandleWindowCommands(Window window, bool value)
        {
            Requires.NotNull(window, nameof(window));

            window.SetValue(HandleWindowCommandsProperty, BooleanBox.FromValue(value));
        }

        /// <summary>
        /// Provides handling for the <see cref="CommandBinding.CanExecute"/> event associated with the <see cref="SystemCommands.MinimizeWindowCommand"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private static void OnMinimizeWindowCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Window window = sender as Window;

            e.CanExecute = window != null && window.WindowState != WindowState.Minimized && window.ResizeMode != ResizeMode.NoResize;
        }

        /// <summary>
        /// Provides handling for the <see cref="CommandBinding.Executed"/> event associated with the <see cref="SystemCommands.MinimizeWindowCommand"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void OnMinimizeWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Window window = sender as Window;
            if (window != null)
            {
                SystemCommands.MinimizeWindow(window);

                e.Handled = true;
            }
        }

        /// <summary>
        /// Provides handling for the <see cref="CommandBinding.CanExecute"/> event associated with the <see cref="SystemCommands.MaximizeWindowCommand"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private static void OnMaximizeWindowCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Window window = sender as Window;

            e.CanExecute = window != null && window.WindowState != WindowState.Maximized && window.ResizeMode != ResizeMode.NoResize && window.ResizeMode != ResizeMode.CanMinimize;
        }

        /// <summary>
        /// Provides handling for the <see cref="CommandBinding.Executed"/> event associated with the <see cref="SystemCommands.MaximizeWindowCommand"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void OnMaximizeWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Window window = sender as Window;
            if (window != null)
            {
                SystemCommands.MaximizeWindow(window);

                e.Handled = true;
            }
        }

        /// <summary>
        /// Provides handling for the <see cref="CommandBinding.CanExecute"/> event associated with the <see cref="SystemCommands.RestoreWindowCommand"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private static void OnRestoreWindowCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Window window = sender as Window;

            e.CanExecute = window != null && window.WindowState != WindowState.Normal && window.ResizeMode != ResizeMode.NoResize && window.ResizeMode != ResizeMode.CanMinimize;
        }

        /// <summary>
        /// Provides handling for the <see cref="CommandBinding.Executed"/> event associated with the <see cref="SystemCommands.RestoreWindowCommand"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void OnRestoreWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Window window = sender as Window;
            if (window != null)
            {
                SystemCommands.RestoreWindow(window);

                e.Handled = true;
            }
        }

        /// <summary>
        /// Provides handling for the <see cref="CommandBinding.Executed"/> event associated with the <see cref="SystemCommands.CloseWindowCommand"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void OnCloseWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Window window = sender as Window;
            if (window != null)
            {
                SystemCommands.CloseWindow(window);

                e.Handled = true;
            }
        }

        /// <summary>
        /// Called when the window's <see cref="Window.WindowState"/> property changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private static void OnWindowStateChanged(object sender, EventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion
    }
}
