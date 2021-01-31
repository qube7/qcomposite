using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Represents the converter that converts values by executing in a chain the converters contained in the <see cref="Converters"/> collection.
    /// </summary>
    [ContentProperty(nameof(Converters))]
    public class CompositeValueConverter : IValueConverter
    {
        #region Fields

        /// <summary>
        /// The converters collection.
        /// </summary>
        private readonly ConverterCollection converters = new ConverterCollection();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of <see cref="IValueConverter"/> to execute in a chain.
        /// </summary>
        /// <value>The converters collection.</value>
        public Collection<IValueConverter> Converters
        {
            get { return converters; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValueConverter"/> class.
        /// </summary>
        public CompositeValueConverter()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts a value by iterating through the <see cref="Converters"/> collection and executing <see cref="IValueConverter.Convert"/> method for each <see cref="IValueConverter"/> contained. The return value of the <see cref="IValueConverter.Convert"/> method call for current <see cref="IValueConverter"/> in the iteration is passed as input value for processing next <see cref="IValueConverter"/> in the iteration.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value, meaning the result of processing the chain of converters.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            for (int i = 0; i < converters.Count; i++)
            {
                value = converters[i].Convert(value, targetType, parameter, culture);

                if (value == Binding.DoNothing)
                {
                    break;
                }
            }

            return value;
        }

        /// <summary>
        /// Converts a value by iterating in reverse order through the <see cref="Converters"/> collection and executing <see cref="IValueConverter.ConvertBack"/> method for each <see cref="IValueConverter"/> contained. The return value of the <see cref="IValueConverter.ConvertBack"/> method call for current <see cref="IValueConverter"/> in the iteration is passed as input value for processing next <see cref="IValueConverter"/> in the iteration.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value, meaning the result of processing the reversed chain of converters.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            for (int i = converters.Count - 1; i >= 0; i--)
            {
                value = converters[i].ConvertBack(value, targetType, parameter, culture);

                if (value == Binding.DoNothing)
                {
                    break;
                }
            }

            return value;
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents the collection of <see cref="IValueConverter"/> to execute in a chain.
        /// </summary>
        private class ConverterCollection : Collection<IValueConverter>
        {
            #region Methods

            /// <summary>
            /// Inserts an element into the <see cref="ConverterCollection"/> at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
            /// <param name="item">The object to insert.</param>
            protected override void InsertItem(int index, IValueConverter item)
            {
                Requires.NotNull(item, nameof(item));

                base.InsertItem(index, item);
            }

            /// <summary>
            /// Replaces the element at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index of the element to replace.</param>
            /// <param name="item">The new value for the element at the specified index.</param>
            protected override void SetItem(int index, IValueConverter item)
            {
                Requires.NotNull(item, nameof(item));

                base.SetItem(index, item);
            }

            #endregion
        }

        #endregion
    }
}
