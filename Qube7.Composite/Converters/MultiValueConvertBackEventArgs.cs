using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Provides data for the <see cref="CustomMultiValueConverter.ConvertBack"/> event.
    /// </summary>
    public class MultiValueConvertBackEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// The value that is produced by the binding target.
        /// </summary>
        private readonly object value;

        /// <summary>
        /// The array of types to convert to.
        /// </summary>
        private readonly Type[] targetTypes;

        /// <summary>
        /// The converter parameter to use.
        /// </summary>
        private readonly object parameter;

        /// <summary>
        /// The culture to use in the converter.
        /// </summary>
        private readonly CultureInfo culture;

        /// <summary>
        /// An array of values that have been converted from the target value back to the source values.
        /// </summary>
        private object[] convertedValues;

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
        /// Gets the array of types to convert to.
        /// </summary>
        /// <value>The array of types to convert to.</value>
        /// <remarks>The array length indicates the number and types of values that are suggested for the converter to return.</remarks>
        public Type[] TargetTypes
        {
            get { return targetTypes; }
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
        /// Gets or sets an array of values that have been converted from the target value back to the source values.
        /// </summary>
        /// <value>An array of values that have been converted from the target value back to the source values.</value>
        /// <remarks>The <c>null</c> value at position i indicates that the valid <c>null</c> value is used for the source binding at index i. The <see cref="DependencyProperty.UnsetValue"/> at position i indicates that the converter is unable to provide a value for the source binding at index i, and that no value is to be set on it. The <see cref="Binding.DoNothing"/> at position i indicates that no value is to be set on the source binding at index i. The <c>null</c> indicates that the converter cannot perform the conversion or that it does not support conversion in this direction.</remarks>
        public object[] ConvertedValues
        {
            get { return convertedValues; }
            set { convertedValues = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiValueConvertBackEventArgs"/> class.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetTypes">The array of types to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public MultiValueConvertBackEventArgs(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            this.value = value;
            this.targetTypes = targetTypes;
            this.parameter = parameter;
            this.culture = culture;
        }

        #endregion
    }
}
