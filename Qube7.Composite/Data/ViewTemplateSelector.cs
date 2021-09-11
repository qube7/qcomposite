using System.Windows;
using System.Windows.Controls;
using Qube7.Composite.Presentation;

namespace Qube7.Composite.Data
{
    /// <summary>
    /// Provides the <see cref="DataTemplate"/> for the view template for the <see cref="ViewModel"/>.
    /// </summary>
    public class ViewTemplateSelector : DataTemplateSelector
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the view template to select.
        /// </summary>
        /// <value>The name of the view template.</value>
        public string TemplateName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewTemplateSelector"/> class.
        /// </summary>
        public ViewTemplateSelector()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the <see cref="DataTemplate"/> for the specified object.
        /// </summary>
        /// <param name="item">The object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>The <see cref="DataTemplate"/> for the <paramref name="item"/>.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ViewModel model)
            {
                return model.ProvideTemplate(TemplateName);
            }

            return base.SelectTemplate(item, container);
        }

        #endregion
    }
}
