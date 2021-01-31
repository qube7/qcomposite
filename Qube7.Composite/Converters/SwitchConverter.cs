using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Represents the converter that converts values by providing output value of the matched switch case, or default value if none of the switch cases is selected.
    /// </summary>
    [ContentProperty(nameof(Cases))]
    public class SwitchConverter : DependencyObject, IValueConverter
    {
        #region Fields

        /// <summary>
        /// Identifies the <see cref="Default"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultProperty = DependencyProperty.Register(nameof(Default), typeof(object), typeof(SwitchConverter), new PropertyMetadata(Binding.DoNothing));

        /// <summary>
        /// The collection of switch cases.
        /// </summary>
        private readonly Collection<SwitchCase> cases = new Collection<SwitchCase>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of <see cref="SwitchCase"/> objects to match the value being converted with.
        /// </summary>
        /// <value>The collection of <see cref="SwitchCase"/> objects.</value>
        public Collection<SwitchCase> Cases
        {
            get { return cases; }
        }

        /// <summary>
        /// Gets or sets the default return value for the switch selection.
        /// </summary>
        /// <value>The default return value.</value>
        public object Default
        {
            get { return GetValue(DefaultProperty); }
            set { SetValue(DefaultProperty, value); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchConverter"/> class.
        /// </summary>
        public SwitchConverter()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts a value by returning the <see cref="SwitchCase.Then"/> value of the <see cref="SwitchCase"/> selected from the <see cref="Cases"/> collection by matching the <paramref name="value"/> with the <see cref="SwitchCase.When"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Object"/> to match with the <see cref="SwitchCase"/>.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>The <see cref="SwitchCase.Then"/> value of the matched <see cref="SwitchCase"/>, or <see cref="Default"/> value if none of the <see cref="SwitchCase"/> contained in the <see cref="Cases"/> collection was selected.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (SwitchCase item in cases)
            {
                if (item != null && Equals(item.When, value))
                {
                    return item.Then;
                }
            }

            return Default;
        }

        /// <summary>
        /// The converter does not support conversion in this direction.
        /// </summary>
        /// <param name="value">This parameter is not used.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>The <see cref="Binding.DoNothing"/>.</returns>
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        #endregion
    }
}
