using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Represents the converter that converts an <see cref="CornerRadius"/> value by multiplying it with the <see cref="CornerRadius"/> mask.
    /// </summary>
    public class CornerRadiusMaskConverter : ValueConverter
    {
        #region Fields

        /// <summary>
        /// Represents the cached instance of the <see cref="CornerRadiusMaskConverter"/> class.
        /// </summary>
        public static readonly CornerRadiusMaskConverter Instance = new CornerRadiusMaskConverter();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CornerRadiusMaskConverter"/> class.
        /// </summary>
        public CornerRadiusMaskConverter()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts an <see cref="CornerRadius"/> by multiplying it with the specified <see cref="CornerRadius"/> mask.
        /// </summary>
        /// <param name="value">The <see cref="CornerRadius"/> to convert.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">The <see cref="CornerRadius"/> mask value.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>The converted <see cref="CornerRadius"/>.</returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CornerRadius && parameter is CornerRadius)
            {
                CornerRadius radius = (CornerRadius)value;
                CornerRadius mask = (CornerRadius)parameter;

                return new CornerRadius(radius.TopLeft * mask.TopLeft, radius.TopRight * mask.TopRight, radius.BottomRight * mask.BottomRight, radius.BottomLeft * mask.BottomLeft);
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
