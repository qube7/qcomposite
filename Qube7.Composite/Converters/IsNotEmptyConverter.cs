using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;
using Qube7.Composite.KnownBoxes;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Represents the converter that converts an <see cref="IEnumerable"/> to a <see cref="Boolean"/> value by checking whether the sequence is not <c>null</c> and contains any elements.
    /// </summary>
    public class IsNotEmptyConverter : ValueConverter
    {
        #region Fields

        /// <summary>
        /// Represents the cached instance of the <see cref="IsNotEmptyConverter"/> class.
        /// </summary>
        public static readonly IsNotEmptyConverter Instance = new IsNotEmptyConverter();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IsNotEmptyConverter"/> class.
        /// </summary>
        public IsNotEmptyConverter()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts an <see cref="IEnumerable"/> to a <see cref="Boolean"/> value.
        /// </summary>
        /// <param name="value">The <see cref="IEnumerable"/> to convert.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns><c>true</c> if <paramref name="value"/> is sequence that is not <c>null</c> and contains any elements; otherwise, <c>false</c>.</returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return BooleanBox.False;
            }

            if (value is string)
            {
                return value.ToString().Length > 0 ? BooleanBox.True : BooleanBox.False;
            }

            IEnumerable enumerable = value as IEnumerable;

            if (enumerable != null)
            {
                IEnumerator enumerator = enumerable.GetEnumerator();

                if (enumerator != null && enumerator.MoveNext())
                {
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
