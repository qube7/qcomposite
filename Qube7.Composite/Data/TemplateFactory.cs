using System;
using System.Windows;
using System.Windows.Markup;

namespace Qube7.Composite.Data
{
    /// <summary>
    /// Provides support for creating <see cref="DataTemplate"/> objects.
    /// </summary>
    internal static class TemplateFactory
    {
        #region Fields

        /// <summary>
        /// The default namespace URI.
        /// </summary>
        private const string NamespaceURI = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

        /// <summary>
        /// The XML namespace.
        /// </summary>
        private const string XmlNamespace = "q";

        /// <summary>
        /// The data template format.
        /// </summary>
        private static readonly string Template = string.Format("<{0}><{1}:{{0}}/></{0}>", nameof(DataTemplate), XmlNamespace);

        #endregion

        #region Methods

        /// <summary>
        /// Creates the <see cref="DataTemplate"/> with the root node of the specified type.
        /// </summary>
        /// <param name="rootType">The type of the root node of the <see cref="DataTemplate"/> to create.</param>
        /// <returns>The <see cref="DataTemplate"/> with the root node of the <paramref name="rootType"/>.</returns>
        internal static DataTemplate CreateTemplate(Type rootType)
        {
            ParserContext context = new ParserContext();

            XamlTypeMapper mapper = new XamlTypeMapper(Array.Empty<string>());

            mapper.AddMappingProcessingInstruction(XmlNamespace, rootType.Namespace, rootType.Assembly.FullName);

            context.XamlTypeMapper = mapper;

            context.XmlnsDictionary.Add(string.Empty, NamespaceURI);
            context.XmlnsDictionary.Add(XmlNamespace, XmlNamespace);

            return XamlReader.Parse(string.Format(Template, rootType.Name), context) as DataTemplate;
        }

        #endregion
    }
}
