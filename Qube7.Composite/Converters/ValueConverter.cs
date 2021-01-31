using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Provides a base class for implementations of the <see cref="IValueConverter"/> interface.
    /// </summary>
    public abstract class ValueConverter : MarkupExtension, IValueConverter
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConverter"/> class.
        /// </summary>
        protected ValueConverter()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// When overridden in a derived class, converts source value to a value for the binding target. The data binding engine calls this method when it propagates a value from the binding source to the binding target.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <c>null</c>, the valid <c>null</c> value is used. A return value of <see cref="DependencyProperty.UnsetValue"/> indicates that the converter produced no value and that the binding uses the <see cref="BindingBase.FallbackValue"/>, if available, or the default value instead. A return value of <see cref="Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="BindingBase.FallbackValue"/> or the default value.</returns>
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        /// <summary>
        /// When overridden in a derived class, converts a binding target value to the binding source value. The data binding engine calls this method when it propagates a value from the binding target to the binding source.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <c>null</c>, the valid <c>null</c> value is used. A return value of <see cref="DependencyProperty.UnsetValue"/> indicates that the converter produced no value and that the binding uses the <see cref="BindingBase.FallbackValue"/>, if available, or the default value instead. A return value of <see cref="Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="BindingBase.FallbackValue"/> or the default value.</returns>
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        /// <summary>
        /// Returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #endregion
    }
}
