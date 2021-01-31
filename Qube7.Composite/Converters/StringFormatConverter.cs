using System;
using System.Globalization;
using System.Windows.Data;
using Qube7.Collections;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Represents the converter that converts an array of <see cref="Object"/> to a formatted <see cref="String"/> value based on the composite format string specified in the first element of the array and the objects to format specified as the remaining elements.
    /// </summary>
    public class StringFormatConverter : MultiValueConverter
    {
        #region Fields

        /// <summary>
        /// Represents the cached instance of the <see cref="StringFormatConverter"/> class.
        /// </summary>
        public static readonly StringFormatConverter Instance = new StringFormatConverter();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StringFormatConverter"/> class.
        /// </summary>
        public StringFormatConverter()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts an array of <see cref="Object"/> to a formatted <see cref="String"/> value.
        /// </summary>
        /// <param name="values">The array of <see cref="Object"/> to convert.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The formatted <see cref="String"/> value.</returns>
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > 0 && values[0] is string)
            {
                if (values.Length > 1)
                {
                    object[] args = ArrayHelper.RemoveAt(values, 0);

                    return string.Format(culture ?? Culture.Current, values[0].ToString(), args);
                }

                return values[0];
            }

            return Binding.DoNothing;
        }

        /// <summary>
        /// The converter does not support conversion in this direction.
        /// </summary>
        /// <param name="value">This parameter is not used.</param>
        /// <param name="targetTypes">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>The <c>null</c>.</returns>
        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

        /// <summary>
        /// Returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }

        #endregion
    }
}
