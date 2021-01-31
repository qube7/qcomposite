using System;
using System.Windows.Markup;
using System.Xaml;

namespace Qube7.Composite.Markup
{
    /// <summary>
    /// Provides the root object from markup or from an object graph.
    /// </summary>
    [MarkupExtensionReturnType(typeof(object))]
    public class RootObject : MarkupExtension
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RootObject"/> class.
        /// </summary>
        public RootObject()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider != null)
            {
                IRootObjectProvider provider = serviceProvider.GetService<IRootObjectProvider>();

                return provider?.RootObject;
            }

            return this;
        }

        #endregion
    }
}
