using System;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;

namespace Qube7.Composite.Design
{
    /// <summary>
    /// Specifies the type of the design-time markup partial class.
    /// </summary>
    [MarkupExtensionReturnType(typeof(string))]
    public class DesignClass : MarkupExtension
    {
        #region Properties

        /// <summary>
        /// Gets or sets the type of the design-time markup partial class.
        /// </summary>
        /// <value>The type of the markup partial class.</value>
        [ConstructorArgument("type")]
        public Type Type { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DesignClass"/> class.
        /// </summary>
        /// <param name="type">The type of the markup partial class.</param>
        public DesignClass(Type type) : this()
        {
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DesignClass"/> class.
        /// </summary>
        public DesignClass()
        {
            if (Designer.DesignMode)
            {
                return;
            }

            throw Error.NotSupported(Format.Current(Strings.DesignTimeOnly, nameof(DesignClass)));
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
                Type type = Type;

                if (Designer.DesignMode)
                {
                    IRootObjectProvider provider = serviceProvider.GetService<IRootObjectProvider>();
                    if (provider != null)
                    {
                        DependencyObject element = provider.RootObject as DependencyObject;
                        if (element != null)
                        {
                            Designer.SetDesignType(element, type);
                        }
                    }
                }

                return type?.FullName;
            }

            return this;
        }

        #endregion
    }
}
