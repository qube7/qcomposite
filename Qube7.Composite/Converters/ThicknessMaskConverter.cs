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
        #region Fields

        /// <summary>
        /// Represents the cached instance of the <see cref="ThicknessMaskConverter"/> class.
        /// </summary>
        public static readonly ThicknessMaskConverter Instance = new ThicknessMaskConverter();

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
        /// Converts an <see cref="Thickness"/> by multiplying it with the specified <see cref="Thickness"/> mask.
        /// </summary>
        /// <param name="value">The <see cref="Thickness"/> to convert.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">The <see cref="Thickness"/> mask value.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>The converted <see cref="Thickness"/>.</returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Thickness && parameter is Thickness)
            {
                Thickness thickness = (Thickness)value;
                Thickness mask = (Thickness)parameter;

                return new Thickness(thickness.Left * mask.Left, thickness.Top * mask.Top, thickness.Right * mask.Right, thickness.Bottom * mask.Bottom);
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
