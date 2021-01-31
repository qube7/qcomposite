using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Qube7.ComponentModel;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Represents the converter that converts an <see cref="IList"/> of data objects to a <see cref="IList"/> of generated item container elements.
    /// </summary>
    public class ItemsConverter : DependencyObject, IValueConverter, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Identifies the <see cref="ItemContainerTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerTemplateProperty = DependencyProperty.Register(nameof(ItemContainerTemplate), typeof(DataTemplate), typeof(ItemsConverter), new PropertyMetadata(OnPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ItemContainerTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerTemplateSelectorProperty = DependencyProperty.Register(nameof(ItemContainerTemplateSelector), typeof(DataTemplateSelector), typeof(ItemsConverter), new PropertyMetadata(OnPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ItemContainerStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register(nameof(ItemContainerStyle), typeof(Style), typeof(ItemsConverter), new PropertyMetadata(OnPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ItemContainerStyleSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleSelectorProperty = DependencyProperty.Register(nameof(ItemContainerStyleSelector), typeof(StyleSelector), typeof(ItemsConverter), new PropertyMetadata(OnPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="P:Item"/> attached dependency property.
        /// </summary>
        private static readonly DependencyProperty ItemProperty = DependencyProperty.RegisterAttached("Item", typeof(object), typeof(ItemsConverter));

        /// <summary>
        /// Identifies the <see cref="P:Style"/> attached dependency property.
        /// </summary>
        private static readonly DependencyProperty StyleProperty = DependencyProperty.RegisterAttached("Style", typeof(Style), typeof(ItemsConverter));

        /// <summary>
        /// The property changed event handler.
        /// </summary>
        private PropertyChangedEventHandler propertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display each item.
        /// </summary>
        /// <value>The <see cref="DataTemplate"/> that specifies the visualization of the data objects.</value>
        public DataTemplate ItemContainerTemplate
        {
            get { return (DataTemplate)GetValue(ItemContainerTemplateProperty); }
            set { SetValue(ItemContainerTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom logic for choosing a template used to display each item.
        /// </summary>
        /// <value>The <see cref="DataTemplateSelector"/> object that returns a <see cref="DataTemplate"/>.</value>
        public DataTemplateSelector ItemContainerTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemContainerTemplateSelectorProperty); }
            set { SetValue(ItemContainerTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> that is applied to the container element generated for each item.
        /// </summary>
        /// <value>The <see cref="Style"/> that is applied to the generated container element.</value>
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom logic for choosing a style that is applied to the container element generated for each item.
        /// </summary>
        /// <value>The <see cref="StyleSelector"/> object that returns a <see cref="Style"/>.</value>
        public StyleSelector ItemContainerStyleSelector
        {
            get { return (StyleSelector)GetValue(ItemContainerStyleSelectorProperty); }
            set { SetValue(ItemContainerStyleSelectorProperty, value); }
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
        /// Initializes a new instance of the <see cref="ItemsConverter"/> class.
        /// </summary>
        public ItemsConverter()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when the effective value of the dependency property changes.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsConverter converter = d as ItemsConverter;
            if (converter != null)
            {
                converter.OnPropertyChanged(new PropertyChangedEventArgs(e.Property.Name));
            }
        }

        /// <summary>
        /// Verifies whether the style with the specified target type is applicable to the specified type.
        /// </summary>
        /// <param name="targetType">The type for which the style is intended.</param>
        /// <param name="type">The type to which to apply the style.</param>
        private static void VerifyStyle(Type targetType, Type type)
        {
            if (targetType == null || targetType.IsAssignableFrom(type))
            {
                return;
            }

            throw Error.InvalidOperation(Format.Current(Strings.StyleTypeInvalid, targetType, type));
        }

        /// <summary>
        /// Applies the style to the specified container element.
        /// </summary>
        /// <param name="container">The container element to apply the style to.</param>
        /// <param name="item">The data object associated with the <paramref name="container"/>.</param>
        /// <param name="style">The <see cref="Style"/> to apply to the container element.</param>
        /// <param name="styleSelector">The <see cref="StyleSelector"/> object that returns a <see cref="Style"/>.</param>
        private static void ApplyStyle(DependencyObject container, object item, Style style, StyleSelector styleSelector)
        {
            if (container.ReadLocalValue(FrameworkElement.StyleProperty) == container.ReadLocalValue(StyleProperty))
            {
                if (style == null && styleSelector != null)
                {
                    style = styleSelector.SelectStyle(item, container);
                }

                if (style != null)
                {
                    VerifyStyle(style.TargetType, container.GetType());

                    container.SetValue(FrameworkElement.StyleProperty, style);
                    container.SetValue(StyleProperty, style);

                    return;
                }

                container.ClearValue(FrameworkElement.StyleProperty);
            }
        }

        /// <summary>
        /// Generates an container element for the specified input data object.
        /// </summary>
        /// <param name="item">The data object for which to generate container.</param>
        /// <param name="template">The <see cref="DataTemplate"/> that specifies the visualization of the data object.</param>
        /// <param name="templateSelector">The <see cref="DataTemplateSelector"/> object that returns a <see cref="DataTemplate"/>.</param>
        /// <param name="style">The <see cref="Style"/> that is applied to the generated container element.</param>
        /// <param name="styleSelector">The <see cref="StyleSelector"/> object that returns a <see cref="Style"/>.</param>
        /// <returns>The container element for the <paramref name="item"/>.</returns>
        private DependencyObject GetContainer(object item, DataTemplate template, DataTemplateSelector templateSelector, Style style, StyleSelector styleSelector)
        {
            DependencyObject container = null;

            if (template == null && templateSelector != null)
            {
                template = templateSelector.SelectTemplate(item, null);
            }

            if (template != null)
            {
                container = template.LoadContent();
            }

            if (container == null)
            {
                container = CreateContainer(item);

                if (container == null)
                {
                    throw Error.ReturnsNull(nameof(CreateContainer));
                }
            }

            container.SetValue(ItemProperty, item);
            container.SetValue(FrameworkElement.DataContextProperty, item);

            ApplyStyle(container, item, style, styleSelector);

            OnContainerCreated(container, item);

            return container;
        }

        /// <summary>
        /// Generates an container element for the specified input data object.
        /// </summary>
        /// <param name="item">The data object for which to generate container.</param>
        /// <returns>The container element for the <paramref name="item"/>.</returns>
        private DependencyObject GetContainer(object item)
        {
            DataTemplate template = ItemContainerTemplate;
            DataTemplateSelector templateSelector = template == null ? ItemContainerTemplateSelector : null;

            Style style = ItemContainerStyle;
            StyleSelector styleSelector = style == null ? ItemContainerStyleSelector : null;

            return GetContainer(item, template, templateSelector, style, styleSelector);
        }

        /// <summary>
        /// Generates a sequence of container elements for the specified <see cref="IList"/> of input data objects.
        /// </summary>
        /// <param name="items">The data objects for which to generate containers.</param>
        /// <returns>The sequence of generated container elements.</returns>
        private IEnumerable<DependencyObject> GetContainers(IList items)
        {
            DataTemplate template = ItemContainerTemplate;
            DataTemplateSelector templateSelector = template == null ? ItemContainerTemplateSelector : null;

            Style style = ItemContainerStyle;
            StyleSelector styleSelector = style == null ? ItemContainerStyleSelector : null;

            for (int i = 0; i < items.Count; i++)
            {
                yield return GetContainer(items[i], template, templateSelector, style, styleSelector);
            }
        }

        /// <summary>
        /// Creates the container element that is used to display the specified data object.
        /// </summary>
        /// <param name="item">The data object for which to create container.</param>
        /// <returns>The container element that is used to display the <paramref name="item"/>.</returns>
        protected virtual DependencyObject CreateContainer(object item)
        {
            return new ContentPresenter();
        }

        /// <summary>
        /// Called when the container element for the specified data object is instantiated.
        /// </summary>
        /// <param name="container">The created container element.</param>
        /// <param name="item">The data object for which the <paramref name="container"/> was created.</param>
        protected virtual void OnContainerCreated(DependencyObject container, object item)
        {
        }

        /// <summary>
        /// Converts an <see cref="IList"/> of data objects to a <see cref="IList"/> of generated item container elements.
        /// </summary>
        /// <param name="value">The <see cref="IList"/> of data objects to convert.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>The <see cref="IList"/> of generated item container elements.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IList items = value as IList;
            if (items != null)
            {
                return new ContainerCollection(items, this);
            }

            return Binding.DoNothing;
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

        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            Event.Raise(propertyChanged, this, e);
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents a collection of generated container elements.
        /// </summary>
        private class ContainerCollection : ReadOnlyCollection<DependencyObject>, INotifyCollectionChanged, INotifyPropertyChanged
        {
            #region Fields

            /// <summary>
            /// The underlying collection.
            /// </summary>
            private readonly ObservableCollection<DependencyObject> collection;

            /// <summary>
            /// The underlying converter.
            /// </summary>
            private readonly ItemsConverter converter;

            /// <summary>
            /// The input data objects.
            /// </summary>
            private readonly IList items;

            /// <summary>
            /// The collection change notification event listener.
            /// </summary>
            private readonly CollectionChangedListener collectionListener;

            /// <summary>
            /// The property change notification event listener.
            /// </summary>
            private readonly PropertyChangedListener propertyListener;

            /// <summary>
            /// The collection changed event handler.
            /// </summary>
            private NotifyCollectionChangedEventHandler collectionChanged;

            /// <summary>
            /// The property changed event handler.
            /// </summary>
            private PropertyChangedEventHandler propertyChanged;

            #endregion

            #region Events

            /// <summary>
            /// Occurs when the collection changes.
            /// </summary>
            event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
            {
                add { Event.Subscribe(ref collectionChanged, value); }
                remove { Event.Unsubscribe(ref collectionChanged, value); }
            }

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
            /// Initializes a new instance of the <see cref="ContainerCollection"/> class.
            /// </summary>
            /// <param name="collection">The underlying collection.</param>
            private ContainerCollection(ObservableCollection<DependencyObject> collection) : base(collection)
            {
                this.collection = collection;

                collection.CollectionChanged += HandleCollectionChanged;

                ((INotifyPropertyChanged)collection).PropertyChanged += HandlePropertyChanged;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ContainerCollection"/> class.
            /// </summary>
            /// <param name="items">The input data objects.</param>
            /// <param name="converter">The underlying converter.</param>
            internal ContainerCollection(IList items, ItemsConverter converter) : this(new ObservableCollection<DependencyObject>(converter.GetContainers(items)))
            {
                this.converter = converter;

                propertyListener = new PropertyChangedListener(this);

                PropertyChangedEvent.AddListener(converter, propertyListener);

                INotifyCollectionChanged collection = items as INotifyCollectionChanged;
                if (collection != null)
                {
                    this.items = items;

                    collectionListener = new CollectionChangedListener(this);

                    CollectionChangedEvent.AddListener(collection, collectionListener);
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called when the collection changes.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
            private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                OnCollectionChanged(e);
            }

            /// <summary>
            /// Called when a property value changes.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
            private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                OnPropertyChanged(e);
            }

            /// <summary>
            /// Raises the <see cref="INotifyCollectionChanged.CollectionChanged"/> event.
            /// </summary>
            /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
            private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            {
                Event.Raise(collectionChanged, this, e);
            }

            /// <summary>
            /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
            /// </summary>
            /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
            private void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                Event.Raise(propertyChanged, this, e);
            }

            /// <summary>
            /// Called when the collection changes.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
            private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                converter.VerifyAccess();

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        VerifyRange(e.NewItems);
                        OnItemAdded(e.NewItems[0], e.NewStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        VerifyRange(e.OldItems);
                        OnItemRemoved(e.OldStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        VerifyRange(e.OldItems);
                        VerifyRange(e.NewItems);
                        OnItemReplaced(e.NewItems[0], e.NewStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        VerifyRange(e.OldItems);
                        OnItemMoved(e.OldStartingIndex, e.NewStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        OnItemsReset();
                        break;
                }
            }

            /// <summary>
            /// Verifies whether the collection change is single item change.
            /// </summary>
            /// <param name="items">The list of items involved in the change.</param>
            private static void VerifyRange(IList items)
            {
                if (items.Count == 1)
                {
                    return;
                }

                throw Error.NotSupported(Strings.RangeAction);
            }

            /// <summary>
            /// Called when an item was added to the collection.
            /// </summary>
            /// <param name="item">The item that was added to the collection.</param>
            /// <param name="index">The zero-based index at which the item was added.</param>
            private void OnItemAdded(object item, int index)
            {
                if (index < 0)
                {
                    throw Error.NotSupported(Strings.ChangeIndexExpected);
                }

                collection.Insert(index, converter.GetContainer(item));
            }

            /// <summary>
            /// Called when an item was removed from the collection.
            /// </summary>
            /// <param name="index">The zero-based index at which the item was removed.</param>
            private void OnItemRemoved(int index)
            {
                if (index < 0)
                {
                    throw Error.NotSupported(Strings.ChangeIndexExpected);
                }

                collection.RemoveAt(index);
            }

            /// <summary>
            /// Called when an item was replaced in the collection.
            /// </summary>
            /// <param name="item">The item that was set in the collection.</param>
            /// <param name="index">The zero-based index at which the item was replaced.</param>
            private void OnItemReplaced(object item, int index)
            {
                if (index < 0)
                {
                    throw Error.NotSupported(Strings.ChangeIndexExpected);
                }

                collection[index] = converter.GetContainer(item);
            }

            /// <summary>
            /// Called when an item was moved within the collection.
            /// </summary>
            /// <param name="oldIndex">The zero-based index from which the item was moved.</param>
            /// <param name="newIndex">The zero-based index to which the item was moved.</param>
            private void OnItemMoved(int oldIndex, int newIndex)
            {
                if (oldIndex < 0 || newIndex < 0)
                {
                    throw Error.NotSupported(Strings.ChangeIndexExpected);
                }

                collection.Move(oldIndex, newIndex);
            }

            /// <summary>
            /// Called when the contents of the collection changed dramatically.
            /// </summary>
            private void OnItemsReset()
            {
                collection.Clear();

                foreach (DependencyObject container in converter.GetContainers(items))
                {
                    collection.Add(container);
                }
            }

            /// <summary>
            /// Called when a property value changes.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
            private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(ItemContainerTemplate) || (e.PropertyName == nameof(ItemContainerTemplateSelector) && converter.ItemContainerTemplate == null))
                {
                    DataTemplate template = converter.ItemContainerTemplate;
                    DataTemplateSelector templateSelector = template == null ? converter.ItemContainerTemplateSelector : null;

                    Style style = converter.ItemContainerStyle;
                    StyleSelector styleSelector = style == null ? converter.ItemContainerStyleSelector : null;

                    for (int i = 0; i < collection.Count; i++)
                    {
                        DependencyObject container = collection[i];

                        object item = container.ReadLocalValue(ItemProperty);

                        collection[i] = converter.GetContainer(item, template, templateSelector, style, styleSelector);
                    }

                    return;
                }

                if (e.PropertyName == nameof(ItemContainerStyle) || (e.PropertyName == nameof(ItemContainerStyleSelector) && converter.ItemContainerStyle == null))
                {
                    Style style = converter.ItemContainerStyle;
                    StyleSelector styleSelector = style == null ? converter.ItemContainerStyleSelector : null;

                    for (int i = 0; i < collection.Count; i++)
                    {
                        DependencyObject container = collection[i];

                        object item = container.ReadLocalValue(ItemProperty);

                        ApplyStyle(container, item, style, styleSelector);
                    }

                    return;
                }
            }

            #endregion

            #region Nested types

            /// <summary>
            /// Represents an listener of the collection change notification event.
            /// </summary>
            private class CollectionChangedListener : EventListener<NotifyCollectionChangedEventArgs, ContainerCollection>
            {
                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="CollectionChangedListener"/> class.
                /// </summary>
                /// <param name="handler">The event handler object.</param>
                internal CollectionChangedListener(ContainerCollection handler) : base(handler)
                {
                }

                #endregion

                #region Methods

                /// <summary>
                /// Handles the subscribed event using the specified handler object.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">An <see cref="NotifyCollectionChangedEventArgs"/> that contains the event data.</param>
                /// <param name="handler">The event handler object.</param>
                protected override void HandleEvent(object sender, NotifyCollectionChangedEventArgs e, ContainerCollection handler)
                {
                    handler.OnCollectionChanged(sender, e);
                }

                #endregion
            }

            /// <summary>
            /// Represents an listener of the property change notification event.
            /// </summary>
            private class PropertyChangedListener : EventListener<PropertyChangedEventArgs, ContainerCollection>
            {
                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="PropertyChangedListener"/> class.
                /// </summary>
                /// <param name="handler">The event handler object.</param>
                internal PropertyChangedListener(ContainerCollection handler) : base(handler)
                {
                }

                #endregion

                #region Methods

                /// <summary>
                /// Handles the subscribed event using the specified handler object.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">An <see cref="PropertyChangedEventArgs"/> that contains the event data.</param>
                /// <param name="handler">The event handler object.</param>
                protected override void HandleEvent(object sender, PropertyChangedEventArgs e, ContainerCollection handler)
                {
                    handler.OnPropertyChanged(sender, e);
                }

                #endregion
            }

            #endregion
        }

        #endregion
    }
}
