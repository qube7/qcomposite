using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Qube7.ComponentModel;

namespace Qube7.Composite.Converters
{
    /// <summary>
    /// Represents the converter that groups and sorts the elements of a sequence of <see cref="DependencyObject"/>.
    /// </summary>
    public class Grouping : DependencyObject, IValueConverter, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Identifies the <see cref="P:GroupName"/> attached dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.RegisterAttached("GroupName", typeof(string), typeof(Grouping));

        /// <summary>
        /// Identifies the <see cref="P:GroupIndex"/> attached dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupIndexProperty = DependencyProperty.RegisterAttached("GroupIndex", typeof(int), typeof(Grouping));

        /// <summary>
        /// Identifies the <see cref="GroupComparer"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupComparerProperty = DependencyProperty.Register(nameof(GroupComparer), typeof(StringComparer), typeof(Grouping), new PropertyMetadata(OnPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="GroupSeparatorTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupSeparatorTemplateProperty = DependencyProperty.Register(nameof(GroupSeparatorTemplate), typeof(DataTemplate), typeof(Grouping), new PropertyMetadata(OnPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="P:Grouping"/> attached dependency property.
        /// </summary>
        private static readonly DependencyProperty GroupingProperty = DependencyProperty.RegisterAttached("Grouping", typeof(Grouping), typeof(Grouping));

