using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Represents the converter that converts an <see cref="Thickness"/> value by multiplying it with the <see cref="Thickness"/> mask.
    /// </summary>
    public class ThicknessMaskConverter : ValueConverter
    {
        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="Thickness"/> mask value.
        /// </summary>
        /// <value>The <see cref="Thickness"/> mask value.</value>
        public Thickness Mask { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThicknessMaskConverter"/> class.
        /// </summary>
        public ThicknessMaskConverter()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts an <see cref="Thickness"/> by multiplying it with the <see cref="Mask"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Thickness"/> to convert.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>The converted <see cref="Thickness"/>.</returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Thickness thickness)
            {
                return new Thickness(thickness.Left * Mask.Left, thickness.Top * Mask.Top, thickness.Right * Mask.Right, thickness.Bottom * Mask.Bottom);
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
