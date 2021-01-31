using System.Windows;
using System.Windows.Markup;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Represents a single switch case to match within the <see cref="SwitchConverter"/>.
    /// </summary>
    [ContentProperty(nameof(Then))]
    public class SwitchCase : DependencyObject
    {
        #region Fields

        /// <summary>
        /// Identifies the <see cref="When"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WhenProperty = DependencyProperty.Register(nameof(When), typeof(object), typeof(SwitchCase));

        /// <summary>
        /// Identifies the <see cref="Then"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ThenProperty = DependencyProperty.Register(nameof(Then), typeof(object), typeof(SwitchCase));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the input value to be matched with the value being converted by the <see cref="SwitchConverter"/>.
        /// </summary>
        /// <value>The input value to match.</value>
        public object When
        {
            get { return GetValue(WhenProperty); }
            set { SetValue(WhenProperty, value); }
        }

        /// <summary>
        /// Gets or sets the output value to be returned by the <see cref="SwitchConverter"/> if the current <see cref="SwitchCase"/> is selected.
        /// </summary>
        /// <value>The output value to return.</value>
        public object Then
        {
            get { return GetValue(ThenProperty); }
            set { SetValue(ThenProperty, value); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchCase"/> class.
        /// </summary>
        public SwitchCase()
        {
        }

        #endregion
    }
}