        /// <summary>
        /// The property changed event handler.
        /// </summary>
        private PropertyChangedEventHandler propertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="StringComparer"/> used to compare and order groups.
        /// </summary>
        /// <value>The <see cref="StringComparer"/> used to compare and order groups.</value>
        public StringComparer GroupComparer
        {
            get { return (StringComparer)GetValue(GroupComparerProperty); }
            set { SetValue(GroupComparerProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display the group separator.
        /// </summary>
        /// <value>The <see cref="DataTemplate"/> that specifies the visualization of the group separator.</value>
        public DataTemplate GroupSeparatorTemplate
        {
            get { return (DataTemplate)GetValue(GroupSeparatorTemplateProperty); }
            set { SetValue(GroupSeparatorTemplateProperty, value); }
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
        /// Initializes a new instance of the <see cref="Grouping"/> class.
        /// </summary>
        public Grouping()
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
            Grouping grouping = d as Grouping;
            if (grouping != null)
            {
                grouping.OnPropertyChanged(new PropertyChangedEventArgs(e.Property.Name));
            }
        }

        /// <summary>
        /// Gets the name of the group that specified <see cref="DependencyObject"/> should appear in.
        /// </summary>
        /// <param name="element">The element for which to get the group name.</param>
        /// <returns>The name of the group for the <paramref name="element"/>.</returns>
        public static string GetGroupName(DependencyObject element)
        {
            Requires.NotNull(element, nameof(element));

            return (string)element.GetValue(GroupNameProperty);
        }

        /// <summary>
        /// Sets the name of the group that specified <see cref="DependencyObject"/> should appear in.
        /// </summary>
        /// <param name="element">The element for which to set the group name.</param>
        /// <param name="value">The name of the group for the <paramref name="element"/>.</param>
        public static void SetGroupName(DependencyObject element, string value)
        {
            Requires.NotNull(element, nameof(element));

            element.SetValue(GroupNameProperty, value);
        }

        /// <summary>
        /// Gets the position within a group at which the specified <see cref="DependencyObject"/> should appear.
        /// </summary>
        /// <param name="element">The element for which to get the position within a group.</param>
        /// <returns>The position within a group of the <paramref name="element"/>.</returns>
        public static int GetGroupIndex(DependencyObject element)
        {
            Requires.NotNull(element, nameof(element));

            return (int)element.GetValue(GroupIndexProperty);
        }

        /// <summary>
        /// Sets the position within a group at which the specified <see cref="DependencyObject"/> should appear.
        /// </summary>
        /// <param name="element">The element for which to set the position within a group.</param>
        /// <param name="value">The position within a group of the <paramref name="element"/>.</param>
        public static void SetGroupIndex(DependencyObject element, int value)
        {
            Requires.NotNull(element, nameof(element));

            element.SetValue(GroupIndexProperty, value);
        }

        /// <summary>
        /// Groups and sorts the elements of the specified sequence of <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="enumerable">The sequence of <see cref="DependencyObject"/> to refresh.</param>
        /// <returns>The sequence whose elements are grouped and sorted.</returns>
        private IEnumerable<DependencyObject> GroupSort(IEnumerable<DependencyObject> enumerable)
        {
            List<DependencyObject> separators = new List<DependencyObject>();

            List<Tuple<DependencyObject, string, int>> tuples = new List<Tuple<DependencyObject, string, int>>();

            foreach (DependencyObject element in enumerable)
            {
                if (element != null)
                {
                    if (element.ReadLocalValue(GroupingProperty) == this)
                    {
                        separators.Add(element);

                        continue;
                    }

                    tuples.Add(Tuple.Create(element, GetGroupName(element) ?? string.Empty, GetGroupIndex(element)));
                }
            }

            DataTemplate template = GroupSeparatorTemplate;
            StringComparer comparer = GroupComparer ?? StringComparer.InvariantCulture;

            List<DependencyObject> list = new List<DependencyObject>();

            foreach (IGrouping<string, Tuple<DependencyObject, string, int>> grouping in tuples.GroupBy(t => t.Item2, comparer).OrderBy(g => g.Key, comparer))
            {
                if (list.Count > 0)
                {
                    if (separators.Count > 0)
                    {
                        list.Add(separators[0]);

                        separators.RemoveAt(0);
                    }
                    else if (template != null)
                    {
                        DependencyObject separator = template.LoadContent();
                        if (separator != null)
                        {
                            separator.SetValue(GroupingProperty, this);

                            list.Add(separator);
                        }
                    }
                }

                foreach (Tuple<DependencyObject, string, int> tuple in grouping.OrderBy(t => t.Item3))
                {
                    list.Add(tuple.Item1);
                }
            }

            return list;
        }

        /// <summary>
        /// Groups and sorts the elements of a sequence of <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="value">The sequence of <see cref="DependencyObject"/> to convert.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>The sequence whose elements are grouped and sorted.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<DependencyObject> enumerable = value as IEnumerable<DependencyObject>;
            if (enumerable != null)
            {
                return new GroupingCollection(enumerable, this);
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
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            Event.Raise(propertyChanged, this, e);
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents a collection of grouped and sorted elements.
        /// </summary>
        private class GroupingCollection : ReadOnlyCollection<DependencyObject>, INotifyCollectionChanged, INotifyPropertyChanged
        {
            #region Fields

            /// <summary>
            /// The underlying collection.
            /// </summary>
            private readonly RecomposableCollection<DependencyObject> collection;

            /// <summary>
            /// The underlying grouping.
            /// </summary>
            private readonly Grouping grouping;

            /// <summary>
            /// The input sequence.
            /// </summary>
            private readonly IEnumerable<DependencyObject> enumerable;

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
            /// Initializes a new instance of the <see cref="GroupingCollection"/> class.
            /// </summary>
            /// <param name="collection">The underlying collection.</param>
            private GroupingCollection(RecomposableCollection<DependencyObject> collection) : base(collection)
            {
                this.collection = collection;

                ((INotifyCollectionChanged)collection).CollectionChanged += HandleCollectionChanged;

                ((INotifyPropertyChanged)collection).PropertyChanged += HandlePropertyChanged;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="GroupingCollection"/> class.
            /// </summary>
            /// <param name="enumerable">The input sequence.</param>
            /// <param name="grouping">The underlying grouping.</param>
            internal GroupingCollection(IEnumerable<DependencyObject> enumerable, Grouping grouping) : this(new RecomposableCollection<DependencyObject>(grouping.GroupSort(enumerable)))
            {
                this.grouping = grouping;

                propertyListener = new PropertyChangedListener(this);

                PropertyChangedEvent.AddListener(grouping, propertyListener);

                INotifyCollectionChanged collection = enumerable as INotifyCollectionChanged;
                if (collection != null)
                {
                    this.enumerable = enumerable;

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
                grouping.VerifyAccess();

                collection.Recompose(grouping.GroupSort(enumerable.Concat(collection.Where(d => d.ReadLocalValue(GroupingProperty) == grouping))));
            }

            /// <summary>
            /// Called when a property value changes.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
            private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(GroupComparer))
                {
                    collection.Recompose(grouping.GroupSort(collection));

                    return;
                }

                if (e.PropertyName == nameof(GroupSeparatorTemplate))
                {
                    collection.Recompose(grouping.GroupSort(collection.Where(d => d.ReadLocalValue(GroupingProperty) != grouping)));

                    return;
                }
            }

            #endregion

            #region Nested types

            /// <summary>
            /// Represents an listener of the collection change notification event.
            /// </summary>
            private class CollectionChangedListener : EventListener<NotifyCollectionChangedEventArgs, GroupingCollection>
            {
                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="CollectionChangedListener"/> class.
                /// </summary>
                /// <param name="handler">The event handler object.</param>
                internal CollectionChangedListener(GroupingCollection handler) : base(handler)
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
                protected override void HandleEvent(object sender, NotifyCollectionChangedEventArgs e, GroupingCollection handler)
                {
                    handler.OnCollectionChanged(sender, e);
                }

                #endregion
            }

            /// <summary>
            /// Represents an listener of the property change notification event.
            /// </summary>
            private class PropertyChangedListener : EventListener<PropertyChangedEventArgs, GroupingCollection>
            {
                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="PropertyChangedListener"/> class.
                /// </summary>
                /// <param name="handler">The event handler object.</param>
                internal PropertyChangedListener(GroupingCollection handler) : base(handler)
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
                protected override void HandleEvent(object sender, PropertyChangedEventArgs e, GroupingCollection handler)
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
