using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Provides data for the <see cref="CustomValueConverter.ConvertBack"/> event.
    /// </summary>
    public class ValueConvertBackEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// The value that is produced by the binding target.
        /// </summary>
        private readonly object value;

        /// <summary>
        /// The type to convert to.
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
        /// Gets the value that is produced by the binding target.
        /// </summary>
        /// <value>The value that is produced by the binding target.</value>
        public object Value
        {
            get { return value; }
        }

        /// <summary>
        /// Gets the type to convert to.
        /// </summary>
        /// <value>The type to convert to.</value>
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
        /// Initializes a new instance of the <see cref="ValueConvertBackEventArgs"/> class.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public ValueConvertBackEventArgs(object value, Type targetType, object parameter, CultureInfo culture)
        {
            this.value = value;
            this.targetType = targetType;
            this.parameter = parameter;
            this.culture = culture;

            convertedValue = Binding.DoNothing;
        }

        #endregion
    }
}
