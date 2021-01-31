using System;
using System.Globalization;
using System.Windows.Data;
using Qube7.Composite.KnownBoxes;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Represents the converter that converts an array of <see cref="Object"/> to a <see cref="Boolean"/> value by checking whether elements of array are not equal.
    /// </summary>
    public class AreNotEqualConverter : MultiValueConverter
    {
        #region Fields

        /// <summary>
        /// Represents the cached instance of the <see cref="AreNotEqualConverter"/> class.
        /// </summary>
        public static readonly AreNotEqualConverter Instance = new AreNotEqualConverter();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AreNotEqualConverter"/> class.
        /// </summary>
        public AreNotEqualConverter()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts an array of <see cref="Object"/> to a <see cref="Boolean"/> value.
        /// </summary>
        /// <param name="values">The array of <see cref="Object"/> to convert.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns><c>true</c> if <paramref name="values"/> are not equal; otherwise, <c>false</c>.</returns>
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > 1)
            {
                for (int i = 0; i < values.Length - 1; i++)
                {
                    if (values[i] == null)
                    {
                        if (values[i + 1] == null)
                        {
                            continue;
                        }

                        return BooleanBox.True;
                    }

                    if (values[i].Equals(values[i + 1]))
                    {
                        continue;
                    }

                    return BooleanBox.True;
                }

                return BooleanBox.False;
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
