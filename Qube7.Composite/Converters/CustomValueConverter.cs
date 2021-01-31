using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Provides a way to apply custom logic in a <see cref="Binding"/>.
    /// </summary>
    public class CustomValueConverter : IValueConverter
    {
        #region Events

        /// <summary>
        /// Occurs when the <see cref="IValueConverter.Convert"/> method is executed.
        /// </summary>
        public event EventHandler<ValueConvertEventArgs> Convert;

        /// <summary>
        /// Occurs when the <see cref="IValueConverter.ConvertBack"/> method is executed.
        /// </summary>
        public event EventHandler<ValueConvertBackEventArgs> ConvertBack;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomValueConverter"/> class.
        /// </summary>
        public CustomValueConverter()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts source value to a value for the binding target. The data binding engine calls this method when it propagates a value from the binding source to the binding target.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <c>null</c>, the valid <c>null</c> value is used. A return value of <see cref="DependencyProperty.UnsetValue"/> indicates that the converter produced no value and that the binding uses the <see cref="BindingBase.FallbackValue"/>, if available, or the default value instead. A return value of <see cref="Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="BindingBase.FallbackValue"/> or the default value.</returns>
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ValueConvertEventArgs args = new ValueConvertEventArgs(value, targetType, parameter, culture);

            Event.Raise(Convert, this, args);

            return args.ConvertedValue;
        }

        /// <summary>
        /// Converts a binding target value to the binding source value. The data binding engine calls this method when it propagates a value from the binding target to the binding source.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <c>null</c>, the valid <c>null</c> value is used. A return value of <see cref="DependencyProperty.UnsetValue"/> indicates that the converter produced no value and that the binding uses the <see cref="BindingBase.FallbackValue"/>, if available, or the default value instead. A return value of <see cref="Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="BindingBase.FallbackValue"/> or the default value.</returns>
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ValueConvertBackEventArgs args = new ValueConvertBackEventArgs(value, targetType, parameter, culture);

            Event.Raise(ConvertBack, this, args);

            return args.ConvertedValue;
        }

        #endregion
    }
}
