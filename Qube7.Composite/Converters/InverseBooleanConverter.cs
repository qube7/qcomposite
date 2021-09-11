using System;
using System.Globalization;
using System.Windows.Data;
using Qube7.Composite.KnownBoxes;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Represents the converter that converts <see cref="Boolean"/> values, <c>true</c> value to <c>false</c> and <c>false</c> value to <c>true</c>.
    /// </summary>
    public class InverseBooleanConverter : ValueConverter
    {
        #region Fields

        /// <summary>
        /// Represents the cached instance of the <see cref="InverseBooleanConverter"/> class.
        /// </summary>
        public static readonly InverseBooleanConverter Instance = new InverseBooleanConverter();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InverseBooleanConverter"/> class.
        /// </summary>
        public InverseBooleanConverter()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts a <see cref="Boolean"/> value, <c>true</c> value to <c>false</c> or <c>false</c> value to <c>true</c>.
        /// </summary>
        /// <param name="value">The <see cref="Boolean"/> value to convert. This value can be a standard <see cref="Boolean"/> value or a nullable <see cref="Boolean"/> value.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns><c>false</c> if <paramref name="value"/> is <c>true</c>; otherwise, <c>true</c>.</returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolean)
            {
                return boolean ? BooleanBox.False : BooleanBox.True;
            }

            if (value == null)
            {
                return null;
            }

            return Binding.DoNothing;
        }

        /// <summary>
        /// Converts a <see cref="Boolean"/> value, <c>false</c> value to <c>true</c> or <c>true</c> value to <c>false</c>.
        /// </summary>
        /// <param name="value">The <see cref="Boolean"/> value to convert. This value can be a standard <see cref="Boolean"/> value or a nullable <see cref="Boolean"/> value.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns><c>true</c> if <paramref name="value"/> is <c>false</c>; otherwise, <c>false</c>.</returns>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
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
