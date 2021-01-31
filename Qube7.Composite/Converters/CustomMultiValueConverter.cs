using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Provides a way to apply custom logic in a <see cref="MultiBinding"/>.
    /// </summary>
    public class CustomMultiValueConverter : IMultiValueConverter
    {
        #region Events

        /// <summary>
        /// Occurs when the <see cref="IMultiValueConverter.Convert"/> method is executed.
        /// </summary>
        public event EventHandler<MultiValueConvertEventArgs> Convert;

        /// <summary>
        /// Occurs when the <see cref="IMultiValueConverter.ConvertBack"/> method is executed.
        /// </summary>
        public event EventHandler<MultiValueConvertBackEventArgs> ConvertBack;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomMultiValueConverter"/> class.
        /// </summary>
        public CustomMultiValueConverter()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <param name="values">The array of values produced by the source bindings in the <see cref="MultiBinding"/>. The value <see cref="DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <c>null</c>, the valid <c>null</c> value is used. A return value of <see cref="DependencyProperty.UnsetValue"/> indicates that the converter produced no value and that the binding uses the <see cref="BindingBase.FallbackValue"/>, if available, or the default value instead. A return value of <see cref="Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="BindingBase.FallbackValue"/> or the default value.</returns>
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            MultiValueConvertEventArgs args = new MultiValueConvertEventArgs(values, targetType, parameter, culture);

            Event.Raise(Convert, this, args);

            return args.ConvertedValue;
        }

        /// <summary>
        /// Converts a binding target value to the source binding values. The data binding engine calls this method when it propagates a value from the binding target to the source bindings.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>An array of values that have been converted from the target value back to the source values. If the method returns <c>null</c> at position i, the valid <c>null</c> value is used for the source binding at index i. Return <see cref="DependencyProperty.UnsetValue"/> at position i to indicate that the converter is unable to provide a value for the source binding at index i, and that no value is to be set on it. Return <see cref="Binding.DoNothing"/> at position i to indicate that no value is to be set on the source binding at index i. Return <c>null</c> to indicate that the converter cannot perform the conversion or that it does not support conversion in this direction.</returns>
        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            MultiValueConvertBackEventArgs args = new MultiValueConvertBackEventArgs(value, targetTypes, parameter, culture);

            Event.Raise(ConvertBack, this, args);

            return args.ConvertedValues;
        }

        #endregion
    }
}
