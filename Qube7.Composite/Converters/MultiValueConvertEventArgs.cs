using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Provides data for the <see cref="CustomMultiValueConverter.Convert"/> event.
    /// </summary>
    public class MultiValueConvertEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// The array of values produced by the source bindings in the <see cref="MultiBinding"/>.
        /// </summary>
        private readonly object[] values;

        /// <summary>
        /// The type of the binding target property.
        /// </summary>
        private readonly Type targetType;

        /// <summary>
        /// The converter parameter to use.
        /// </summary>
        private readonly object parameter;

        /// <summary>
        /// The culture to use in the converter.
        /// </summary>
        private readonly CultureInfo culture;

        /// <summary>
        /// The converted value.
        /// </summary>
        private object convertedValue;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the array of values produced by the source bindings in the <see cref="MultiBinding"/>.
        /// </summary>
        /// <value>The array of values produced by the source bindings in the <see cref="MultiBinding"/>.</value>
        /// <remarks>The value <see cref="DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</remarks>
        public object[] Values
        {
            get { return values; }
        }

        /// <summary>
        /// Gets the type of the binding target property.
        /// </summary>
        /// <value>The type of the binding target property.</value>
        public Type TargetType
        {
            get { return targetType; }
        }

        /// <summary>
        /// Gets the converter parameter to use.
        /// </summary>
        /// <value>The converter parameter to use.</value>
        public object Parameter
        {
            get { return parameter; }
        }

        /// <summary>
        /// Gets the culture to use in the converter.
        /// </summary>
        /// <value>The culture to use in the converter.</value>
        public CultureInfo Culture
        {
            get { return culture; }
        }

        /// <summary>
        /// Gets or sets the conversion result value. The default value is <see cref="Binding.DoNothing"/>.
        /// </summary>
        /// <value>The converted value.</value>
        /// <remarks>The <c>null</c> value indicates that the binding uses the valid <c>null</c> value. A value of <see cref="DependencyProperty.UnsetValue"/> indicates that the converter produced no value and that the binding uses the <see cref="BindingBase.FallbackValue"/>, if available, or the default value instead. A value of <see cref="Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="BindingBase.FallbackValue"/> or the default value.</remarks>
        public object ConvertedValue
        {
            get { return convertedValue; }
            set { convertedValue = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiValueConvertEventArgs"/> class.
        /// </summary>
        /// <param name="values">The array of values produced by the source bindings in the <see cref="MultiBinding"/>.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public MultiValueConvertEventArgs(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            this.values = values;
            this.targetType = targetType;
            this.parameter = parameter;
            this.culture = culture;

            convertedValue = Binding.DoNothing;
        }

        #endregion
    }
}
