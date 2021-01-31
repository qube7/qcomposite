using System;
using System.Globalization;
using System.Windows.Data;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Represents the converter that converts a <see cref="String"/> value to uppercase equivalent.
    /// </summary>
    public class StringUpperConverter : ValueConverter
    {
        #region Fields

        /// <summary>
        /// Represents the cached instance of the <see cref="StringUpperConverter"/> class.
        /// </summary>
        public static readonly StringUpperConverter Instance = new StringUpperConverter();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StringUpperConverter"/> class.
        /// </summary>
        public StringUpperConverter()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts a <see cref="String"/> value to uppercase equivalent.
        /// </summary>
        /// <param name="value">The <see cref="String"/> value to convert.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The uppercase <see cref="String"/> equivalent of the <paramref name="value"/>.</returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                return value.ToString().ToUpper(culture ?? Culture.Current);
            }

            return Binding.DoNothing;
        }

        /// <summary>
        /// The converter does not support conversion in this direction.
        /// </summary>
        /// <param name="value">This parameter is not used.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>The <see cref="Binding.DoNothing"/>.</returns>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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
            return Instance;
        }

        #endregion
    }
}
