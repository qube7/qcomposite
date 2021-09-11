using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Qube7.Composite.Data
{
    /// <summary>
    /// Provides access to the culture-specific resources.
    /// </summary>
    public class ResourceProvider : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// The providers table.
        /// </summary>
        private static readonly ConditionalWeakTable<Type, ResourceProvider> table = new ConditionalWeakTable<Type, ResourceProvider>();

        /// <summary>
        /// The underlying resource manager.
        /// </summary>
        private readonly ResourceManager manager;

        /// <summary>
        /// The property changed event handler.
        /// </summary>
        private PropertyChangedEventHandler propertyChanged;

        #endregion

        #region Indexers

        /// <summary>
        /// Gets the value of the specified resource.
        /// </summary>
        /// <param name="name">The name of the resource to get.</param>
        /// <value>The value of the localized resource.</value>
        public object this[string name]
        {
            get { return manager.GetObject(name, Culture.Current); }
        }

        /// <summary>
        /// Gets the value of the specified resource localized for the specified culture.
        /// </summary>
        /// <param name="name">The name of the resource to get.</param>
        /// <param name="culture">The culture for which the resource is localized.</param>
        /// <value>The value of the localized resource.</value>
        public object this[string name, CultureInfo culture]
        {
            get { return manager.GetObject(name, culture ?? Culture.Current); }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { Event.Subscribe(ref propertyChanged, value); }
            remove { Event.Unsubscribe(ref propertyChanged, value); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="ResourceProvider"/> class.
        /// </summary>
        static ResourceProvider()
        {
            Culture.CurrentChanged += OnCultureChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceProvider"/> class.
        /// </summary>
        /// <param name="source">The source type.</param>
        private ResourceProvider(Type source)
        {
            manager = new ResourceManager(source);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns an <see cref="ResourceProvider"/> created for the specified source type.
        /// </summary>
        /// <param name="source">The source type.</param>
        /// <returns>The <see cref="ResourceProvider"/> for the <paramref name="source"/>.</returns>
        public static ResourceProvider FromType(Type source)
        {
            Requires.NotNull(source, nameof(source));

            return table.GetValue(source, t => new ResourceProvider(t));
        }

        /// <summary>
        /// Called when the user interface culture associated with the current application changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private static void OnCultureChanged(object sender, EventArgs e)
        {
            ResourceProvider[] providers = table.Select(p => p.Value).ToArray();

            for (int i = 0; i < providers.Length; i++)
            {
                providers[i].OnPropertyChanged(EventArgsCache.ObjectPropertyChanged);
            }
        }

        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            Event.Raise(propertyChanged, this, e);
        }

        #endregion
    }
}
