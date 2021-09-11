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
        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="CornerRadius"/> mask value.
        /// </summary>
        /// <value>The <see cref="CornerRadius"/> mask value.</value>
        public CornerRadius Mask { get; set; }

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
        /// Converts an <see cref="CornerRadius"/> by multiplying it with the <see cref="Mask"/> value.
        /// </summary>
        /// <param name="value">The <see cref="CornerRadius"/> to convert.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>The converted <see cref="CornerRadius"/>.</returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CornerRadius radius)
            {
                return new CornerRadius(radius.TopLeft * Mask.TopLeft, radius.TopRight * Mask.TopRight, radius.BottomRight * Mask.BottomRight, radius.BottomLeft * Mask.BottomLeft);
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

        #endregion
    }
}
