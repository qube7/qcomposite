using System;
using System.Globalization;
using System.Windows.Data;
using Qube7.Composite.KnownBoxes;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Represents the converter that converts an <see cref="Object"/> to a <see cref="Boolean"/> value by checking whether the reference is not <c>null</c> or <see cref="DBNull.Value"/>.
    /// </summary>
    public class IsNotNullConverter : ValueConverter
    {
        #region Fields

        /// <summary>
        /// Represents the cached instance of the <see cref="IsNotNullConverter"/> class.
        /// </summary>
        public static readonly IsNotNullConverter Instance = new IsNotNullConverter();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IsNotNullConverter"/> class.
        /// </summary>
        public IsNotNullConverter()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts an <see cref="Object"/> to a <see cref="Boolean"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Object"/> to convert.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns><c>true</c> if <paramref name="value"/> is not <c>null</c> or <see cref="DBNull.Value"/>; otherwise, <c>false</c>.</returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null || value == DBNull.Value ? BooleanBox.False : BooleanBox.True;
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
