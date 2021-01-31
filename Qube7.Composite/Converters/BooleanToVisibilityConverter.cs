using System;
using System.Globalization;
using System.Windows;
using Qube7.Composite.KnownBoxes;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Represents the converter that converts <see cref="Boolean"/> values to and from <see cref="Visibility"/> enumeration values.
    /// </summary>
    public class BooleanToVisibilityConverter : ValueConverter
    {
        #region Fields

        /// <summary>
        /// Represents the cached instance of the <see cref="BooleanToVisibilityConverter"/> class.
        /// </summary>
        public static readonly BooleanToVisibilityConverter Instance = new BooleanToVisibilityConverter();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanToVisibilityConverter"/> class.
        /// </summary>
        public BooleanToVisibilityConverter()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts a <see cref="Boolean"/> value to a <see cref="Visibility"/> enumeration value.
        /// </summary>
        /// <param name="value">The <see cref="Boolean"/> value to convert. This value can be a standard <see cref="Boolean"/> value or a nullable <see cref="Boolean"/> value.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns><see cref="Visibility.Visible"/> if <paramref name="value"/> is <c>true</c>; otherwise, <see cref="Visibility.Collapsed"/>.</returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? VisibilityBox.Visible : VisibilityBox.Collapsed;
            }

            if (value is bool?)
            {
                bool? nullable = (bool?)value;

                return nullable.HasValue && nullable.Value ? VisibilityBox.Visible : VisibilityBox.Collapsed;
            }

            return VisibilityBox.Collapsed;
        }

        /// <summary>
        /// Converts a <see cref="Visibility"/> enumeration value to a <see cref="Boolean"/> value.
        /// </summary>
        /// <param name="value">A <see cref="Visibility"/> enumeration value.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns><c>true</c> if <paramref name="value"/> is <see cref="Visibility.Visible"/>; otherwise, <c>false</c>.</returns>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible ? BooleanBox.True : BooleanBox.False;
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
