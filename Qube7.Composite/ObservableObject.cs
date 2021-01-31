using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Qube7.Composite
{
    /// <summary>
    /// Represents an object that supports property change notification.
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Identifies the name of the indexer property.
        /// </summary>
        public const string IndexerName = "Item[]";

        /// <summary>
        /// The trace category.
        /// </summary>
        private const string TraceCategory = "ObservableObject";

        /// <summary>
        /// The property change notification.
        /// </summary>
        private readonly INotification notification;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableObject"/> class.
        /// </summary>
        protected ObservableObject()
        {
            notification = Notification.FromType(GetType());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Notifies that object's property value has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <remarks>The <c>null</c> or <see cref="String.Empty"/> used for the <paramref name="propertyName"/> indicate that all properties on the object have changed.</remarks>
        protected void NotifyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            notification.Raise(this, e);
        }

        /// <summary>
        /// Determines whether the current object declares an instance property with the specified name.
        /// </summary>
        /// <param name="propertyName">The name of the property being tested.</param>
        [Conditional("DEBUG")]
        private void ValidateProperty(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                Trace.WriteLine(Format.Current(Strings.PropertyNotFound, propertyName, GetType()), TraceCategory);
            }
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Specifies that the property is dependent on the value of another property.
        /// </summary>
        internal interface IDependsOn
        {
            #region Properties

            /// <summary>
            /// Gets the names of the related properties declared in this <see cref="IDependsOn"/>.
            /// </summary>
            /// <value>The names of the related properties.</value>
            string[] Properties { get; }

            #endregion
        }

        /// <summary>
        /// Indicates that the attributed property is dependent on the value of another property.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, Inherited = false)]
        protected sealed class DependsOn : Attribute, IDependsOn
        {
            #region Fields

            /// <summary>
            /// The names of the related properties.
            /// </summary>
            private readonly string[] properties;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the names of the related properties declared in this <see cref="DependsOn"/>.
            /// </summary>
            /// <value>The names of the related properties.</value>
            public string[] Properties
            {
                get { return properties; }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="DependsOn"/> class.
            /// </summary>
            /// <param name="properties">The names of the properties that the property associated with this <see cref="DependsOn"/> depends on.</param>
            public DependsOn(params string[] properties)
            {
                this.properties = properties ?? Array.Empty<string>();
            }

            #endregion
        }

        /// <summary>
        /// Manages the property change notification of the <see cref="ObservableObject"/>.
        /// </summary>
        private interface INotification
        {
            #region Methods

            /// <summary>
            /// Raises the property change notification for the specified <see cref="ObservableObject"/>.
            /// </summary>
            /// <param name="observable">The target observable.</param>
            /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
            void Raise(ObservableObject observable, PropertyChangedEventArgs e);

            #endregion
        }

        /// <summary>
        /// Manages the property change notification of the <see cref="ObservableObject"/>.
        /// </summary>
        private class Notification : Dictionary<string, string[]>, INotification
        {
            #region Fields

            /// <summary>
            /// The notification table.
            /// </summary>
            private static readonly ConditionalWeakTable<Type, INotification> table = new ConditionalWeakTable<Type, INotification>();

            #endregion

            #region Methods

            /// <summary>
            /// Returns an <see cref="INotification"/> created for the specified target observable type.
            /// </summary>
            /// <param name="target">The type of the target observable.</param>
            /// <returns>The <see cref="INotification"/> for the <paramref name="target"/>.</returns>
            internal static INotification FromType(Type target)
            {
                return table.GetValue(target, Create);
            }

            /// <summary>
            /// Returns an <see cref="INotification"/> created for the specified target observable type.
            /// </summary>
            /// <param name="target">The type of the target observable.</param>
            /// <returns>The <see cref="INotification"/> for the <paramref name="target"/>.</returns>
            private static INotification Create(Type target)
            {
                Dictionary<string, Entry> dictionary = new Dictionary<string, Entry>();

                PropertyDescriptorCollection collection = TypeDescriptor.GetProperties(target);

                for (int i = 0; i < collection.Count; i++)
                {
                    Entry current = null;

                    string property = collection[i].Name;

                    AttributeCollection attributes = collection[i].Attributes;

                    for (int j = 0; j < attributes.Count; j++)
                    {
                        IDependsOn dependsOn = attributes[j] as IDependsOn;
                        if (dependsOn != null)
                        {
                            string[] properties = dependsOn.Properties;

                            for (int k = 0; k < properties.Length; k++)
                            {
                                if (string.IsNullOrEmpty(properties[k]) || properties[k] == property)
                                {
                                    continue;
                                }

                                current = current ?? Entry.Get(property, dictionary);

                                Entry entry = Entry.Get(properties[k], dictionary);

                                if (entry.Contains(current))
                                {
                                    continue;
                                }

                                entry.Add(current);
                            }
                        }
                    }
                }

                if (dictionary.Count > 0)
                {
                    Notification notification = new Notification();

                    foreach (Entry entry in dictionary.Values)
                    {
                        string[] properties = entry.Resolve();

                        if (properties != null)
                        {
                            notification.Add(entry.Name, properties);
                        }
                    }

                    return notification;
                }

                return PropertyChanged.Instance;
            }

            /// <summary>
            /// Raises the property change notification for the specified <see cref="ObservableObject"/>.
            /// </summary>
            /// <param name="observable">The target observable.</param>
            /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
            public void Raise(ObservableObject observable, PropertyChangedEventArgs e)
            {
                if (string.IsNullOrEmpty(e.PropertyName))
                {
                    Event.Raise(observable.PropertyChanged, observable, e);

                    return;
                }

                observable.ValidateProperty(e.PropertyName);

                Event.Raise(observable.PropertyChanged, observable, e);

                if (TryGetValue(e.PropertyName, out string[] properties))
                {
                    for (int i = 0; i < properties.Length; i++)
                    {
                        observable.ValidateProperty(properties[i]);

                        Event.Raise(observable.PropertyChanged, observable, new PropertyChangedEventArgs(properties[i]));
                    }
                }
            }

            #endregion

            #region Nested types

            /// <summary>
            /// Represents the property <see cref="DependsOn"/> association.
            /// </summary>
            private class Entry : List<Entry>
            {
                #region Fields

                /// <summary>
                /// The name of the associated property.
                /// </summary>
                private readonly string name;

                #endregion

                #region Properties

                /// <summary>
                /// Gets the name of the associated property.
                /// </summary>
                /// <value>The name of the property.</value>
                internal string Name
                {
                    get { return name; }
                }

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="Entry"/> class.
                /// </summary>
                /// <param name="name">The name of the associated property.</param>
                private Entry(string name)
                {
                    this.name = name;
                }

                #endregion

                #region Methods

                /// <summary>
                /// Returns an <see cref="Entry"/> associated with the specified property.
                /// </summary>
                /// <param name="name">The property name of the <see cref="Entry"/> to get.</param>
                /// <param name="dictionary">The dictionary from which to get the <see cref="Entry"/>.</param>
                /// <returns>An <see cref="Entry"/> associated with the specified property.</returns>
                internal static Entry Get(string name, Dictionary<string, Entry> dictionary)
                {
                    if (!dictionary.TryGetValue(name, out Entry entry))
                    {
                        entry = new Entry(name);

                        dictionary.Add(name, entry);
                    }

                    return entry;
                }

                /// <summary>
                /// Resolves the collection of dependent properties of the associated property.
                /// </summary>
                /// <returns>The collection of dependent properties.</returns>
                internal string[] Resolve()
                {
                    List<string> list = new List<string>();

                    Resolve(this, list);

                    if (list.Count > 1)
                    {
                        int count = list.Count - 1;

                        string[] array = new string[count];

                        list.CopyTo(1, array, 0, count);

                        return array;
                    }

                    return null;
                }

                /// <summary>
                /// Resolves the collection of dependent properties for the specified <see cref="Entry"/>.
                /// </summary>
                /// <param name="entry">The <see cref="Entry"/> to resolve.</param>
                /// <param name="list">The collection of dependent properties.</param>
                private void Resolve(Entry entry, List<string> list)
                {
                    if (list.Contains(entry.Name))
                    {
                        return;
                    }

                    list.Add(entry.Name);

                    for (int i = 0; i < entry.Count; i++)
                    {
                        Resolve(entry[i], list);
                    }
                }

                #endregion
            }

            /// <summary>
            /// Manages the property change notification of the <see cref="ObservableObject"/>.
            /// </summary>
            private class PropertyChanged : INotification
            {
                #region Fields

                /// <summary>
                /// Represents the cached instance of the <see cref="PropertyChanged"/> class.
                /// </summary>
                internal static readonly PropertyChanged Instance = new PropertyChanged();

                #endregion

                #region Constructors

                /// <summary>
                /// Prevents a default instance of the <see cref="PropertyChanged"/> class from being created.
                /// </summary>
                private PropertyChanged()
                {
                }

                #endregion

                #region Methods

                /// <summary>
                /// Raises the property change notification for the specified <see cref="ObservableObject"/>.
                /// </summary>
                /// <param name="observable">The target observable.</param>
                /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
                public void Raise(ObservableObject observable, PropertyChangedEventArgs e)
                {
                    ValidateProperty(observable, e.PropertyName);

                    Event.Raise(observable.PropertyChanged, observable, e);
                }

                /// <summary>
                /// Determines whether the specified object declares an instance property with the specified name.
                /// </summary>
                /// <param name="observable">The target observable.</param>
                /// <param name="propertyName">The name of the property being tested.</param>
                [Conditional("DEBUG")]
                private static void ValidateProperty(ObservableObject observable, string propertyName)
                {
                    if (string.IsNullOrEmpty(propertyName))
                    {
                        return;
                    }

                    observable.ValidateProperty(propertyName);
                }

                #endregion
            }

            #endregion
        }

        #endregion
    }
}
