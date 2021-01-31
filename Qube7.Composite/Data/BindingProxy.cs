using System.Windows;
using System.Windows.Markup;

namespace Qube7.Composite.Data
{
    /// <summary>
    /// Enables data binding to the wrapped object.
    /// </summary>
    [ContentProperty(nameof(Value))]
    public class BindingProxy : Freezable
    {
        #region Fields

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(object), typeof(BindingProxy), new PropertyMetadata(OnValueChanged));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the data object that this <see cref="BindingProxy"/> is forwarding.
        /// </summary>
        /// <value>The forwarded data object.</value>
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the effective value of the <see cref="Value"/> property changes.
        /// </summary>
        public event DependencyPropertyChangedEventHandler ValueChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingProxy"/> class.
        /// </summary>
        public BindingProxy()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when the effective value of the <see cref="Value"/> dependency property changes.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindingProxy proxy = d as BindingProxy;
            if (proxy != null)
            {
                proxy.OnValueChanged(e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="BindingProxy"/> class.
        /// </summary>
        /// <returns>The new instance.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        #endregion
    }
}
