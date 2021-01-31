using System;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;
using Qube7.Composite.Design;

namespace Qube7.Composite.Data
{
    /// <summary>
    /// Implements a markup extension that binds property of the target object to the resource provided by the <see cref="ResourceProvider"/>.
    /// </summary>
    [MarkupExtensionReturnType(typeof(object))]
    public class Resource : MarkupExtension
    {
        #region Fields

        /// <summary>
        /// The indexer format.
        /// </summary>
        private const string Indexer = "[{0}]";

        /// <summary>
        /// The name of the resource.
        /// </summary>
        private string name;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the source type used for retrieving the <see cref="ResourceProvider"/>.
        /// </summary>
        /// <value>The source type used for retrieving the <see cref="ResourceProvider"/>.</value>
        public Type SourceType { get; set; }

        /// <summary>
        /// Gets or sets the name of the resource.
        /// </summary>
        /// <value>The name of the resource.</value>
        [ConstructorArgument("name")]
        public string Name
        {
            get { return name; }
            set
            {
                Requires.NotNullOrEmpty(value, nameof(value));

                name = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        public Resource(string name)
        {
            Requires.NotNullOrEmpty(name, nameof(name));

            this.name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        public Resource()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        /// <remarks>This method returns the <see cref="BindingExpression"/>, if the markup extension targets a <see cref="DependencyProperty"/> on a <see cref="DependencyObject"/>; otherwise, the <see cref="Binding"/> object with the resource binding source set.</remarks>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (name == null)
            {
                throw Error.InvalidOperation(Strings.ResourceNameExpected);
            }

            if (serviceProvider != null)
            {
                Type source = SourceType ?? GetRootType(serviceProvider);
                if (source != null)
                {
                    ResourceProvider provider = ResourceProvider.FromType(source);

                    Binding binding = new Binding(string.Format(Indexer, name)) { Source = provider, Mode = BindingMode.OneWay };

                    return binding.ProvideValue(serviceProvider);
                }
            }

            return this;
        }

        /// <summary>
        /// Gets the source type for the root object from markup or from an object graph.
        /// </summary>
        /// <param name="serviceProvider">The object that can provide services for the markup extension.</param>
        /// <returns>The source type for the root object.</returns>
        private static Type GetRootType(IServiceProvider serviceProvider)
        {
            IRootObjectProvider provider = serviceProvider.GetService<IRootObjectProvider>();
            if (provider != null)
            {
                object root = provider.RootObject;
                if (root != null)
                {
                    if (Designer.DesignMode)
                    {
                        DependencyObject element = root as DependencyObject;

                        return element != null ? Designer.GetDesignType(element) : null;
                    }

                    return root.GetType();
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="Resource"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> that represents the current <see cref="Resource"/>.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("{");
            builder.Append(nameof(Resource));
            builder.Append(" ");
            builder.Append(name ?? string.Empty);

            Type source = SourceType;
            if (source != null)
            {
                builder.Append(", ");
                builder.Append(nameof(SourceType));
                builder.Append("=");
                builder.Append(source.Name);
            }

            builder.Append("}");

            return builder.ToString();
        }

        #endregion
    }
}
