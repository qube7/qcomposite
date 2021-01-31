using System;
using System.ComponentModel;
using System.Windows;

namespace Qube7.Composite.Design
{
    /// <summary>
    /// Provides support for the designer.
    /// </summary>
    internal static class Designer
    {
        #region Fields

        /// <summary>
        /// Identifies the <see cref="P:DesignType"/> attached dependency property.
        /// </summary>
        private static readonly DependencyProperty DesignTypeProperty = DependencyProperty.RegisterAttached("DesignType", typeof(Type), typeof(Designer), new PropertyMetadata(null, null, OnDesignTypeCoerce));

        /// <summary>
        /// A value indicating whether the process is running in design mode.
        /// </summary>
        private static readonly bool enabled;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the process is running in design mode.
        /// </summary>
        /// <value><c>true</c> if the process is running in design mode; otherwise, <c>false</c>.</value>
        internal static bool DesignMode
        {
            get { return enabled; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="Designer"/> class.
        /// </summary>
        static Designer()
        {
            enabled = (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(DependencyObject)).Metadata.DefaultValue;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when coercion of the <see cref="DesignTypeProperty"/> value is requested.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which the property value is to be coerced.</param>
        /// <param name="baseValue">The value to coerce.</param>
        /// <returns>The coerced value.</returns>
        private static object OnDesignTypeCoerce(DependencyObject d, object baseValue)
        {
            object value = d.ReadLocalValue(DesignTypeProperty);

            return value == DependencyProperty.UnsetValue ? baseValue : value;
        }

        /// <summary>
        /// Gets the value of the <see cref="P:DesignType"/> attached property for a specified <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="element">The element from which the property value is read.</param>
        /// <returns>The <see cref="P:DesignType"/> property value for the <paramref name="element"/>.</returns>
        internal static Type GetDesignType(DependencyObject element)
        {
            Requires.NotNull(element, nameof(element));

            return (Type)element.GetValue(DesignTypeProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="P:DesignType"/> attached property to a specified <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="element">The element to which the property value is written.</param>
        /// <param name="value">The <see cref="P:DesignType"/> property value to set.</param>
        internal static void SetDesignType(DependencyObject element, Type value)
        {
            Requires.NotNull(element, nameof(element));

            element.SetValue(DesignTypeProperty, value);
        }

        #endregion
    }
}
